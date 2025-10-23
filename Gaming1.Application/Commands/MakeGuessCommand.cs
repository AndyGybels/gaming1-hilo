using Gaming1.Application.Interfaces;

namespace Gaming1.Application.Commands;

public record MakeGuessCommand(Guid GameId, string Player, int Number);
public record MakeGuessResult(string Result, bool IsOver, string? Winner, int Attempts);

public class MakeGuessCommandHandler : ICommandHandler<MakeGuessResult, MakeGuessCommand>
{
    private readonly IGameRepository _repo;

    public MakeGuessCommandHandler(IGameRepository repo)
    {
        _repo = repo;
    }

    public async Task<MakeGuessResult> Handle(MakeGuessCommand cmd, CancellationToken cancellationToken)
    {
        var game = await _repo.GetAsync(cmd.GameId, cancellationToken)
                   ?? throw new KeyNotFoundException("Game not found");

        var result = game.Guess(cmd.Player, cmd.Number);
        await _repo.UpdateAsync(game, cancellationToken);

        return new MakeGuessResult(result, game.IsOver, game.Winner, game.Attempts);
    }
}