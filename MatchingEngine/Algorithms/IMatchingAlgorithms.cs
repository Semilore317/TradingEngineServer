using TradingEngineServer.Orders;

namespace TradingEngineServer.MatchingEngine.Algorithms;

public interface IMatchingAlgorithms
{
    MatchResult Match(
        SortedSet<Limit> bidLimits, 
        SortedSet<Limit> askLimits,
        Dictionary<long, OrderbookEntry> orders);
}