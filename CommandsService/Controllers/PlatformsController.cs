
using Microsoft.AspNetCore.Mvc;

namespace CommandsService.Controllers;

[Route("/api/c/[controller]")]
[ApiController]
public class PlatformsController : ControllerBase
{
    public PlatformsController()
    {
        
    }

    [HttpPost]
    public ActionResult TestInboundConnection() {
        Console.WriteLine("QR501: Inbound POST @ Commands Service");
        
        return Ok("Inbound POST Success");
    }
}