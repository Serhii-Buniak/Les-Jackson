using Microsoft.AspNetCore.Mvc;

namespace PlatformService.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class ApiControllerBase : ControllerBase
{

}