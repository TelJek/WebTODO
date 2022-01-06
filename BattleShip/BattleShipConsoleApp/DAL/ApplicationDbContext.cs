using System;
using System.Linq;
using BattleShipBrain;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace DAL
{
    public class ApplicationDbContext : DbContext
    {
        
    // dotnet ef database update --project DAL --startup-project BattleShipConsoleApp 
    // dotnet ef migrations add InitialCreate --project DAL --startup-project BattleShipConsoleApp 
    // one user ms sql (only on windows) - Server=(localdb)\mssqllocaldb;Database=MyDatabase;Trusted_Connection=True;
    private static string ConnectionString =
        "Server=barrel.itcollege.ee;User Id=student;Password=Student.Pass.1;Database=student_arljub_battleship;MultipleActiveResultSets=true";
    
    public DbSet<GameConfigSaved?> GameConfigSaves { get; set; } = default!;
    public DbSet<GameStateSaved> GameStateSaves { get; set; } = default!;

    public DbSet<StartedGame> StartedGames { get; set; } = default!;

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

    public bool DeleteFromDbByName(string NameToDelete, string table, string columnName)
    {
        var db = this;

        using (SqlConnection con = new SqlConnection(ConnectionString))
        {
            con.Open();
            using (SqlCommand command = new SqlCommand("DELETE FROM " + table + " WHERE " + columnName + " = '" + NameToDelete+"'", con))
            {
                command.ExecuteNonQuery();
            }
            con.Close();
        }
        
        return false;
    }
    
    }
    
}