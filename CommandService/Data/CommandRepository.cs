using CommandService.Models;

namespace CommandService.Data;

public class CommandRepository : ICommandRepository
{
    private readonly AppDbContext _context;

    public CommandRepository(AppDbContext context) => _context = context;

    public void CreateCommand(int platformId, Command command)
    {
        command.PlatformId = platformId;
        _context.Commands?.Add(command);
    }

    public void CreatePlatform(Platform platform) => _context.Platforms?.Add(platform);

    public bool ExternalPlatformExists(int externalPlatformId) =>
        _context.Platforms != null && _context.Platforms.Any(p => p.ExternalId.Equals(externalPlatformId));

    public IEnumerable<Platform> GetAllPlatforms() =>
        _context.Platforms == null ?
            Enumerable.Empty<Platform>() :
            _context.Platforms.ToList();

    public Command? GetCommand(int platformId, int commandId) =>
        _context.Commands == null ?
            null :
            _context.Commands.FirstOrDefault(c => c.PlatformId.Equals(platformId) && c.Id.Equals(commandId));

    public IEnumerable<Command> GetCommandsForPlatform(int platformId) =>
        _context.Commands == null ?
            Enumerable.Empty<Command>() :
            _context.Commands.Where(c => c.PlatformId.Equals(platformId));

    public bool PlatformExists(int platformId) =>
        _context.Platforms != null && _context.Platforms.Any(p => p.Id.Equals(platformId));

    public bool SaveChanges() => _context.SaveChanges() >= 0;
}