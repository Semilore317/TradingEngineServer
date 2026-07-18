// entry point

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Valkyrie.Core;

using var engine = TradingEngineServerHostBuilder.BuildTradingEngineServer();

TradingEngineServerServiceProvider.ServiceProvider = engine.Services;

{
    using var scope = TradingEngineServerServiceProvider.ServiceProvider.CreateScope();
    
    await engine.RunAsync().ConfigureAwait(false);
}