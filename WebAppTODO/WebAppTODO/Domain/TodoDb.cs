using System.ComponentModel.DataAnnotations;

namespace WebAppTODO.Domain;

public class TodoDb
{
    public int Id { get; set; }
    [MaxLength(64)]
    public string SessionID { get; set; } = default!;
    [MaxLength(64)]
    public string Headline { get; set; } = default!;
    [MaxLength(16)]
    public int Priority { get; set; } = default!;
    public string Description { get; set; } = default!;
    public bool Done { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime DueDate { get; set; }

    public CategoryDb CategoryDb { get; set; } = default!;
    public int CategoryDbId { get; set; }
}