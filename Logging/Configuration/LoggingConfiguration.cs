namespace Valkyrie.Logging.Configuration;

public class LoggingConfiguration
{
    public LoggingConfiguration(TextLoggerConfiguration textLoggerConfiguration)
    {
        TextLoggerConfiguration = textLoggerConfiguration;
    }

    public LoggerType LoggerType { get; set; }
    public TextLoggerConfiguration TextLoggerConfiguration { get; set; } // Fixed type here
    
    // i can define others later for db etc...
}

public class TextLoggerConfiguration
{
    public string Directory { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string FileExtension { get; set; } = string.Empty;
}