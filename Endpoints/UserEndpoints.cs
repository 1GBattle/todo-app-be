using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

public static class Users
{
    public static void RegisterUserEndpoints(this IEndpointRouteBuilder routes)
    {
        var users = routes.MapGroup("/api/users");

        users.MapPost(
            "/login",
            ([FromBody] User usr, HttpContext ctx) =>
            {
                if (usr.username == "admin" && usr.password == "password")
                {
                    var token = GenerateJwt(usr.username, usr.uid);
                    var cookieOptions = new CookieOptions()
                    {
                        Expires = DateTimeOffset.UtcNow.AddMinutes(30),
                        HttpOnly = true, // Accessible only by the server
                        IsEssential = true, // Required for GDPR compliance
                        SameSite = SameSiteMode.Strict,
                    };
                    ctx.Response.Cookies.Append("X-Access-Token", token, cookieOptions);
                    return Results.Ok();
                }

                return Results.Unauthorized();
            }
        );
    }

    private static string GenerateJwt(string username, string uid)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, username),
            new Claim(JwtRegisteredClaimNames.Jti, uid),
            new Claim(JwtRegisteredClaimNames.Jti, value: Guid.NewGuid().ToString()),
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes("123456789101112131415161718192021222324252627")
        );
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: "http://localhost:5112",
            audience: "http://localhost:5112",
            claims: claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static bool ValidateJwt(string authToken, string key)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParams = new TokenValidationParameters()
        {
            ValidateLifetime = true, // Because there is no expiration in the generated token
            ValidateAudience = true, // Because there is no audiance in the generated token
            ValidateIssuer = true, // Because there is no issuer in the generated token
            ValidIssuer = "http://localhost:5112",
            ValidAudience = "http://localhost:5112",
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes("123456789101112131415161718192021222324252627")
            ), 
        };
        

        SecurityToken validatedToken;
        IPrincipal principal = tokenHandler.ValidateToken(
            authToken,
            validationParams,
            out validatedToken
        );

        return true;
    }
}
