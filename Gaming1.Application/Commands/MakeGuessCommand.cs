using Gaming1.Application.Interfaces;

namespace Gaming1.Application.Commands;

public record MakeGuessCommand(Guid GameId, string Player, int Number);

public class MakeGuessHandler
{
    private readonly IGameRepository _repo;

    public MakeGuessHandler(IGameRepository repo)
    {
        _repo = repo;
    }

    public async Task<(string Result, bool IsOver, string? Winner, int Attempts)> Handle(MakeGuessCommand cmd)
    {
        var game = await _repo.GetAsync(cmd.GameId)
                   ?? throw new KeyNotFoundException("Game not found");

        var result = game.Guess(cmd.Player, cmd.Number);
        await _repo.UpdateAsync(game);

        return (result, game.IsOver, game.Winner, game.Attempts);
    }
}