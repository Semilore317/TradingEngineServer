using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using TradingEngineServer.Core.Configuration;

namespace TradingEngineServer.Core
{
    sealed class TradingEngineServer : BackgroundService, ITradingEngineServer
    {
        private readonly ILogger<TradingEngineServer> _logger;
        private readonly TradingEngineServerConfiguration _tradingEngineServerconfig;
        public TradingEngineServer(
            ILogger<TradingEngineServer> logger, 
            IOptions<TradingEngineServerConfiguration> config
            )
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _tradingEngineServerconfig = config.Value ?? throw new ArgumentNullException(nameof(config)); 
       }

        public Task Run(CancellationToken token)
        {
            return ExecuteAsync(token);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
             _logger.LogInformation($"Trading Engine Server {nameof(TradingEngineServer)} started");
            // the server technically doesn't need a loop, but it is here to keep the service running
            while (!stoppingToken.IsCancellationRequested)
            {
            }
            _logger.LogInformation($"Trading Engine Server {nameof(TradingEngineServer)} stopped");

            return Task.CompletedTask;
        }
    }
}
