using TradingEngineServer.Orders;

namespace TradingEngineServer.MatchingEngine.Algorithms;

/// <summary>
/// Price-Time Priority (FIFO) matching algorithm
/// At each price level, the oldest resting order (Head of the linked list) fills first.
/// </summary>
public class Fifo : IMatchingAlgorithm
{
    public static readonly IMatchingAlgorithm Instance = new Fifo();

    private Fifo()
    {
    } // enforces singleton pattern

    public static void RemoveFilledOrder(
        OrderbookEntry entry,
        SortedSet<Limit> limitLevel,
        Dictionary<long, OrderbookEntry> orders)
    {
        Limit parentLimit = entry.ParentLimit;

        // rewire the nodes beside the removed order to close the gap
        if (entry.Previous != null)
            entry.Previous.Next = entry.Next;
        if (entry.Next != null)
            entry.Next.Previous = entry.Previous;

        // update the head and tail pointers
        if (parentLimit.Head == entry)
            parentLimit.Head = entry.Next;

        if (parentLimit.Tail == entry)
            parentLimit.Tail = entry.Previous;

        orders.Remove(entry.OrderId);

        if (parentLimit.IsEmpty)
            limitLevel.Remove(parentLimit);
    }

    public MatchResult MatchIncoming(
        Order incoming,
        SortedSet<Limit> bidLimits,
        SortedSet<Limit> askLimits,
        Dictionary<long, OrderbookEntry> orders)
    {
        List<Fill> fills = new List<Fill>();

        // a buy aggressor lifts the asks; a sell aggressor hits the bids
        SortedSet<Limit> restingLimits = incoming.IsBuySide ? askLimits : bidLimits;

        while (incoming.CurrentQuantity > 0 && restingLimits.Count > 0)
        {
            Limit bestLevel = restingLimits.Min!;

            // does the incoming price cross this limit level?
            bool crosses = incoming.IsBuySide
                ? incoming.Price >= bestLevel.Price
                : incoming.Price <= bestLevel.Price;

            if (!crosses)
                break;

            OrderbookEntry restingEntry = bestLevel.Head!; // oldest order at the level (FIFO) 

            uint tradeQuantity = Math.Min(incoming.CurrentQuantity, restingEntry.CurrentQuantity);
            long executionPrice = bestLevel.Price; // resting order now sets the price

            // resting/passive side sets the execution price
            incoming.DecrementQuantity(tradeQuantity);
            restingEntry.DecrementQuantity(tradeQuantity);

            fills.Add(new Fill
            {
                SecurityId = incoming.SecurityId,
                BidOrderId = incoming.IsBuySide ? incoming.OrderId : restingEntry.OrderId,
                AskOrderId = incoming.IsBuySide ? restingEntry.OrderId : incoming.OrderId,
                ExecutionPrice = executionPrice,
                FilledAt = DateTime.UtcNow,
                FilledQuantity = tradeQuantity
            });

            //clean up filled orders
            if (restingEntry.CurrentQuantity == 0)
                RemoveFilledOrder(restingEntry, restingLimits, orders);
        }

        return new MatchResult(fills);
    }
}