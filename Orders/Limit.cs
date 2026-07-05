namespace TradingEngineServer.Orders;

public class Limit
{
    /*
    this is essentially the maximum they're willing to pay (bid)
    or the minimum they're willing to accept (ask)
    if a trader wants to buy Microsoft stock MSFT at $100.00, Limit == $100.00
    */
    public long Price { get; set; }
    public OrderbookEntry Head { get; set; }
    public OrderbookEntry Tail { get; set; }

    public bool IsEmpty => Head == null;
    
    public uint GetLevelOrderCount()
    {
        uint orderCount = 0;
        if (IsEmpty)
            return orderCount;

        OrderbookEntry entry = Head;
        while (entry != null)
        {
            if (entry.CurentOrder.CurrentQuantity != 0)
            {
                orderCount++;
            }
            entry = entry.Next;
        }
        
        return orderCount;
    }

    public uint GetLevelOrderQuantity()
    {
        uint orderQuantity = 0;
        if (IsEmpty)
            return orderQuantity;
        
        
        OrderbookEntry entry = Head;
        while (entry != null)
        {
            orderQuantity += entry.CurentOrder.CurrentQuantity;
            entry = entry.Next;
        }
        
        return orderQuantity;
    }


    public Side side
    {
        get
        {
            if (IsEmpty)
                return Side.Unknown;
            else
                return Head.CurentOrder.IsBuySide ? Side.Buy : Side.Sell;
        }
    }

    public List<OrderRecord> GetLevelOrderRecords()
    {
        List<OrderRecord> orderRecords = new List<OrderRecord>();
        OrderbookEntry entry = Head;
        uint theoreticalQueuePosition = 0;
        while (entry != null)
        {
            var currentOrder = entry.CurentOrder;
            if (currentOrder.CurrentQuantity != 0)
                orderRecords.Add(new OrderRecord(
                    currentOrder.OrderId,
                    currentOrder.CurrentQuantity,
                    Price,
                    currentOrder.IsBuySide,
                    currentOrder.Username,
                    currentOrder.SecurityId,
                    theoreticalQueuePosition
                ));

            theoreticalQueuePosition++;
            entry = entry.Next;
        }
        return orderRecords;
    }
}