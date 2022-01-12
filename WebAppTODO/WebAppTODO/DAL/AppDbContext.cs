using Microsoft.EntityFrameworkCore;
using WebAppTODO.Domain;

namespace WebAppTODO.DAL;

public class AppDbContext: DbContext
{
    public DbSet<TodoDb> TodoDbs { get; set; } = default!;
    public DbSet<CategoryDb> CategoryDbs { get; set; } = default!;
    
    public AppDbContext(DbContextOptions<AppDbContext> options): base(options)
    {
        
    }
}