using Instruments;
using Valkyrie.Orders;

namespace Valkyrie.MatchingEngine;

public interface IMatchingEngine
{
   void AddOrderBook(Security instrument);
   MatchResult AddOrder(Order order);
   MatchResult ChangeOrders(ModifyOrder modifyOrder);
   void RemoveOrder(CancelOrder cancelOrder);
}