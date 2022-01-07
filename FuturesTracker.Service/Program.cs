global using FuturesTracker.Service;
using FuturesTracker.Service.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddSingleton<IDeribitClient, DeribitClient>()
    .AddSingleton<IKeyValueStore, RocksDbStore>()
    .AddSingleton<ITickerTracker, TickerTracker>();

var app = builder.Build();

app.UseFileServer();

app.MapGet("api/ticker/{ticker}",
    async (string ticker, ITickerTracker tracker) => await tracker.GetTickerDataAsync(ticker) );

app.Run();
