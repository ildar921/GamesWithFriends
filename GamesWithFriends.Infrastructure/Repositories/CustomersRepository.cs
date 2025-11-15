using System.Diagnostics;
using System.Linq.Expressions;
using GamesWithFriends.Core.Entities;
using GamesWithFriends.Core.Enums;
using GamesWithFriends.Infrastructure.Contexts;
using GamesWithFriends.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace GamesWithFriends.Infrastructure.Repositories;

public sealed class CustomersRepository(AppDbContext context, IHasherService hasher) : ICustomersRepository
{
    public async Task<IReadOnlyList<Customer>?> GetAllAsync()
    {
        try
        {
            return await context.Customers
                .AsNoTracking()
                .ToListAsync();
        }
        catch (Exception e)
        {
            Debug.WriteLine(e);

            return null;
        }
    }

    public async Task<Customer?> GetAsync(string username)
    {
        try
        {
            return await context.Customers
                .AsNoTracking()
                .FirstOrDefaultAsync(customer => customer.Username == username);
        }
        catch (Exception e)
        {
            Debug.WriteLine(e);

            return null;
        }
    }

    public async Task<string?> GetUsernameAsync(string username)
    {
        try
        {
            return await context.Customers
                .AsNoTracking()
                .Where(customer => customer.Username == username)
                .Select(customer => customer.Username)
                .FirstOrDefaultAsync();
        }
        catch (Exception e)
        {
            Debug.WriteLine(e);

            return null;
        }
    }

    public async Task<string?> GetPasswordHashAsync(string username)
    {
        try
        {
            return await context.Customers
                .AsNoTracking()
                .Where(customer => customer.Username == username)
                .Select(customer => customer.PasswordHash)
                .FirstOrDefaultAsync();
        }
        catch
            (Exception e)
        {
            Debug.WriteLine(e);

            return null;
        }
    }

    public async Task<decimal?> GetBalanceAsync(string username)
    {
        try
        {
            if (!await ExistsAsync(username))
                return null;

            return await context.Customers
                .AsNoTracking()
                .Where(customer => customer.Username == username)
                .Select(customer => customer.Balance)
                .FirstOrDefaultAsync();
        }
        catch
            (Exception e)
        {
            Debug.WriteLine(e);

            return null;
        }
    }

    public async Task<RoleType?> GetRoleAsync(string username)
    {
        try
        {
            return await context.Customers
                .AsNoTracking()
                .Where(customer => customer.Username == username)
                .Select(customer => customer.Role)
                .FirstOrDefaultAsync();
        }
        catch
            (Exception e)
        {
            Debug.WriteLine(e);

            return null;
        }
    }

    public async Task<(Customer?, BackendActionResult)> AddAsync(Customer customer)
    {
        try
        {
            if (await ExistsAsync(customer.Username))
                return (null, BackendActionResult.AlreadyExists);

            await context.Customers.AddAsync(customer);
            await context.SaveChangesAsync();

            return (customer, BackendActionResult.Success);
        }
        catch (Exception e)
        {
            Debug.WriteLine(e);

            return (null, BackendActionResult.Error);
        }
    }

    public async Task<(Customer?, BackendActionResult)> AddAsync(string username, string password)
    {
        try
        {
            if (await ExistsAsync(username))
                return (null, BackendActionResult.AlreadyExists);

            var customer = new Customer
            {
                Username = username,
                PasswordHash = hasher.Hash(password)
            };

            await context.Customers.AddAsync(customer);
            await context.SaveChangesAsync();

            return (customer, BackendActionResult.Success);
        }
        catch (Exception e)
        {
            Debug.WriteLine(e);

            return (null, BackendActionResult.Error);
        }
    }

    public async Task<BackendActionResult> UpdateUsernameAsync(string username, string newUsername)
    {
        try
        {
            return await ExecuteUpdateAsync(
                customer => customer.Username == username,
                calls => calls.SetProperty(customer => customer.Username, newUsername));
        }
        catch (Exception e)
        {
            Debug.WriteLine(e);

            return BackendActionResult.Error;
        }
    }

    public async Task<BackendActionResult> UpdatePasswordAsync(string username, string newPassword)
    {
        try
        {
            return await ExecuteUpdateAsync(
                customer => customer.Username == username,
                calls => calls.SetProperty(customer => customer.PasswordHash, hasher.Hash(newPassword)));
        }
        catch (Exception e)
        {
            Debug.WriteLine(e);

            return BackendActionResult.Error;
        }
    }

    public async Task<BackendActionResult> UpdateBalanceAsync(string username, decimal newBalance)
    {
        try
        {
            return await ExecuteUpdateAsync(
                customer => customer.Username == username,
                calls => calls.SetProperty(customer => customer.Balance, newBalance));
        }
        catch (Exception e)
        {
            Debug.WriteLine(e);

            return BackendActionResult.Error;
        }
    }

    public async Task<BackendActionResult> UpdateRoleAsync(string username, RoleType newRole)
    {
        try
        {
            return await ExecuteUpdateAsync(
                customer => customer.Username == username,
                calls => calls.SetProperty(customer => customer.Role, newRole));
        }
        catch (Exception e)
        {
            Debug.WriteLine(e);

            return BackendActionResult.Error;
        }
    }

    public async Task<BackendActionResult> DeleteAsync(string username)
    {
        try
        {
            return await ExecuteDeleteAsync(customer => customer.Username == username);
        }
        catch (Exception e)
        {
            Debug.WriteLine(e);

            return BackendActionResult.Error;
        }
    }

    public async Task<bool> ExistsAsync(string username)
    {
        return await context.Customers
            .AnyAsync(customer => customer.Username == username);
    }

    public async Task<bool> VerifyPasswordAsync(string username, string password)
    {
        var passwordHash = await GetPasswordHashAsync(username);

        return !string.IsNullOrWhiteSpace(passwordHash) &&
               hasher.Verify(password, passwordHash);
    }

    private async Task<BackendActionResult> ExecuteUpdateAsync(Expression<Func<Customer, bool>> where,
        Expression<Func<SetPropertyCalls<Customer>, SetPropertyCalls<Customer>>> calls)
    {
        var updated = await context.Customers
            .Where(where)
            .ExecuteUpdateAsync(calls);

        return updated > 0 ? BackendActionResult.Success : BackendActionResult.NotFound;
    }

    private async Task<BackendActionResult> ExecuteDeleteAsync(Expression<Func<Customer, bool>> where)
    {
        var deleted = await context.Customers
            .Where(where)
            .ExecuteDeleteAsync();

        return deleted > 0 ? BackendActionResult.Success : BackendActionResult.NotFound;
    }
}