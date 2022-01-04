using PlatformService.Models;

namespace PlatformService.Data;

public class PlatformRepository : IPlatformRepository
{
    private readonly AppDbContext _context;

    public PlatformRepository(AppDbContext context) => _context = context;

    public void CreatePlatform(Platform platform) => _context.Add(platform);

    public IEnumerable<Platform> GetAllPlatforms() => _context.Platforms.AsEnumerable();

    public Platform? GetbyId(int id) => _context.Platforms.FirstOrDefault(p => p.Id.Equals(id));

    public bool SaveChanges() => _context.SaveChanges() >= 0;
}