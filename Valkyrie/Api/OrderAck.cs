using Valkyrie.MatchingEngine;

namespace Valkyrie.Api;

public class OrderAck
{
    public long Id { get;  }
    public MatchResult Result { get;  }
    
    public OrderAck(MatchResult result, long id)
    {
        Id = id;
        Result = result;
    }   
    public static OrderAck From(long id, MatchResult result)
    {
        return new OrderAck(result, id);
    }
}