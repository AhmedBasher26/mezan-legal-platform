using Microsoft.AspNetCore.Mvc;

namespace Mezan.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class ApiControllerBase : ControllerBase
{
}
