using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;
using PlatformService.SyncDataServices.Http;

namespace PlatformService.Controllers;

[Route("api/platforms")]
[ApiController]
public class PlatformController : ControllerBase
{
    private readonly IPlatformRepository _platformRepository;
    private readonly IMapper _mapper;
    private readonly ICommandDataClient _commandDataClient;
    private readonly IMessageBusClient _messageBusClient;

    public PlatformController(
        IPlatformRepository platformRepository,
        IMapper mapper,
        ICommandDataClient commandDataClient,
        IMessageBusClient messageBusClient)
    {
        _platformRepository = platformRepository;
        _mapper = mapper;
        _commandDataClient = commandDataClient;
        _messageBusClient = messageBusClient;
    }

    [HttpGet]
    public ActionResult<IEnumerable<PlatformReadDto>> GetPlatforms()
    {
        Console.WriteLine("--> Getting platforms...");

        var allPlatforms = _platformRepository.GetAllPlatforms();
        return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(allPlatforms));
    }

    [HttpGet("{id}", Name = "GetPlatformById")]
    public ActionResult<PlatformReadDto> GetPlatformById(int id)
    {
        Console.WriteLine($"--> Getting platform by id {id}...");
        var platformItem = _platformRepository.GetbyId(id);

        if (platformItem == null)
        {
            return NotFound();
        }

        return Ok(_mapper.Map<PlatformReadDto>(platformItem));
    }

    [HttpPost]
    public async Task<ActionResult<PlatformReadDto>> CreatePlatform(PlatformCreateDto platformCreateDto)
    {
        Console.WriteLine("--> Create platform...");

        var platformModel = _mapper.Map<Platform>(platformCreateDto);
        _platformRepository.CreatePlatform(platformModel);
        _platformRepository.SaveChanges();

        var platformReadDto = _mapper.Map<PlatformReadDto>(platformModel);

        // send Sync Message
        try
        {
            await _commandDataClient.SendPlatformToCommand(platformReadDto);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"--> Could not send synchronously : {ex.Message}");
        }

        // send Async Message
        try
        {
            var platformPublishDto = _mapper.Map<PlatformPublishDto>(platformReadDto);
            platformPublishDto.Event = "Platform_Published";
            _messageBusClient.PublishNewPlatform(platformPublishDto);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"--> Could not send asynchronously : {ex.Message}");
        }

        return CreatedAtRoute(nameof(GetPlatformById), new { Id = platformReadDto.Id }, platformReadDto);
    }
}