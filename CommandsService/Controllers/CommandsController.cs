using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using CommandsService.Models;
using Microsoft.AspNetCore.Mvc;

namespace CommandsService.Controllers;

[Route("/api/c/platforms/{platformId}/[controller]")]
[ApiController]
public class CommandsController : ControllerBase
{
    private readonly ICommandRepo repository;
    private readonly IMapper mapper;

    public CommandsController(ICommandRepo repository, IMapper mapper)
    {
        this.repository = repository;
        this.mapper = mapper;
    }

    [HttpGet]
    public ActionResult<IEnumerable<CommandReadDto>> GetCommands(int platformId) {
        Console.WriteLine($"QR501: {nameof(GetCommands)}()");
        if(!repository.IsPlatformExists(platformId)) {
            return NotFound();
        }

        return Ok(mapper.Map<IEnumerable<CommandReadDto>>(
            repository.GetCommandsForPlatform(platformId)));
    }

    [HttpGet("{id}", Name = nameof(GetCommandForPlatform))]
    public ActionResult<CommandReadDto> GetCommandForPlatform(int platformId, int id) {
        if(!repository.IsPlatformExists(platformId)) {
            return NotFound();
        }

        var command = repository.GetCommand(platformId, id);

        if(command is null) {
            return NotFound();
        }

        return base.Ok(mapper.Map<CommandReadDto>(command));
    }

    [HttpPost]
    public ActionResult CreateCommand(int platformId, CommandCreateDto commandCreateDto) {
        Console.WriteLine($"QR501: {nameof(CreateCommand)}({platformId}, ...)");
        if(!repository.IsPlatformExists(platformId)) {
            Console.WriteLine("QR501: Platform not found");
            return NotFound();
        }

        var command = mapper.Map<Command>(commandCreateDto);

        repository.CreateCommand(platformId, command);
        repository.SaveChanges();

        var commandReadDto = mapper.Map<CommandReadDto>(command);

        Console.WriteLine("QR501: Command created");

        return CreatedAtRoute(
            nameof(GetCommandForPlatform), 
            new { platformId = platformId, id = commandReadDto.Id }, 
            commandReadDto);
    }
}