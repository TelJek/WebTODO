using Microsoft.EntityFrameworkCore;

namespace WebBattleShip.DAL;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }
}
