using System;
using System.Linq;
using BattleShipBrain;
using Microsoft.EntityFrameworkCore;

namespace DAL
{
    public class ApplicationDbContext : DbContext
    {
        
    // dotnet ef database update --project DAL --startup-project BattleShipConsoleApp 
    // one user ms sql (only on windows) - Server=(localdb)\mssqllocaldb;Database=MyDatabase;Trusted_Connection=True;
    private static string ConnectionString =
        "Server=barrel.itcollege.ee;User Id=student;Password=Student.Pass.1;Database=student_arljub_battleship;MultipleActiveResultSets=true";
    
    
    public DbSet<GameConfigSaved> GameConfigSaves { get; set; } = default!;
    public DbSet<GameStateSaved> GameStateSaves { get; set; } = default!;
    // public DbSet<CourseDeclaration> CourseDeclarations { get; set; } = default!;
    // public DbSet<Grade> Grades { get; set; } = default!;
    // public DbSet<Homework> Homeworks { get; set; } = default!;
    // public DbSet<Person> Persons { get; set; } = default!;
    
    
    // not recommended - do not hardcode DB conf!
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(ConnectionString);
    }
    
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    
        // remove the cascade delete globally
        foreach (var relationship in modelBuilder.Model
            .GetEntityTypes()
            .Where(e => !e.IsOwned())
            .SelectMany(e => e.GetForeignKeys()))
        {
            relationship.DeleteBehavior = DeleteBehavior.Restrict;
        }
    }
    }
}