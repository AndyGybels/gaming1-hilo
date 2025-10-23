using System.Collections.Concurrent;
using System.Threading.Channels;
using HiLoGame.Grpc;

namespace Gaming1.Web.Services;

public class GameUpdatePublisher
{
    private readonly ConcurrentDictionary<Guid, List<ChannelWriter<GameUpdate>>> _subs = new();

    public ChannelReader<GameUpdate> Subscribe(Guid gameId, CancellationToken cancellationToken)
    {
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
            RemoveWriter(gameId, channel.Writer);
            channel.Writer.TryComplete();
        });

        return channel.Reader;
    }

    public void Publish(Guid gameId, GameUpdate update)
    {
        if (!_subs.TryGetValue(gameId, out var writers)) return;

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
                    RemoveWriter(gameId, w);
                }
            }
            catch
            {
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
            if (writers.Count == 0)
            {
                _subs.TryRemove(gameId, out _);
            }
        }
    }
}
