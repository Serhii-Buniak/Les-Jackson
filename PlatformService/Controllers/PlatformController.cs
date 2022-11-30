using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;
using PlatformService.SyncDataServices.Http;

namespace PlatformService.Controllers;

public class PlatformController : ApiControllerBase
{
    private readonly IPlatformRepo _repository;
    private readonly IMapper _mapper;
    private readonly ICommandDataClient _commandDataClient;
    private readonly IMessageBusClient _messageBus;

    public PlatformController(IPlatformRepo repository,
    IMapper mapper,
    ICommandDataClient commandDataClient,
    IMessageBusClient messageBus)
    {
        _repository = repository;
        _mapper = mapper;
        _commandDataClient = commandDataClient;
        _messageBus = messageBus;
    }

    [HttpGet]
    public IActionResult GetPlatforms()
    {
        IEnumerable<Platform> entities = _repository.GetAllPlatforms();
        var platforms = _mapper.Map<IEnumerable<PlatformReadDto>>(entities);
        return Ok(platforms);
    }

    [HttpGet("{id}")]
    public IActionResult GetPlatformById(int id)
    {
        Platform? entity = _repository.GetPlatfromById(id);

        if (entity == null)
        {
            return NotFound();
        }

        return Ok(_mapper.Map<PlatformReadDto>(entity));
    }

    [HttpPost]
    public async Task<IActionResult> CreatePlatform(PlatformCreateDto platformCreate)
    {
        var entity = _mapper.Map<Platform>(platformCreate);
        _repository.CreatePlatform(entity);
        _repository.SaveChanges();
        var platformReadDto = _mapper.Map<PlatformReadDto>(entity);

        // Sync
        try
        {
            await _commandDataClient.SendPlatformToCommand(platformReadDto);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"--> Could not set syncronously: ${ex.Message}");
        }

        // Async
        try
        {
            var platformPublishedDto = _mapper.Map<PlatformPublishedDto>(platformReadDto);
            platformPublishedDto.Event = "Platform_Published";
            _messageBus.PublishNewPlatform(platformPublishedDto);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"--> Could not set asyncronously: ${ex.Message}");
        }

        return CreatedAtAction(nameof(GetPlatformById), new { Id = entity.Id }, platformReadDto);
    }
}