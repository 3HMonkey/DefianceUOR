using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace Server.WebAPI.Controllers.V1;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[ApiVersion("1.0")]
public class ExampleController : ControllerBase
{
    [MapToApiVersion("1.0")]
    [HttpGet]
    public IEnumerable<string> Get()
    {
        return new string[] { "value1", "value2" };
    }

    [MapToApiVersion("1.0")]
    [HttpGet("{id}")]
    public string Get(int id)
    {
        return "value";
    }

    [MapToApiVersion("1.0")]
    [HttpPost]
    public void Post([FromBody] string value)
    {
    }

    [MapToApiVersion("1.0")]
    [HttpPut("{id}")]
    public void Put(int id, [FromBody] string value)
    {
    }

    [MapToApiVersion("1.0")]
    [HttpDelete("{id}")]
    public void Delete(int id)
    {
    }
}
