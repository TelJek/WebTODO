using System.ComponentModel.DataAnnotations;

namespace WebAppTODO.Domain;

public class CategoryDb
{
    public int Id { get; set; }
    [MaxLength(64)]
    public string CategoryName { get; set; } = default!;
    
    public ICollection<TodoDb>? TodoDbs { get; set; }
}