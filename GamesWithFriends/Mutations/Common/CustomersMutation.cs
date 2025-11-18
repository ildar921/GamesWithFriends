using GamesWithFriends.Core.Enums;
using GamesWithFriends.Infrastructure.Repositories;
using GamesWithFriends.Mutations.Inputs;
using GamesWithFriends.Mutations.Payloads;
using HotChocolate.Resolvers;

namespace GamesWithFriends.Mutations.Common;

[MutationType]
public static class CustomersMutation
{
    public static async Task<RegisterCustomerPayload> RegisterCustomerAsync(RegisterCustomerInput input,
        IResolverContext context,
        [Service] ICustomersRepository repo)
    {
        var (customer, result) = await repo.AddAsync(input.Username, input.Password);

        if (result == BackendActionResult.AlreadyExists)
            context.ReportError(ErrorBuilder
                .New()
                .SetCode("ALREADY_EXISTS")
                .SetMessage("Already Exists")
                .Build());
        else if (result != BackendActionResult.Success)
            context.ReportError(ErrorBuilder
                .New()
                .SetCode("ERROR")
                .SetMessage("Unknown error")
                .Build());

        return new RegisterCustomerPayload(customer);
    }
}