using TradingEngineServer.Orders;

namespace TradingEngineServer.OrderBook;
// <summary>
// Combines retrieval (read) and order entry (write) permissions.
// This interface represents the full access required by the matching engine to process trades.
// </summary>
public interface IRetrievalOrderBook: IReadonlyOrderBook
{
    List<OrderbookEntry> GetAskOrders();
    List<OrderbookEntry> GetBidOrders();
}