using Instruments;
using TradingEngineServer.Orders;

namespace TradingEngineServer.MatchingEngine;

public interface IMatchingEngine
{
   void AddOrderBook(Security instrument);
   MatchResult AddOrder(Order order);
   MatchResult ChangeOrders(ModifyOrder modifyOrder);
   void RemoveOrder(CancelOrder cancelOrder);
}