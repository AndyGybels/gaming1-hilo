using System.Collections.Concurrent;
using System.Threading.Channels;
using HiLoGame.Grpc;

namespace Gaming1.Web.Services;

public class GameUpdatePublisher
{
    private readonly ConcurrentDictionary<Guid, List<ChannelWriter<GameUpdate>>> _subs = new();
    private readonly ILogger<GameUpdatePublisher> _logger;

    public GameUpdatePublisher(ILogger<GameUpdatePublisher> logger)
    {
        _logger = logger;
    }

    public ChannelReader<GameUpdate> Subscribe(Guid gameId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Subscribe called for game {GameId}", gameId);
        var channel = Channel.CreateUnbounded<GameUpdate>(new UnboundedChannelOptions
        {
            SingleReader = true,
            SingleWriter = false
        });

        var writers = _subs.GetOrAdd(gameId, _ => new List<ChannelWriter<GameUpdate>>());
        lock (writers)
        {
            writers.Add(channel.Writer);
        }

        cancellationToken.Register(() =>
        {
            _logger.LogInformation("Subscription for game {GameId} cancelled", gameId);
            RemoveWriter(gameId, channel.Writer);
            channel.Writer.TryComplete();
        });

        return channel.Reader;
    }

    public void Publish(Guid gameId, GameUpdate update)
    {
        _logger.LogDebug("Publishing update for game {GameId}: {Update}", gameId, update);
        if (!_subs.TryGetValue(gameId, out var writers))
        {
            _logger.LogDebug("No subscribers for game {GameId}", gameId);
            return;
        }

        List<ChannelWriter<GameUpdate>> snapshot;
        lock (writers)
        {
            snapshot = writers.ToList();
        }

        foreach (var w in snapshot)
        {
            try
            {
                if (!w.TryWrite(update))
                {
                    _logger.LogDebug("Writer for game {GameId} rejected write, removing", gameId);
                    RemoveWriter(gameId, w);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Exception while writing update for game {GameId}, removing writer", gameId);
                RemoveWriter(gameId, w);
            }
        }
    }

    private void RemoveWriter(Guid gameId, ChannelWriter<GameUpdate> writer)
    {
        if (!_subs.TryGetValue(gameId, out var writers)) return;

        lock (writers)
        {
            writers.Remove(writer);
            _logger.LogInformation("Removed a subscriber for game {GameId}, remaining={Count}", gameId, writers.Count);
            if (writers.Count == 0)
            {
                _subs.TryRemove(gameId, out _);
                _logger.LogInformation("No more subscribers for game {GameId}, removed entry", gameId);
            }
        }
    }
}
