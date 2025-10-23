using Gaming1.Web.Services;
using Gaming1.Infrastructure.Persistence;
using Gaming1.Infrastructure.Repositories;
using Gaming1.Application.Commands;
using Gaming1.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add gRPC
builder.Services.AddGrpc();

// Infrastructure & application DI - keeping layers separated per Clean Architecture
// Use an in-memory database for local/dev. Swap out with a real provider in production.
builder.Services.AddDbContext<GameDbContext>(opts => opts.UseInMemoryDatabase("HiLoGames"));
builder.Services.AddScoped<IGameRepository, GameRepository>();

// Register handlers from Application layer
builder.Services.AddScoped<StartGameHandler>();
builder.Services.AddScoped<MakeGuessHandler>();

// Publisher for streaming updates
builder.Services.AddSingleton<GameUpdatePublisher>();

var app = builder.Build();

app.MapGrpcService<HiLoService>();
app.MapGet("/",
    () =>
        "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();