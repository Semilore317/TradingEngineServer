using System.Collections.Concurrent;

namespace Valkyrie.Api.MarketData;

public class MarketDataHub
{
    // securityId ==> (connectionID ==> connection)
    private readonly ConcurrentDictionary<long, ConcurrentDictionary<string, MarketDataConnections>>
        _bySecurity = new();

    public void Subscribe(MarketDataConnections connection, long securityId)
    {
        _bySecurity.GetOrAdd(securityId, _ => new ConcurrentDictionary<string, MarketDataConnections>())
            .TryAdd(connection.Id, connection);
    }

    public void Unsubscribe(MarketDataConnections connection, long securityId)
    {
        if(_bySecurity.TryGetValue(securityId, out var set))
            set.TryRemove(connection.Id, out _);
    }

    public void RemoveEveryWhere(MarketDataConnections connections) // on disconnect
    {
        foreach (var set in _bySecurity.Values)
            set.TryRemove(connections.Id, out _);
    }
    
    public void BroadCast(long securityId, byte[] message)
    {
        if(!_bySecurity.TryGetValue(securityId, out var set)) return;
        foreach (var connection in set.Values)
            connection.Enqueue(message); // enqueue rather then sending
    }
    
    
}