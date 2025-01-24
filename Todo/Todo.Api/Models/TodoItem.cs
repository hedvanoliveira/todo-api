namespace Todo.Api.Models;

public class TodoItem
{
    public int Id { get; set; }
    public string Description { get; set; }
    public DateTime Created { get; set; }
}
