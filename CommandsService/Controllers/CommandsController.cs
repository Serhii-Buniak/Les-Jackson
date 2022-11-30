using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using CommandsService.Models;
using Microsoft.AspNetCore.Mvc;

namespace CommandsService.Controllers;

[Route("api/c/platform/{platformId}/[controller]")]
public class CommandsController : ApiControllerBase
{
    private readonly ICommandRepo _repo;
    private readonly IMapper _mapper;

    public CommandsController(ICommandRepo repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;

    }

    [HttpGet]
    public IActionResult GetCommandsForPlatform(int platformId)
    {
        Console.WriteLine($"--> Hit GetCommandsForPlatform: {platformId}");

        if (!_repo.PlatformExist(platformId))
        {
            return NotFound();
        }

        IEnumerable<Command> commands = _repo.GetCommandsForPlatform(platformId);

        return Ok(_mapper.Map<IEnumerable<CommandReadDto>>(commands));
    }

    [HttpGet("{commandId}", Name = "GetCommandForPlatform")]
    public IActionResult GetCommandForPlatform(int platformId, int commandId)
    {
        Console.WriteLine($"--> Hit GetCommandsForPlatform: {platformId} / {commandId}");

        if (!_repo.PlatformExist(platformId))
        {
            return NotFound();
        }

        Command? command = _repo.GetCommand(platformId, commandId);

        if (command is null)
        {
            return NotFound();
        }

        return Ok(_mapper.Map<CommandReadDto>(command));
    }

    [HttpPost]
    public IActionResult CreateCommandForPlatform(int platformId, CommandCreateDto model)
    {
        Console.WriteLine($"--> Hit CreateCommandForPlatform: {platformId}");

        if (!_repo.PlatformExist(platformId))
        {
            return NotFound();
        }

        var command = _mapper.Map<Command>(model);

        _repo.CreateCommand(platformId, command);
        _repo.SaveChanges();

        var commandReadDto = _mapper.Map<CommandReadDto>(command);

        return CreatedAtAction(
            nameof(GetCommandsForPlatform),
            new { commandId = commandReadDto.Id, platformId = commandReadDto.PlatformId },
            commandReadDto
        );
    }
}