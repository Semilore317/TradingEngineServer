using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Valkyrie.Api;
using Valkyrie.Core;
using Valkyrie.Core.Configuration;
using Valkyrie.Logging;
using Valkyrie.Logging.Configuration;
using Valkyrie.MatchingEngine;
using Valkyrie.MatchingEngine.Algorithms;
using Valkyrie.MatchingEngine.Configuration;
using static System.AppContext;

using var engine = ValkyrieHostBuilder.BuildValkyrie();

ValkyrieServiceProvider.ServiceProvider = engine.Services;

{
    var builder = WebApplication.CreateBuilder(
        new WebApplicationOptions
        {
            Args = args,
            ContentRootPath = BaseDirectory, // read appsettings.json from the bin output, like the old host
        });

    builder.Configuration
        .SetBasePath(AppContext.BaseDirectory)
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
    
    builder.Services.AddOptions();
    builder.Services.Configure<ValkyrieConfiguration>(
        builder.Configuration.GetSection(nameof(ValkyrieConfiguration)));
    builder.Services.Configure<LoggingConfiguration>(
        builder.Configuration.GetSection(nameof(LoggingConfiguration)));
    builder.Services.Configure<MatchingEngineConfiguration>(
        builder.Configuration.GetSection(nameof(MatchingEngineConfiguration)));

    builder.Services.AddSingleton<ITextLogger, TextLogger>();
    builder.Services.AddSingleton<IMatchingAlgorithm>(sp =>
    {
        var config = sp.GetRequiredService<IOptions<MatchingEngineConfiguration>>().Value;
        return config.Algorithm switch
        {
            MatchingAlgorithmType.Fifo => Fifo.Instance,
            MatchingAlgorithmType.ProRata => ProRata.Instance,
            _ => throw new InvalidOperationException($"Algorithm '{config.Algorithm}' is not supported.")
        };
    });

    builder.Services.AddSingleton<IMatchingEngine, MatchingEngine>();
    builder.Services.AddSingleton<OrderGateway>();
    builder.Services.AddHostedService<Valkyrie.Core.Valkyrie>(); // the background service... it still runs

    
    var app = builder.Build();
    app.MapOrderEndpoints();
    await app.RunAsync();
}