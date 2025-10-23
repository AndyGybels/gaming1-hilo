using Gaming1.Application.Commands;
using Grpc.Core;
using HiLoGame.Grpc;

namespace Gaming1.Web.Services;

public class HiLoService : HiLo.HiLoBase
{
    private readonly ILogger<HiLoService> _logger;
    private readonly StartGameHandler _startHandler;
    private readonly MakeGuessHandler _guessHandler;
    private readonly GameUpdatePublisher _publisher;

    public HiLoService(ILogger<HiLoService> logger, StartGameHandler startHandler, MakeGuessHandler guessHandler, GameUpdatePublisher publisher)
    {
        _logger = logger;
        _startHandler = startHandler;
        _guessHandler = guessHandler;
        _publisher = publisher;
    }

    public override async Task<GuessReply> Guess(GuessRequest request, ServerCallContext context)
    {
        var (result, isOver, winner, attempts) =
            await _guessHandler.Handle(new MakeGuessCommand(Guid.Parse(request.GameId), request.Player, request.Number));

        _logger.LogInformation("Player {Player} guessed {Number} on game {GameId}: {Result}", request.Player, request.Number, request.GameId, result);

        // publish update to subscribers
        _publisher.Publish(Guid.Parse(request.GameId), new GameUpdate
        {
            Type = UpdateType.Guess,
            Message = result,
            Player = request.Player,
            Attempts = attempts,
            IsOver = isOver,
            Winner = winner ?? string.Empty
        });

        return new GuessReply
        {
            Result = result,
            IsOver = isOver,
            Winner = winner ?? string.Empty,
            Attempts = attempts
        };
    }

    public override async Task<StartGameReply> StartGame(StartGameRequest request, ServerCallContext context)
    {
        var game = await _startHandler.Handle(new StartGameCommand(request.Min, request.Max));

        _logger.LogInformation("Starting game {GameId} with range [{Min},{Max}]", game.Id, game.Min, game.Max);

        _publisher.Publish(game.Id, new GameUpdate
        {
            Type = UpdateType.Start,
            Message = $"Game created between [{game.Min}, {game.Max}]",
            Player = string.Empty,
            Attempts = game.Attempts,
            IsOver = game.IsOver,
            Winner = game.Winner ?? string.Empty
        });

        return new StartGameReply
        {
            GameId = game.Id.ToString(),
            Message = $"Game created between [{game.Min}, {game.Max}]"
        };
    }

    public override async Task SubscribeToGame(SubscribeRequest request, IServerStreamWriter<GameUpdate> responseStream, ServerCallContext context)
    {
        if (!Guid.TryParse(request.GameId, out var gameId))
        {
            context.Status = new Status(StatusCode.InvalidArgument, "Invalid game id");
            return;
        }

        _logger.LogInformation("Client {Peer} subscribed to game {GameId}", context.Peer, gameId);

        var reader = _publisher.Subscribe(gameId, context.CancellationToken);

        try
        {
            await foreach (var update in reader.ReadAllAsync(context.CancellationToken))
            {
                await responseStream.WriteAsync(update);
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Subscription for client {Peer} to game {GameId} cancelled", context.Peer, gameId);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error while streaming updates to client {Peer} for game {GameId}", context.Peer, gameId);
            throw;
        }
    }
}