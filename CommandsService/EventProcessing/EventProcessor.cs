using System.Text.Json;
using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using CommandsService.Models;

namespace CommandsService.EventProcessing;

public class EventProcessor : IEventProcessor
{
    private readonly IServiceScopeFactory scopeFactory;
    private readonly IMapper mapper;

    public EventProcessor(IServiceScopeFactory scopeFactory, IMapper mapper)
    {
        this.scopeFactory = scopeFactory;
        this.mapper = mapper;
    }

    public void ProcessEvent(string message)
    {
        var eventType = DetermineEvent(message);

        switch(eventType) {
            case EventType.PlatformPublished:
                AddPlatform(message);
                break;
            default:
                break;
        }
    }

    private EventType DetermineEvent(string notificationMessage) {
        Console.WriteLine("QR501: Determining event...");

        var eventType = JsonSerializer.Deserialize<GenericEventDto>(notificationMessage);

        if(eventType is null) {
            return default;
        }

        switch(eventType.Event) {
            case "Platform_Published":
                Console.WriteLine("QR501: Platform_Published event detected");
                return EventType.PlatformPublished;
            default:
                Console.WriteLine("QR501: Could not determine event type");
                return default;
        }
    }

    private void AddPlatform(string platformPublishedMessage) {
        using (var scope = scopeFactory.CreateScope())
        {
            var repository = scope.ServiceProvider.GetService<ICommandRepo>();

            if(repository is null) {
                Console.WriteLine("QR501: Couldn't get a repository");
            }

            var platformPublishedDto = JsonSerializer.Deserialize<PlatformPublishedDto>(platformPublishedMessage);

            try
            {
                var platform = mapper.Map<Platform>(platformPublishedDto);

                if(repository.IsExternalPlatformExists(platform.ExternalId)) {
                    Console.WriteLine("QR501: Platform already exists");
                } else {
                    Console.WriteLine("QR501: Creating Platform...");
                    repository.CreatePlatform(platform);
                    repository.SaveChanges();
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"QR501: Could not add platform to DB: {ex.Message}");
            }
        }
    }
}

enum EventType
{
    UNDEFINED = 0,
    PlatformPublished,
}