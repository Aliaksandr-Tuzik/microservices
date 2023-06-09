using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;
using PlatformService.SyncDataServices.Http;

namespace PlatformService.Controllers;

[Route("/api/[controller]")]
[ApiController]
public class PlatformsController : ControllerBase
{
    private readonly IPlatformRepo platformRepo;
    private readonly IMapper mapper;
    private readonly ICommandDataClient commandDataClient;
    private readonly IMessageBusClient messageBusClient;

    public PlatformsController(
        IPlatformRepo platformRepo,
        IMapper mapper,
        ICommandDataClient commandDataClient,
        IMessageBusClient messageBusClient)
    {
        this.platformRepo = platformRepo;
        this.mapper = mapper;
        this.commandDataClient = commandDataClient;
        this.messageBusClient = messageBusClient;
    }

    [HttpGet]
    public IEnumerable<PlatformReadDto> GetPlatforms() {
        Console.WriteLine("QR501: Getting Platforms...");
        return mapper.Map<IEnumerable<PlatformReadDto>>(platformRepo.GetAllPlatforms());
    }

    [HttpGet("{id}", Name = nameof(GetPlatformById))]
    public ActionResult<PlatformReadDto> GetPlatformById(int id) {
        var platform = platformRepo.GetPlatformById(id);

        if(platform is null) {
            return NotFound();
        }

        return Ok(mapper.Map<PlatformReadDto>(platform));
    }

    [HttpPost(Name = nameof(CreatePlatform))]
    public async Task<ActionResult<PlatformReadDto>> CreatePlatform(PlatformCreateDto platform) {
        Console.WriteLine("QR501: Creating Platform...");
        var model = mapper.Map<Platform>(platform);
        
        platformRepo.CreatePlatform(model);
        platformRepo.SaveChanges();

        var readDto = mapper.Map<PlatformReadDto>(model);

        // Send sync message
        try
        {
            await commandDataClient.SendPlatformToCommand(readDto);
        }
        catch (System.Exception ex)
        {
            Console.WriteLine($"QR501: Could not send sync message to Command Service: {ex.Message}");
        }

        // Send async message
        try
        {
            var platformPublishedDto = mapper.Map<PlatformPublishedDto>(readDto);

            platformPublishedDto.Event = "Platform_Published";

            messageBusClient.PublishNewPlatform(platformPublishedDto);
        }
        catch (System.Exception ex)
        {
            Console.WriteLine($"QR501: Could not send async message to Command Service: {ex.Message}");
        }

        return base.CreatedAtRoute(
            nameof(GetPlatformById),
            new { Id = readDto.Id },
            readDto
        );
    }
}