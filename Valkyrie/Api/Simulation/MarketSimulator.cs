using Valkyrie.Logging;

namespace Valkyrie.Api.Simulation;

/// <summary>
/// implements a stochastic* market simulation using a random walk for price movements
/// and a poisson arrival process(Jitter) for "realistic" order delays
/// </summary>
public class MarketSimulator(
    IMarketDataSource source,
    ITextLogger logger
) : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken token)
    {
        logger.Info("MarketSimulatorRunner", $"Starting Data Source: {source.Name}");
        return source.RunAsync(token);
    }
}