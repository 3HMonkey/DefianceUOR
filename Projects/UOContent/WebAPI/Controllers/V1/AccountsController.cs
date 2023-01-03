using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Server.Accounting;
using Server.WebAPI.Services;

namespace Server.WebAPI.Controllers.V1;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[ApiVersion("1.0")]
public class AccountsController : ControllerBase
{
    private readonly IAccountService _accountService;

    public AccountsController(IAccountService accountService) => _accountService = accountService;

    [MapToApiVersion("1.0")]
    [HttpGet]
    public  IEnumerable<IAccount> Get() => _accountService.GetAccounts().ToArray();

    [MapToApiVersion("1.0")]
    [HttpGet("{username}")]
    public string Get(string username) => JsonSerializer.Serialize(_accountService.GetAccount(username));

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
