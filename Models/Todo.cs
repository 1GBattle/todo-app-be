public class TodoItem
{
  public required string Id { get; init; }
  public required string UserId { get; init; } 
  public required string Title { get; set; }
  public bool IsComplete { get; set; }
  public required string Description { get; set; }
  public DateTime DueDate { get; set; }
  public DateTime CreatedDate { get; init; }
}