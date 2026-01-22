namespace Lab6.Services.DI;

public class UserService : IUserService
{
    public Guid ServiceId { get; } = Guid.NewGuid();
    private static readonly List<string> _users = new() { "Admin", "User1", "User2" };

    public List<string> GetUsers() => _users.ToList();

    public void AddUser(string user)
    {
        if (!string.IsNullOrWhiteSpace(user))
        {
            _users.Add(user);
        }
    }

    public int GetUserCount() => _users.Count;
}
