using System;
using System.Collections.Generic;
using System.Text;

namespace TradingEngineServer.Core
{
    interface ITradingEngineServer
    {
        Task Run(CancellationToken token);
    }
}
