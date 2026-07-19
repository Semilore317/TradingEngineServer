using Instruments;
using Valkyrie.Api.Dto;
using Valkyrie.MatchingEngine;
using Valkyrie.MatchingEngine.Algorithms;
using Valkyrie.OrderBook;
using Valkyrie.Orders;

namespace Valkyrie.Api;

public sealed class OrderGateway(IMatchingEngine engine)
{
    private readonly object _gate = new();
    private long _nextOrderId;

    public OrderAck Submit(PlaceOrderRequest r)
    {
        lock (_gate)                                // one writer at a time
        {
            var id = ++_nextOrderId;           // server assigns the id
            var order = new Order(id, r.SecurityId, r.Username, r.Side, r.Price, r.Quantity);
            var result = engine.AddOrder(order);
            return OrderAck.From(id, result);
        }
    }

    public void Cancel(long id, long securityId, string username)
    {
        lock (_gate)
            engine.RemoveOrder(new CancelOrder(id, securityId, username));
    }

    public bool TryGetBook(long securityId, out OrderBookSnapshot? book)
    {
        lock (_gate)
            return engine.TryGetSnapshot(securityId, out book);
    }
}