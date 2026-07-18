using Instruments;
using TradingEngineServer.MatchingEngine.Algorithms;
using TradingEngineServer.Orders;

using EngineOrderBook = TradingEngineServer.OrderBook.OrderBook;
namespace TradingEngineServer.MatchingEngine;
public class MatchingEngine: IMatchingEngine
{
    private readonly IMatchingAlgorithm _algorithm;
    private readonly Dictionary<long, EngineOrderBook> _orderBooks; // SecurityId → book

    public MatchingEngine(IMatchingAlgorithm algorithm)
    {
        _algorithm = algorithm;
        _orderBooks = new Dictionary<long, EngineOrderBook>();
    }

    public void AddOrderBook(Security instrument)
    {
        if(_orderBooks.ContainsKey(instrument.SecurityId))
            throw new InvalidOperationException($"OrderBook already registered for SecurityId {instrument.SecurityId}");
        _orderBooks.Add(instrument.SecurityId, new EngineOrderBook(instrument));
    }
    
    public MatchResult AddOrder(Order order)
    {
        long id = order.SecurityId;
        if (!_orderBooks.TryGetValue(id, out EngineOrderBook? book))
            throw new InvalidOperationException($"No orderbook registered for SecurityId {order.SecurityId}");
       
        MatchResult result = _algorithm.MatchIncoming(order, book.BidLimits, book.AskLimits, book.Orders);
        
        // .. then rest whatever's left as a passive order
        if(order.CurrentQuantity > 0)
            book.AddOrder(order);

        return result;
    }

    public MatchResult ChangeOrders(ModifyOrder modifyOrder)
    {
        long id =  modifyOrder.SecurityId;
        if (!_orderBooks.TryGetValue(id, out EngineOrderBook? book))
            throw new InvalidOperationException($"No orderbook registered for SecurityId {id}");
        
        // cancel the old resting order and treat the change as a fresh incomint order
        book.RemoveOrder(modifyOrder.ToCancelOrder());
        Order incoming = modifyOrder.ToNewOrder();
        
        MatchResult result = _algorithm.MatchIncoming(incoming, book.BidLimits, book.AskLimits, book.Orders);
        
        if(incoming.CurrentQuantity > 0)
            book.AddOrder(incoming);

        return result;
    }

    public void RemoveOrder(CancelOrder cancelOrder)
    {
        long id = cancelOrder.SecurityId;
        if (!_orderBooks.TryGetValue(id, out EngineOrderBook? book))
            throw new InvalidOperationException($"No orderbook registered for SecurityId {id}");
        
        book.RemoveOrder(cancelOrder);
    }
}
