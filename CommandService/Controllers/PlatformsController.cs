using AutoMapper;
using CommandService.Data;
using Microsoft.AspNetCore.Mvc;

namespace CommandService.Controllers;

[Route("api/c/[controller]")]
[ApiController]
public class PlatformsController : ControllerBase
{
    private readonly ICommandRepository _repository;
    private readonly IMapper _mapper;

    public PlatformsController(
        ICommandRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    [HttpPost]
    public ActionResult TestInboundConnetion()
    {
        Console.WriteLine("--> Inbound POST # Command Service");
        return Ok("Inbound test OK from platformsController");
    }
}