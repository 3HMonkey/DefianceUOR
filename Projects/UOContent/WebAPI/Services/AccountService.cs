using System.Collections.Generic;
using System.Linq;
using Serilog;
using Server.Accounting;

namespace Server.WebAPI.Services;

public interface IAccountService
{
    IEnumerable<IAccount> GetAccounts();
    IAccount GetAccount(string username);
}

public sealed class AccountService: IAccountService
{
    private readonly ILogger _logger;

    public AccountService(ILogger logger)
    {
        _logger= logger;
    }

    public IEnumerable<IAccount> GetAccounts()
    {

        var accounts = Accounts.GetAccounts();
        _logger.Information($"Sent {accounts.Count()} accounts over API");
        return Accounts.GetAccounts();
    }

    public IAccount GetAccount(string username)
    {

        var account = Accounts.GetAccount(username);
        _logger.Information($"Sent {username}'s account over API");
        return account;
    }
}
