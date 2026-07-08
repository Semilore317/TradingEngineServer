using Instruments;
using TradingEngineServer.Orders;

namespace TradingEngineServer.OrderBook;

public class OrderBook : IRetrievalOrderBook
{
    private readonly Security _instrument;
    
    // orders keyed by order-id
    private readonly Dictionary<long, OrderbookEntry> _orders = new Dictionary<long, OrderbookEntry>();
    
    // helps to sort in nlogn time
    private readonly SortedSet<Limit> _askLimits = new SortedSet<Limit>(AskLimitComparer.Comparer);
    private readonly SortedSet<Limit> _bidLimits = new SortedSet<Limit>(AskLimitComparer.Comparer);
    
    public OrderBook(Security instrument)
    {
        _instrument = instrument;
    }
    
    public int Count => _orders.Count;

    public bool ContainsOrder(long orderId)
    {
        return _orders.ContainsKey(orderId);
    }

    public OrderBookSpread GetSpread()
    {
        throw new NotImplementedException();
    }

    public int OrderCount { get; }

    public List<OrderbookEntry> GetAskOrders()
    {
        throw new NotImplementedException();
    }

    public List<OrderbookEntry> GetBidOrders()
    {
        throw new NotImplementedException();
    }

    public void AddOrder(Order order)
    {
        throw new NotImplementedException();
    }

    public void ChangeOrder(ModifyOrder modifyOrder)
    {
        throw new NotImplementedException();
    }

    public void RemoveOrder(CancelOrder cancelOrder)
    {
        throw new NotImplementedException();
    }
}