# HiLo Game (Gaming1)

This repository contains a simple gRPC-based HiLo game server and a console client.

Projects of interest
- `Gaming1.Web` - gRPC server (uses an in-memory EF Core database for development).
- `Clients/HiLo.Client` - Console-based gRPC client that can start or join games.

Prerequisites
- .NET SDK 9.0 or later (the web project targets `net9.0`). You can verify with:

```bash
dotnet --version
```

Running the server

1. Open a terminal and change to the web project directory:

```bash
cd Gaming1.Web
```

2. Run the server with the default launch profile (Development). The server listens on http://localhost:5277 and https://localhost:7024 by default (see `Properties/launchSettings.json`):

```bash
dotnet run
```

You should see output indicating Kestrel started and gRPC endpoints are mapped. The server hosts the gRPC service `HiLoService`.

Running the console client

1. Open a new terminal and change to the client directory:

```bash
cd Clients/HiLo.Client
```

2. Run the client. The client expects the server to be available at http://localhost:5277 (the client uses an insecure HTTP channel):

```bash
dotnet run
```

3. Follow the interactive prompts in the console client to start or join a game and make guesses.

Notes and troubleshooting
- If the server was started with HTTPS only and the client uses the HTTP URL, the client will fail to connect. The server's `Properties/launchSettings.json` includes an HTTP URL (http://localhost:5277) which the client targets by default.
- If you change the server port, update the client `GrpcChannel.ForAddress("http://localhost:5277")` in `Clients/HiLo.Client/Program.cs` or start the client with environment variables to point to the correct address.
- The server uses an in-memory database, so games are not persisted between restarts.
- If you see issues with SSL or certificate trust when using HTTPS locally, run the server on the HTTP endpoint or configure local dev certificates using `dotnet dev-certs https --trust`.

Development tips
- To run both server and client concurrently in separate terminals, start the server first, then run the client.
- To run from an IDE (Visual Studio / Rider), set the startup projects appropriately: `Gaming1.Web` (server) and `Clients/HiLo.Client` (client) as needed.

License / Attribution
- This repo is for demo purposes.
