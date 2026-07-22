namespace Valkyrie.Core.Configuration;

public sealed class MarketSimulatorConfiguration
{
    public bool Enabled { get; set; }
    public string Username { get; set; } = "mm"; // market-maker identity
    public List<SimulatedInstrument> Instruments { get; set; } = new();
}

public sealed class SimulatedInstrument
{
    public long SecurityId { get; set; }
    public long SeedPrice { get; set; }
    public long TickSize { get; set; } = 1; // $0.01
    public double OrdersPerSecond { get; set; } = 2.0;
    public int BookDepth { get; set; } = 6; // pending bids and asks on both sides i'm maintaining for the sim 
}