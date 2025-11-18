using System.Security.Claims;
using GamesWithFriends.Core.Entities;
using GamesWithFriends.Core.Enums;
using GamesWithFriends.Infrastructure.Contexts;
using HotChocolate.Authorization;
using Microsoft.EntityFrameworkCore;
using ClaimTypes = GamesWithFriends.Core.Enums.ClaimTypes;

namespace GamesWithFriends.Queries.Common;

[QueryType]
public static class CustomersQuery
{
    [Authorize]
    [UsePaging]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public static IQueryable<Customer>? GetCustomers(ClaimsPrincipal principal, [Service] AppDbContext context)
    {
        if (principal.HasClaim(nameof(ClaimTypes.Role), nameof(RoleType.Admin)))
            return context.Customers;

        var username = principal.FindFirstValue(nameof(ClaimTypes.Username));

        if (string.IsNullOrWhiteSpace(username))
            return null;

        return context.Customers
            .AsNoTracking()
            .Where(customer => customer.Username == username);
    }
}