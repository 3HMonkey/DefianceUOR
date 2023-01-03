using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace Server.WebAPI.Controllers.V2;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[ApiVersion("2.0")]
public class ExampleController : ControllerBase
{
    [MapToApiVersion("2.0")]
    [HttpGet]
    public IEnumerable<string> Get()
    {
        return new string[] { "value1", "value2" };
    }

    [MapToApiVersion("2.0")]
    [HttpGet("{id}")]
    public string Get(int id)
    {
        return "value";
    }

    [MapToApiVersion("2.0")]
    [HttpPost]
    public void Post([FromBody] string value)
    {
    }

    [MapToApiVersion("2.0")]
    [HttpPut("{id}")]
    public void Put(int id, [FromBody] string value)
    {
    }

    [MapToApiVersion("2.0")]
    [HttpDelete("{id}")]
    public void Delete(int id)
    {
    }
}
