namespace TradingEngineServer.Logging.Configuration;

public class LoggingConfiguration
{
 public TextLogger TextLoggerConfiguration { get; set; }   
}


public class TextLoggerConfiguration
{
    public string directory { get; set; }
    public string fileName { get; set; }    
    public string fileExtension { get; set; }
}