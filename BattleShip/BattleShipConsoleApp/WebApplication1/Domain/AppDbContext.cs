using BattleShipBrain;
using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Domain;

public class AppDbContext: DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }
}