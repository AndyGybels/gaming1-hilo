using Gaming1.Domain.Entities;
using Xunit;

namespace Gaming1.UnitTests;

public class GameTests
{
    [Fact]
    public void Guess_WhenCorrect_SetsWinnerAndIsOver()
    {
        var game = new Game(1, 1); // secret will be 1

        var result = game.Guess("Alice", 1);

        Assert.Equal("🎉 Alice guessed correctly!", result);
        Assert.True(game.IsOver);
        Assert.Equal("Alice", game.Winner);
        Assert.Equal(1, game.Attempts);
    }

    [Fact]
    public void Guess_WhenAlreadyOver_Throws()
    {
        var game = new Game(1, 1);
        game.Guess("Alice", 1);

        Assert.Throws<Gaming1.Domain.Exceptions.GameAlreadyFinishedException>(() => game.Guess("Bob", 1));
    }
}
