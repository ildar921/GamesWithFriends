using GamesWithFriends.Core.Entities;
using GamesWithFriends.Core.Enums;

namespace GamesWithFriends.Infrastructure.Repositories;

public interface ICustomersRepository
{
    public Task<IReadOnlyList<Customer>?> GetAllAsync();
    public Task<Customer?> GetAsync(string username);

    public Task<string?> GetUsernameAsync(string username);
    public Task<string?> GetPasswordHashAsync(string username);
    public Task<decimal?> GetBalanceAsync(string username);
    public Task<RoleType?> GetRoleAsync(string username);

    public Task<(Customer?, BackendActionResult)> AddAsync(Customer customer);
    public Task<(Customer?, BackendActionResult)> AddAsync(string username, string password);

    public Task<BackendActionResult> UpdateUsernameAsync(string username, string newUsername);
    public Task<BackendActionResult> UpdatePasswordAsync(string username, string newPassword);
    public Task<BackendActionResult> UpdateBalanceAsync(string username, decimal newBalance);
    public Task<BackendActionResult> UpdateRoleAsync(string username, RoleType newRole);

    public Task<BackendActionResult> DeleteAsync(string username);

    public Task<bool> ExistsAsync(string username);
    public Task<bool> VerifyPasswordAsync(string username, string password);
}