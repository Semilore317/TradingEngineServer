using System;
using System.Collections.Generic;
using System.Text;

namespace TradingEngineServer.Core
{
    public static class TradingEngineServerServiceProvider
    {
        // static classes will be created at either the start of the application or when the class is first used
        public static IServiceProvider ServiceProvider { get; set; }
    }
}
