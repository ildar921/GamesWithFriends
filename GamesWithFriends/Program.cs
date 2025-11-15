using GamesWithFriends.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Configure();

var app = builder.Build();

await app.RunAsync();