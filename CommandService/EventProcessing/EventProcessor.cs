using System.Text.Json;
using AutoMapper;
using CommandService.Data;
using CommandService.Dtos;
using CommandService.Models;

namespace CommandService.EventProcessing;

public class EventProcessor : IEventProcessor
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public IMapper _mapper { get; }

    public EventProcessor(
        IServiceScopeFactory serviceScopeFactory,
        IMapper mapper)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _mapper = mapper;
    }

    public void ProcessEvent(string message)
    {
        var eventType = DetermineEvent(message);
        Console.WriteLine($"--> {eventType} determined");

        _ = eventType switch
        {
            EventType.PlatformPublished => AddPlatform(message),
            _ => 0            
        };
    }

    private int AddPlatform(string platformPublishedMessage)
    {
        using (var scope = _serviceScopeFactory.CreateScope())
        {
            var repo = scope.ServiceProvider.GetRequiredService<ICommandRepository>();

            var platformPublishedDto = JsonSerializer.Deserialize<PlatformPublishDto>(platformPublishedMessage);

            try
            {
                var platform = _mapper.Map<Platform>(platformPublishedDto);
                if (!repo.ExternalPlatformExists(platform.ExternalId))
                {
                    repo.CreatePlatform(platform);
                    repo.SaveChanges();
                }
                else
                {
                Console.WriteLine($"--> Platform Id {platform.ExternalId}  already exists");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Could not add platform to database : {ex.Message}");
            }
        }

        return 1;
    }

    private EventType DetermineEvent(string notificationMessage)
    {
        Console.WriteLine("--> Determining Event");

        var eventType = JsonSerializer.Deserialize<GenericEventDto>(notificationMessage);

        if (eventType == null)
        {
            return EventType.Undetermined;
        }

        return eventType.Event switch
        {
            "Platform_Published" => EventType.PlatformPublished,
            _ => EventType.Undetermined,
        };
    }
}

public enum EventType
{
    PlatformPublished,
    Undetermined
}