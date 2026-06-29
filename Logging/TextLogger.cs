namespace TradingEngineServer.Logging;

public class TextLogger: AbstractLogger, ITextLogger
{
    protected override void Log(LogLevel logLevel, string module, string message)
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }
}