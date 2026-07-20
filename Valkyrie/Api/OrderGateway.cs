using Valkyrie.Api.Dto;
using Valkyrie.Api.MarketData;
using Valkyrie.MatchingEngine;
using Valkyrie.Orders;

namespace Valkyrie.Api;

/// <summary>
///  provides a thread-safe entry point for order operations routing to the matching engine
///  It uses a private lock object pattern to guarantee a mutex across all state mutations and reads
/// </summary>
public sealed class OrderGateway(IMatchingEngine engine, IMarketDataPublisher publisher)
{
    private readonly object _gate = new();
    private long _nextOrderId;

    public OrderAck Submit(PlaceOrderRequest request)
    {

        MatchResult result;
        long id;
        OrderBookSnapshot? snapshot;
        
        // syncs access to the sequential ID generation and the underlying non-thread-safe matching engine instance
        // this prevents lock contention from external code
        lock (_gate)                          
        {
            // server assigns the id for now.... since it's locked it's not a major concern for now
            id = ++_nextOrderId;          
            result = engine.AddOrder(
                new Order(
                    id, request.SecurityId, request.Username, request.Side, request.Price, request.Quantity));
            engine.TryGetSnapshot(request.SecurityId, out snapshot);
        }

        foreach (var fill in result.Fills)
            publisher.PublishTrade(TradeEvent.From(fill));
        
        if(snapshot != null)
            publisher.PublishBook(snapshot);


        return OrderAck.From(id, result);
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
            var modifyOrder = new ModifyOrder(request.OrderId, request.SecurityId, request.Username, request.Side, request.Price,
                request.Quantity);
            var result = engine.ChangeOrders(modifyOrder);
            return OrderAck.From(request.OrderId, result);
        }
    }

    public OrderAck Modify(long id, long securityId, ModifyOrderRequest request)
    {
        lock (_gate)
        {
            var modifyOrder = new ModifyOrder(
                id, securityId, request.Username, request.Side,  request.Price, request.Quantity);
            var result = engine.ChangeOrders(modifyOrder);
            return OrderAck.From(id, result);
        }
    }
}