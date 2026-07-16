using TradingEngineServer.Orders;

namespace TradingEngineServer.MatchingEngine.Algorithms;

public interface IMatchingAlgorithm
{
    MatchResult Match(
        SortedSet<Limit> bidLimits, 
        SortedSet<Limit> askLimits,
        Dictionary<long, OrderbookEntry> orders);
}