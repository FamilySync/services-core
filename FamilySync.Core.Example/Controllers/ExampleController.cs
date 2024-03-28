using FamilySync.Core.Example.Services;
using Microsoft.AspNetCore.Mvc;

namespace FamilySync.Core.Example.Controllers;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
public class ExampleController : ControllerBase
{
    [HttpGet("example")]
    [ProducesResponseType(200)]
    public ActionResult<string> Example([FromQuery] string name)
    {
        return Ok($"Hello {name}!");
    }
    
    [HttpGet("example-service")]
    [ProducesResponseType(200)]
    public async Task<ActionResult<string>> ExampleService([FromServices] IExampleService exampleService)
    {
        return Ok(await exampleService.GetExampleData());
    }
}