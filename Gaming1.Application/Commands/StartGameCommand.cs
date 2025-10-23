using Gaming1.Application.Interfaces;
using Gaming1.Domain.Entities;

namespace Gaming1.Application.Commands;

public record StartGameCommand(int Min, int Max);

public class StartGameCommandHandler : ICommandHandler<Game, StartGameCommand>
{
    private readonly  IGameRepository _repo;

    public StartGameCommandHandler(IGameRepository repo)
    {
        _repo = repo;
    }

    public async Task<Game> Handle(StartGameCommand cmd, CancellationToken cancellationToken)
    {
        var game = new Game(cmd.Min, cmd.Max);
        await _repo.AddAsync(game, cancellationToken);
        return game;
    }
}