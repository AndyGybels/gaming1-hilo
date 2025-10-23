using System;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using HiLoGame.Grpc;

Console.WriteLine("HiLo gRPC interactive client");

using var channel = GrpcChannel.ForAddress("http://localhost:5277");
var client = new HiLo.HiLoClient(channel);

string gameId;
Console.WriteLine("Options: (s)tart a new game, (j)oin an existing game");
var opt = Console.ReadLine()?.Trim().ToLowerInvariant();
if (opt == "s" || opt == "start")
{
    Console.Write("Min: ");
    var minText = Console.ReadLine();
    Console.Write("Max: ");
    var maxText = Console.ReadLine();
    if (!int.TryParse(minText, out var min)) min = 1;
    if (!int.TryParse(maxText, out var max)) max = 10;

    var start = await client.StartGameAsync(new StartGameRequest { Min = min, Max = max });
    gameId = start.GameId;
    Console.WriteLine($"Started game {gameId} ({min}-{max})");
}
else
{
    Console.Write("Enter game id to join: ");
    gameId = Console.ReadLine() ?? string.Empty;
}

if (string.IsNullOrWhiteSpace(gameId))
{
    Console.WriteLine("No game id provided. Exiting.");
    return;
}
Console.Write("Enter your player name (default: ConsolePlayer): ");
var playerName = Console.ReadLine();
if (string.IsNullOrWhiteSpace(playerName)) playerName = "ConsolePlayer";

using var cts = new CancellationTokenSource();
var streamingCall = client.SubscribeToGame(new SubscribeRequest { GameId = gameId });

var gameOver = false;

var readTask = Task.Run(async () =>
{
    try
    {
        var responseStream = streamingCall.ResponseStream;
        while (await responseStream.MoveNext(cts.Token))
        {
            var update = responseStream.Current;
            Console.WriteLine($"[Update] {update.Type} - {update.Message} player={update.Player} attempts={update.Attempts} isOver={update.IsOver} winner={update.Winner}");
            if (update.IsOver)
            {
                gameOver = true;
                try { cts.Cancel(); } catch { }
            }
        }
    }
    catch (OperationCanceledException) { }
    catch (Exception ex)
    {
        Console.WriteLine($"Stream error: {ex.Message}");
    }
});

Console.WriteLine("Type a number and press Enter to guess. Type 'exit' to quit.");
while (!gameOver)
{
    Console.Write("> ");
    var line = Console.ReadLine();
    if (line == null) break;
    line = line.Trim();
    if (string.Equals(line, "exit", StringComparison.OrdinalIgnoreCase)) break;
    if (!int.TryParse(line, out var guessNumber))
    {
        Console.WriteLine("Please enter a valid number or 'exit'.");
        continue;
    }

    try
    {
        var reply = await client.GuessAsync(new GuessRequest { GameId = gameId, Player = playerName, Number = guessNumber });
        Console.WriteLine($"Server: {reply.Result} (isOver={reply.IsOver} winner={reply.Winner} attempts={reply.Attempts})");
        if (reply.IsOver)
        {
            gameOver = true;
            try { cts.Cancel(); } catch { }
            break;
        }
    }
    catch (RpcException rpcEx)
    {
        Console.WriteLine($"RPC error: {rpcEx.Status}");
    }
}

// clean up
try { cts.Cancel(); } catch { }
await readTask;
Console.WriteLine("Client exiting.");
