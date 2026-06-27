using System;
using System.Collections.Generic;
using System.Text;

namespace TradingEngineServer.Core.Configuration
{
    class TradingEngineServerConfiguration
    {
    public TradingEngineServerSettings TradingEngineServerSettings { get; set; }
    }

    class TradingServerSettings
    {
        public string ServerName { get; set; }
        public int Port { get; set; }
        public string Host { get; set; }
    }
}
