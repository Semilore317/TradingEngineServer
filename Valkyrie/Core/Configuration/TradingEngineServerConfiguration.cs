using System;
using System.Collections.Generic;
using System.Text;

namespace Valkyrie.Core.Configuration
{
    class TradingEngineServerConfiguration
    {
    public TradingEngineServerSettings TradingEngineServerSettings { get; set; }
    }

    class TradingServerSettings
    {
        public required string ServerName { get; set; }
        public int Port { get; set; }
        public required string Host { get; set; }
    }
}
