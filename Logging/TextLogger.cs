using System.Threading.Tasks.Dataflow;
using Microsoft.Extensions.Options;
using TradingEngineServer.Logging.Configuration;

namespace TradingEngineServer.Logging;

public class TextLogger : AbstractLogger, ITextLogger
{
    private readonly LoggingConfiguration _loggingConfiguration;

    
    public TextLogger(IOptions<LoggingConfiguration> loggingConfiguration) : base()
    {
        _loggingConfiguration = loggingConfiguration.Value ?? throw new ArgumentNullException(nameof(loggingConfiguration));
        var config = _loggingConfiguration.TextLoggerConfiguration ?? throw new InvalidOperationException("TextLoggerConfiguration is missing.");
        
        // create the directory (if it doesn't exist) and start logging in a file
        
        string logFileName = $"{config.FileName}_{DateTime.Now:yyyy-MM-dd HH_mm_ss}.{config.FileExtension.TrimStart('.')}";
        string filePath = Path.Combine(config.Directory, logFileName);
        Directory.CreateDirectory(config.Directory);
        
        _ = Task.Run(() => logAsync(filePath, _logQueue, _tokenSource.Token));
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
        return $"[{logItem.Now:yyyy-MM-dd HH-mm-ss.fffffff}] [{logItem.ThreadName,-30}:{logItem.ThreadId:000}"
               + $"[{logItem.LogLevel}] {logItem.Message}";
    }

    protected override void Log(LogLevel logLevel, string module, string message)
    {
        _logQueue.Post(new LogInfo(logLevel, module, message, DateTime.Now, Thread.CurrentThread.ManagedThreadId,
            Thread.CurrentThread.Name));
    }

    // Finalizer Queue.... kinda like a destructor in C++ but not realaly
    ~TextLogger()
    {
        //destroy everything that hasn't been GC'd and is available for destruction
        Dispose(false);
    }
    
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        lock (_lock)
        {
            if (_disposed)
            {
                return;
            }
        }

        lock (_lock)
        {
            _disposed = true;
        }


        if (disposing)
        {
            // remove managed resources(internal stuff)
            _tokenSource.Cancel();
            _tokenSource.Dispose();
        }
        
        
        // remove unmanaged resources(external stuff) e.g db, filestream atc
    }

    /// <summary>
    /// thread-safe queue with an async api
    /// </summary>
    private readonly BufferBlock<LogInfo> _logQueue = new BufferBlock<LogInfo>();


    private readonly CancellationTokenSource _tokenSource = new CancellationTokenSource();
    private bool _disposed = false;
    private readonly object _lock = new object();
}