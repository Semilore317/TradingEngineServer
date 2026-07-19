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

    public OrderAck Submit(PlaceOrderRequest request)
    {
        lock (_gate)                                // one writer at a time
        {
            // server assigns the id for now.... since it's locked it's not a major concern for now
            var id = ++_nextOrderId;          
            var order = new Order(id, request.SecurityId, request.Username, request.Side, request.Price, request.Quantity);
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

    public OrderAck Modify(ModifyOrderRequest request)
    {
        lock (_gate)
        {
            var id = ++_nextOrderId;
            var modifyOrder = new ModifyOrder(id, request.SecurityId, request.Username, request.Side, request.Price,
                request.Quantity);
            var result = engine.ChangeOrders(modifyOrder);
            return OrderAck.From(id, result);
        }
    }
}