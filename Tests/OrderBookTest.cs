using Instruments;
using TradingEngineServer.OrderBook;
using TradingEngineServer.Orders;
using FluentAssertions;

namespace Tests;

public class OrderBookTest
{

    [Fact]
    public void AddOrder_ShouldInsertAndIndexCorrectly()
    {
        // Arrange
        var security = new Security(1, "AAPL");
        var orderbook = new OrderBook(security);

        var buyOrder = new Order(1, 1, "Jane Street", Side.Buy, 75, 100);
        var sellOrder = new Order(2, 2, "JPMORGAN CHASE", Side.Sell, 76, 100);
        
        // Act
        orderbook.AddOrder(buyOrder);
        orderbook.AddOrder(sellOrder);

        var expectedSpread = orderbook.GetSpread();
        var orderbookSpread = new OrderBookSpread(buyOrder.Price,  sellOrder.Price);
        
        // Assert
        orderbook.Count.Should().Be(2);
        orderbook.ContainsOrder(buyOrder.OrderId).Should().BeTrue();
        orderbook.ContainsOrder(sellOrder.OrderId).Should().BeTrue();
        
        orderbook.GetSpread().Should().BeEquivalentTo(expectedSpread);
    }
    
    /// <summary>
    /// essential to assert there are no memory leaks!!!
    /// </summary>
    /*
    [Fact]
    public void RemoveOrder_ShouldCleanUpAndPruneEmptyLimits()
    {
        // Arrange
        var security = new Security(1, "MSFT"); 
        var  orderbook = new OrderBook(security);
        
        var limit = new Limit(100);
        
        var order =  new Order(1, 1, "HRT", Side.Buy, 75, 100);
        var entry =  new OrderbookEntry(order, limit);
        
        var cancelOrder = new CancelOrder(order.OrderId, order.SecurityId, order.Username); 
        
        // Act
        orderbook.AddOrder(order);
        orderbook.RemoveOrder(cancelOrder);
        
        // Assert
        Assert.Equal(0, orderbook.Count);
        Assert.False(orderbook.ContainsOrder(order.OrderId));
        Assert.Null(orderbook.GetSpread());
    }
   */ 
    
    [Fact]
    public void ChangeOrder_ShouldReplaceOriginalOrder()
    {
        // Arrange
       var security = new Security(1, "SPCX"); 
       var orderbook = new OrderBook(security);
       
       var limit =  new Limit(100);

       var order = new Order(1, 2, "Jump Trading", Side.Buy, 75, 100);
       var entry =  new OrderbookEntry(order, limit);
        
       
    }
}