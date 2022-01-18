using Microsoft.EntityFrameworkCore;
using PlatformService.Models;

namespace PlatformService.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> option) : base(option)
    {
        if(option == null)
        {
            throw new ArgumentException(nameof(option));
        }
    }

    public DbSet<Platform> Platforms { get; set; }
}