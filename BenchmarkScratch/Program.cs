using System.Diagnostics;
using Instruments;
using Valkyrie.MatchingEngine;
using Valkyrie.MatchingEngine.Algorithms;
using Valkyrie.Orders;

var security = new Security(1, "MSFT");
var engine = new MatchingEngine(Fifo.Instance);
engine.AddOrderBook(security);

int orderCount = 1_000_000;
var stopwatch = new Stopwatch();

Console.WriteLine($"Warming up...");
// Warmup
for (int i = 0; i < 1000; i++)
{
    engine.AddOrder(new Order(i, 1, "Jane Street", Side.Buy, 100, 10));
    engine.AddOrder(new Order(i + 1000, 1, "Citadel", Side.Sell, 100, 10));
}

engine = new MatchingEngine(Fifo.Instance);
engine.AddOrderBook(security);

Console.WriteLine($"Running benchmark with {orderCount} orders...");

GC.Collect();
GC.WaitForPendingFinalizers();

stopwatch.Start();

for (long i = 0; i < orderCount; i++)
{
    var side = (i % 2 == 0) ? Side.Buy : Side.Sell;
    var price = (i % 2 == 0) ? 100 + (i % 10) : 100 - (i % 10); // Causes constant crosses
    var order = new Order(i + 1, 1, "TestFirm", side, price, 10);
    engine.AddOrder(order);
}

stopwatch.Stop();

var elapsedMs = stopwatch.ElapsedMilliseconds;
var ordersPerSecond = (orderCount / (double)elapsedMs) * 1000;

Console.WriteLine($"Total Time: {elapsedMs} ms");
Console.WriteLine($"Orders Per Second: {ordersPerSecond:N0}");
