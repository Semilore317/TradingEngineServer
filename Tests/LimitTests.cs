using TradingEngineServer.Orders;

namespace Tests;

public class LimitTests
{
    [Fact]
    public void GetLevelOrderCount_ShouldSumValuesCorrectly()
    {
        // Arrange: Create a Limit level and add several orders to it.
        // create Order objects and link them into Limit's Head/Tail.

        var limit = new Limit(1500);
        var order1 = new Order(1, 1, "HRT", Side.Buy, 1500, 100);
        var order2 = new Order(2, 2, "OPTV", Side.Buy, 1500, 1200);

        var entry1 = new OrderbookEntry(order1, limit);
        var entry2 = new OrderbookEntry(order2, limit);

        entry1.Next = entry2;
        entry2.Previous = entry1;

        limit.Head = entry1;
        limit.Tail = entry2;

        // Act

        uint orderCount = limit.GetLevelOrderCount();

        // 3. Assert: Verify the sum matches the expected aggregate quantity. 
        Assert.Equal(2, (int)orderCount);
    }

    [Fact]
    public void GetLevelOrderQuantity_ShouldSumValuesCorrectly()
    {
        // Assert
        var limit = new Limit(1500);

        var order1 = new Order(1, 1, "HRT", Side.Buy, 1500, 100);
        var order2 = new Order(2, 2, "OPTV", Side.Buy, 1500, 1200);

        var entry1 = new OrderbookEntry(order1, limit);
        var entry2 = new OrderbookEntry(order2, limit);

        entry1.Next = entry2;
        entry2.Previous = entry1;

        limit.Head = entry1;
        limit.Tail = entry2;

        // Act
        uint orderQuantity = limit.GetLevelOrderQuantity();

        // Assert
        Assert.Equal(1300u, orderQuantity);
    }
    
    /// <summary>
    /// This test verifies that the order book correctly extract the
    /// L3 snapshot records for external systems, specifically tracking their positions in queue.
    /// </summary>
    [Fact]
    public void GetLevelOrderRecords_ShouldReturnCorrectRecords()
    {
        // Arrange
        var limit  = new Limit(1500);
        var order1 = new Order(1, 1, "HRT", Side.Buy, 1500, 100);
        var order2 = new Order(2, 2, "OPTV", Side.Buy, 1500, 1200);
        
        var entry1 = new OrderbookEntry(order1, limit);
        var entry2 = new OrderbookEntry(order2, limit);
        
        entry1.Next = entry2;
        entry2.Previous = entry1;
        
        limit.Head = entry1;
        limit.Tail = entry2;
        
        // Act
        var records = limit.GetLevelOrderRecords();
        
        // Assert
        
        // count check
        Assert.Equal(2, records.Count);
        
        // position checks
        Assert.Equal(0u,  records[0].TheoreticalQueuePosition);
        Assert.Equal(1u, records[1].TheoreticalQueuePosition);
        
        //property checks
        Assert.Equal(1u, records[0].OrderId);
        Assert.Equal(2u, records[1].OrderId);
        
        Assert.Equal(100u, records[0].Quantity);
        Assert.Equal(1200u, records[1].Quantity);
        
        Assert.Equal("HRT", records[0].Username);
        Assert.Equal("OPTV", records[1].Username);
        
    } 
}