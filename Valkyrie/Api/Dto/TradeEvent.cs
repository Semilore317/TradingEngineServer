using Valkyrie.MatchingEngine;

namespace Valkyrie.Api.Dto;

public record TradeEvent(
    long SecurityId,
    long Price,
    uint Quantity,
    DateTime FilledAt
    )
{
    public static TradeEvent From(Fill fill)
    {
        return new TradeEvent(fill.SecurityId, fill.ExecutionPrice, fill.FilledQuantity, fill.FilledAt);
    }
}