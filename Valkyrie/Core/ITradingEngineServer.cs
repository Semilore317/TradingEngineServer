using System;
using System.Collections.Generic;
using System.Text;

namespace Valkyrie.Core
{
    interface ITradingEngineServer
    {
        Task Run(CancellationToken token);
    }
}
