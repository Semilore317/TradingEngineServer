using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;

namespace TradingEngineServer.Core
{
    public sealed class TradingEngineServerHostBuilder
    {
        public static IHost BuildTradingEngineServer()
            => Host.CreateDefaultBuilder().ConfigureServices((context, services)
            =>{
                services.AddOptions();
                services.Configure(context.Configuration.GetSection(nameof("TradingEngineServerConfiguration")));
            }).Build();
    }
}
