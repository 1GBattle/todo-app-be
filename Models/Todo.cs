public class TodoItem
{
  public int Id { get; set; }
  public required string Title { get; set; }
  public bool IsComplete { get; set; }
  public required string Description { get; set; }
  public DateTime DueDate { get; set; }
  public DateTime CreatedDate { get; set; }
}