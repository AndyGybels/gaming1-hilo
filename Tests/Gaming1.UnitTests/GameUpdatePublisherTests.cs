using System;
using System.Threading;
using System.Threading.Tasks;
using HiLoGame.Grpc;
using Gaming1.Web.Services;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Gaming1.UnitTests;

public class GameUpdatePublisherTests
{
    [Fact]
    public async Task Publish_DeliversUpdateToSubscriber()
    {
        var publisher = new GameUpdatePublisher(new NullLogger<GameUpdatePublisher>());
        var gameId = Guid.NewGuid();

        using var cts = new CancellationTokenSource();
        var reader = publisher.Subscribe(gameId, cts.Token);

        var update = new GameUpdate { Type = UpdateType.Start, Message = "started" };
        publisher.Publish(gameId, update);

        var read = await reader.ReadAsync(cts.Token);
        Assert.Equal(UpdateType.Start, read.Type);
        Assert.Equal("started", read.Message);
    }
}
