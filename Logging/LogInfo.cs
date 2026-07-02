namespace TradingEngineServer.Logging;

public class LogInfo(
    LogLevel logLevel, 
    string module, 
    string message, 
    DateTime now, 
    int currentThreadManagedThreadId, 
    string? currentThreadName)
{
    public record LogInformation(
        LogLevel LogLevel, 
        string Module, 
        string Message, 
        DateTime Now, 
        int ThreadId);
}