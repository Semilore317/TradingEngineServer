namespace TradingEngineServer.Logging.Configuration;

public class LoggingConfiguration
{
    public LoggerType LoggerType { get; set; }
    public TextLogger TextLoggerConfiguration { get; set; }
    // i can define other loggers later
}

public class DatabaseLoggerConfiguration
{
}

public class TextLoggerConfiguration
{
    public string Directory { get; set; }
    public string FileName { get; set; }
    public string FileExtension { get; set; }
}