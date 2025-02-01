using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

public static class Users
{
    public static void RegisterUserEndpoints(this IEndpointRouteBuilder routes)
    {
        var users = routes.MapGroup("/api/users");

        users.MapPost("/login", ([FromBody] User usr) =>
        {
            if (usr is { username: "admin", password: "password" })
            {
                var token = GenerateJwt(usr.username, usr.uid);
                return Results.Ok(new { token });
            }

            return Results.Unauthorized();
        });
    }

    private static string GenerateJwt(string username, string uid)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, username),
            new Claim(JwtRegisteredClaimNames.Jti, value: Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(uid));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: "localhost:5112",
            audience: "localhost:5112",
            claims: claims,
            expires: DateTime.Now.AddHours(24),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
