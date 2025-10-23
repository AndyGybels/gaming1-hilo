using Gaming1.Web.Services;
using Gaming1.Infrastructure.Persistence;
using Gaming1.Infrastructure.Repositories;
using Gaming1.Application.Commands;
using Gaming1.Application.Interfaces;
using Gaming1.Domain.Entities;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();

builder.Services.AddDbContext<GameDbContext>(opts => opts.UseInMemoryDatabase("HiLoGames"));
builder.Services.AddScoped<IGameRepository, GameRepository>();

builder.Services.AddScoped<ICommandHandler<Game, StartGameCommand>, StartGameCommandHandler>();
builder.Services.AddScoped<ICommandHandler<MakeGuessResult, MakeGuessCommand>, MakeGuessCommandHandler>();
builder.Services.AddScoped<IQueryHandler<ListGamesQuery, IEnumerable<Game>>, ListGamesHandler>();

builder.Services.AddSingleton<GameUpdatePublisher>();

var app = builder.Build();

app.MapGrpcService<HiLoService>();
app.MapGet("/",
    () =>
        "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();