namespace Lab6.Services.DI;

public interface IEmailService
{
    Guid ServiceId { get; }
    void SendEmail(string recipient, string subject, string body);
    List<string> GetSentEmails();
}
