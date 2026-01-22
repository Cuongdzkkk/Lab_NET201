namespace Lab6.Services.DI;

public class EmailService : IEmailService
{
    public Guid ServiceId { get; } = Guid.NewGuid();
    private static readonly List<string> _sentEmails = new();

    public void SendEmail(string recipient, string subject, string body)
    {
        var email = $"To: {recipient} | Subject: {subject} | Body: {body}";
        _sentEmails.Add(email);
    }

    public List<string> GetSentEmails() => _sentEmails.ToList();
}
