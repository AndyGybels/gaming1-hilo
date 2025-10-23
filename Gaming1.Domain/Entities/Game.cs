using Gaming1.Domain.Exceptions;

namespace Gaming1.Domain.Entities;

public class Game
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public int Min { get; private set; }
    public int Max { get; private set; }
    public int Secret { get; private set; }
    public bool IsOver { get; private set; }
    public string? Winner { get; private set; }
    public int Attempts { get; private set; }

    private Game() { } // EF Core

    public Game(int min, int max)
    {
        Min = min;
        Max = max;
        Secret = Random.Shared.Next(min, max + 1);
    }

    public string Guess(string player, int number)
    {
        if (IsOver)
            throw new GameAlreadyFinishedException();

        Attempts++;

        if (number == Secret)
        {
            IsOver = true;
            Winner = player;
            return $"🎉 {player} guessed correctly!";
        }

        return number < Secret ? "HI" : "LO";
    }
}
