namespace TODO;

public class Todo
{
    public string Headline { get; set; }
    public string Description { get; set; }
    public string Priority { get; set; }
    public bool Done { get; set; } = false;
    public DateTime CreationDate { get; set; }
    public DateTime DueDate { get; set; }

    public Todo(string headline, string description, string priority, DateTime creationDate)
    {
        Headline = headline;
        Description = description;
        Priority = priority;
    }
}