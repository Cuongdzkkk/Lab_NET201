namespace Lab6.Services.DI;

public class LoggingService : ILoggingService
{
    public Guid ServiceId { get; } = Guid.NewGuid();
    private readonly List<string> _logs = new();

    public void Log(string message)
    {
        var logEntry = $"[{DateTime.Now:HH:mm:ss}] {message}";
        _logs.Add(logEntry);
    }

    public List<string> GetLogs() => _logs.ToList();
}
