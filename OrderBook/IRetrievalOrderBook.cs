using TradingEngineServer.Orders;

namespace TradingEngineServer.OrderBook;
// <summary>
// Retrieval interface of the orderbook.
// Allows retrieving the full lists of active bid and ask orders.
// </summary>
public interface IRetrievalOrderBook: IReadonlyOrderBook
{
    List<OrderbookEntry> GetAskOrders();
    List<OrderbookEntry> GetBidOrders();
}