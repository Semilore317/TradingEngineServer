namespace TradingEngineServer.Logging.Configuration;

public class LoggingConfiguration
{
    public LoggerType LoggerType { get; set; }
    public TextLoggerConfiguration TextLoggerConfiguration { get; set; } // Fixed type here
    
    // i can define others later for db etc...
}

public class TextLoggerConfiguration
{
    public string Directory { get; set; }
    public string FileName { get; set; } 
    public string FileExtension { get; set; }
}