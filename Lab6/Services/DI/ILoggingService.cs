namespace Lab6.Services.DI;

public interface ILoggingService
{
    Guid ServiceId { get; }
    void Log(string message);
    List<string> GetLogs();
}
