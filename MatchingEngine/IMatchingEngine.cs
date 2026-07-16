using Instruments;
using TradingEngineServer.OrderBook;
using TradingEngineServer.Orders;

namespace MatchingEngine;

public interface IMatchingEngine
{
   void AddOrderBook(Security instrument);
   MatchResult AddOrder(Order order);
   MatchResult ChangeOrders(ModifyOrder modifyOrder);
   void RemoveOrder(CancelOrder cancelOrder);
}