namespace Instruments;

public class Security(long securityId, string  symbol)
{
    public long SecurityId { get; } = securityId;
    public string Symbol { get; } = symbol;
}