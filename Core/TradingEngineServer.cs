using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using TradingEngineServer.Core.Configuration;
using TradingEngineServer.Logging;

namespace TradingEngineServer.Core
{
    sealed class TradingEngineServer : BackgroundService, ITradingEngineServer
    {
        private readonly ITextLogger _logger;
        private readonly TradingEngineServerConfiguration _tradingEngineServerconfig;
        public TradingEngineServer(
            ITextLogger textLogger, 
            IOptions<TradingEngineServerConfiguration> config
            )
        {
            _logger = textLogger ?? throw new ArgumentNullException(nameof(textLogger));
            _tradingEngineServerconfig = config.Value ?? throw new ArgumentNullException(nameof(config)); 
       }

        public Task Run(CancellationToken token)
        {
            return ExecuteAsync(token);
            // the cancellation token makes it so that we can cancel the "run loop"
            // kinda similar to the way games run on a loop or tk runs on a mainLoop
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
             _logger.Info("TradingEngineServer.Core", "Trading Engine Server Started");
            // the server technically doesn't need a loop, but it is here to keep the service running
            while (!stoppingToken.IsCancellationRequested)
            {
                CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
                cancellationTokenSource.Cancel();
                cancellationTokenSource.Dispose();
                //cancellationTokenSource.Token --> this is what's being cancelled by the cancellation via Ctrl + C
            }
            _logger.Info("TradingEngineServer.Core", "Trading Engine Server Stopped");
            return Task.CompletedTask;
        }
    }
}
