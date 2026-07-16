using TradingEngineServer.Orders;

namespace TradingEngineServer.MatchingEngine;

public class MatchResult(
    List<Fill> fills
)
{
    public List<Fill> Fills { get; } = new();
    public bool IsMatch => Fills.Any(); // returns true if fills is NOT empty
}