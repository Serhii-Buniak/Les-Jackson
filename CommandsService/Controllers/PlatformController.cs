using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using CommandsService.Models;
using Microsoft.AspNetCore.Mvc;

namespace CommandsService.Controllers;

[Route("api/c/[controller]")]
public class PlatformController : ApiControllerBase
{
    private readonly ICommandRepo _repo;
    private readonly IMapper _mapper;

    public PlatformController(ICommandRepo repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    [HttpGet]
    public IActionResult GetPlatforms()
    {
        Console.WriteLine("--> Getting platforms from CommandsService");

        IEnumerable<Platform> entities = _repo.GetAllPlatforms();

        return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(entities));
    }

    [HttpPost]
    public IActionResult TestInboundConnection()
    {
        Console.WriteLine("--->>> Post PlatformController TestInboundConnection");
        return Ok("TestInboundConnection");
    }
}