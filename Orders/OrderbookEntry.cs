namespace TradingEngineServer.Orders;

public class OrderbookEntry: Order
{
    
    public Limit ParentLimit { get; private set; }
    
    public Order CurentOrder { get; private set; }
    public DateTime CreationTime { get; private set; }
    public OrderbookEntry? Next { get; set; }
    public OrderbookEntry? Previous { get; set; }
    
    
    // i'm avoiding primary constructors here because it's less dense without it in this scenario
    public OrderbookEntry(Order order, Limit parentLimit) :
        base(order.OrderId, order.SecurityId, order.Username, order.Side, order.Price, order.InitialQuantity)
    {
        CreationTime = DateTime.UtcNow;
        ParentLimit = parentLimit;
        CurentOrder = order;
    }
}