
using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace CommandsService.Controllers;

[Route("/api/c/[controller]")]
[ApiController]
public class PlatformsController : ControllerBase
{
    private readonly ICommandRepo repository;
    private readonly IMapper mapper;

    public PlatformsController(ICommandRepo repository, IMapper mapper)
    {
        this.repository = repository;
        this.mapper = mapper;
    }

    [HttpGet]
    public IEnumerable<PlatformReadDto> GetPlatforms() {
        Console.WriteLine($"QR501: {nameof(GetPlatforms)}()");
        return mapper.Map<IEnumerable<PlatformReadDto>>(repository.GetAllPlatforms());
    }

    [HttpPost]
    public ActionResult TestInboundConnection() {
        Console.WriteLine("QR501: Inbound POST @ Commands Service");
        
        return Ok("Inbound POST Success");
    }
}