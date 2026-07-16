using TradingEngineServer.Orders;

namespace TradingEngineServer.MatchingEngine.Algorithms;

/// <summary>
/// Price-Time Priority (FIFO) matching algorithm
/// At each price level, the oldest resting order (Head of the linked list) fills first.
/// </summary>
public class Fifo : IMatchingAlgorithm
{
    public static readonly IMatchingAlgorithm Instance = new Fifo();
    
    private Fifo() {} // enforces singleton pattern

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
    }

    public MatchResult Match(
        SortedSet<Limit> bidLimits,
        SortedSet<Limit> askLimits,
        Dictionary<long, OrderbookEntry> orders)
    {
        List<Fill> fills = new List<Fill>();

        while (bidLimits.Count > 0 && askLimits.Count > 0)
        {
            Limit bestBid = bidLimits.Min!; // BidLimitComparer... descending -> Min = highest price
            Limit bestAsk = askLimits.Min!; // AskLimitComparer... ascending -> Min = lowest price

            // if spread hasn't crossed, no match possible
            if (bestBid.Price < bestAsk.Price)
                break;

            // Head --> oldest order at this price level
            OrderbookEntry bidEntry = bestBid.Head!;
            OrderbookEntry askEntry = bestAsk.Head!;

            uint filledQuantity = Math.Min(bidEntry.CurrentQuantity, askEntry.CurrentQuantity);

            // resting/passive side sets the execution price
            // both orders were already in the book, use the ask price as a convention
            long executionPrice = askEntry.Price;

            bidEntry.DecrementQuantity(filledQuantity);
            askEntry.DecrementQuantity(filledQuantity);


            fills.Add(new Fill
            {
                SecurityId = bidEntry.SecurityId,
                BidOrderId = bidEntry.OrderId,
                AskOrderId = askEntry.OrderId,
                ExecutionPrice = executionPrice,
                FilledAt = DateTime.UtcNow,
                FilledQuantity = filledQuantity
            });

            //TODO: clean up filled orders
            if(bidEntry.CurrentQuantity == 0)
                RemoveFilledOrder(bidEntry, bidLimits ,orders);
            if(askEntry.CurrentQuantity == 0)
                RemoveFilledOrder(askEntry, askLimits, orders);
        }
        return new MatchResult(fills);
    }
}