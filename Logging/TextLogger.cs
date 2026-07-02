using System.Threading.Tasks.Dataflow;
using Microsoft.Extensions.Options;
using TradingEngineServer.Logging.Configuration;

namespace TradingEngineServer.Logging;

public class TextLogger: AbstractLogger, ITextLogger
{
    private readonly LoggingConfiguration _loggingConfiguration;
    
    public TextLogger(IOptions<LoggingConfiguration> loggingConfiguration): base()
    {
        _loggingConfiguration = loggingConfiguration.Value ?? throw new ArgumentNullException(nameof(loggingConfiguration));
    }

    private static async Task logAsync(string filepath, BufferBlock<LogInfo> logQueue, CancellationToken token)
    {
        // using tells C# to close these when the operations end
        // important since they open direct connections to the OS's file system
        using var fileStream = new FileStream(filepath, FileMode.Append, FileAccess.Write, FileShare.Read);
        using var streamWriter = new StreamWriter(fileStream);

        try
        {
            while (true)
            {
                var logItem = await logQueue.ReceiveAsync(token).ConfigureAwait(false);
                string formattedMessage = FormatLogItem(logItem);
                
                await streamWriter.WriteLineAsync(formattedMessage).ConfigureAwait(false);
            }
            {
                
            }
        }
        catch (OperationCanceledException)
        {
            
        }
    }

    private static string FormatLogItem(LogInfo logItem)
    {
        return $"[{logItem.Now:dd-MM-yyyy HH-mm-ss.fffffff}] [{logItem.ThreadName, -30}:{logItem.ThreadId:000}"
            + $"[{logItem.LogLevel}] {logItem.Message}";
    }

    protected override void Log(LogLevel logLevel, string module, string message)
    {
        _logQueue.Post(new LogInfo(logLevel, module, message, DateTime.Now, Thread.CurrentThread.ManagedThreadId, Thread.CurrentThread.Name));
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// thread-safe queue with an async api
    /// </summary>
    private readonly BufferBlock<LogInfo> _logQueue = new BufferBlock<LogInfo>();
}