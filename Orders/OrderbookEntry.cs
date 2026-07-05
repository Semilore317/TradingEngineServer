namespace TradingEngineServer.Orders;

public class OrderbookEntry: Order
{
    // i'm avoiding primary constructors here because it's less dense without it in this scenario
    public OrderbookEntry(Order order, Limit parentLimit) :
        base(order.OrderId, order.SecurityId, order.Username, order.Side, order.Price, order.InitialQuantity)
    {
        ParentLimit = parentLimit;
    }

    public Limit ParentLimit { get;}
    public OrderbookEntry? Next { get; set; }
    public OrderbookEntry? Previous { get; set; }
    
}