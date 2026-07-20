using System.Text.Json.Serialization;
using Instruments;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Valkyrie.Api;
using Valkyrie.Core.Configuration;
using Valkyrie.Instrument.Configuration;
using Valkyrie.Logging;
using Valkyrie.Logging.Configuration;
using Valkyrie.MatchingEngine;
using Valkyrie.MatchingEngine.Algorithms;
using Valkyrie.MatchingEngine.Configuration;
using static System.AppContext;


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
    // this makes sure that i don't have to add actual enum values like side -> 2... much more intuitive from the json
    builder.Services.ConfigureHttpJsonOptions(
        o => o.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));
    
    var app = builder.Build();
    app.MapOrderEndpoints();

    var engine = app.Services.GetRequiredService<IMatchingEngine>();

    var instruments = new List<InstrumentConfiguration>();
    app.Configuration.GetSection(nameof(Instruments)).Bind(instruments);

    foreach (var instrument in instruments)
    {
        engine.AddOrderBook(new Security(instrument.SecurityId, instrument.Symbol));
    }
    
    await app.RunAsync();
}


public partial class Program; // since WebApplicationFactory requires it