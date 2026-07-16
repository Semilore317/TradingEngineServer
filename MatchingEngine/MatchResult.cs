using TradingEngineServer.Orders;

namespace TradingEngineServer.MatchingEngine;

public class MatchResult
{
    public IReadOnlyList<Fill> Fills { get; }
    public bool IsMatch => Fills.Any(); // returns true if fills is NOT empty
}