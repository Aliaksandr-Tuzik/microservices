using CommandsService.Models;

namespace CommandsService.Data;

public class CommandRepo : ICommandRepo
{
    private readonly AppDbContext context;

    public CommandRepo(AppDbContext context)
    {
        this.context = context;
    }

    public void CreateCommand(int platformId, Command command)
    {
        command.PlatformId = platformId;
        
        context.Commands.Add(command);
    }

    public void CreatePlatform(Platform platform)
    {
        context.Add(platform);
    }

    public IEnumerable<Platform> GetAllPlatforms()
    {
        return context.Platforms.ToList();
    }

    public Command? GetCommand(int platformId, int commandId)
    {
        return context.Commands
            .SingleOrDefault(c => c.PlatformId == platformId && c.Id == commandId);
    }

    public IEnumerable<Command> GetCommandsForPlatform(int platformId)
    {
        return context.Commands
            .Where(c => c.PlatformId == platformId)
            .ToList();
    }

    public bool IsExternalPlatformExists(int externalPlatformId)
    {
        return context.Platforms.Any(p => p.ExternalId == externalPlatformId);
    }

    public bool IsPlatformExists(int platformId)
    {
        return context.Platforms.Any(p => p.Id == platformId);
    }

    public bool SaveChanges()
    {
        return (context.SaveChanges() >= 0);
    }
}