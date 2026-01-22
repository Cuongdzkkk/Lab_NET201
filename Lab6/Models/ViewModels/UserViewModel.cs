namespace Lab6.Models.ViewModels;

public class UserViewModel
{
    public List<string> Users { get; set; } = new();
    public Guid UserServiceId { get; set; }
    public Guid LoggingServiceId { get; set; }
    public Guid EmailService1Id { get; set; }
    public Guid EmailService2Id { get; set; }
    public List<string> Logs { get; set; } = new();
}
