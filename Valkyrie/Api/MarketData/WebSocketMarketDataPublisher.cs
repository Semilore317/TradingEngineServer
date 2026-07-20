using System.Text.Json;
using Valkyrie.Api.Dto;
using Valkyrie.MatchingEngine;

namespace Valkyrie.Api.MarketData;

public sealed class WebSocketMarketDataPublisher(MarketDataHub Hub) : IMarketDataPublisher
{
    // use camelCase
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);
    
    private static byte[] Serialize(Object o) => JsonSerializer.SerializeToUtf8Bytes(o, Json);
    
    public void PublishTrade(TradeEvent trade)
    {
        Hub.BroadCast(trade.SecurityId, Serialize(new
        {
            type = "trade", trade.SecurityId, trade.Price, trade.Quantity, trade.FilledAt
        }));
    }

    public void PublishBook(OrderBookSnapshot snapshot)
    {
        Hub.BroadCast(snapshot.SecurityId, Serialize(new
        {
            type = "book", 
            snapshot.SecurityId,
            snapshot.Bid, 
            snapshot.Ask, 
            snapshot.Spread, 
            snapshot.Bids,
            snapshot.Asks
        }));
    }
    
}