namespace Lab6.Services.DI;

public interface IUserService
{
    Guid ServiceId { get; }
    List<string> GetUsers();
    void AddUser(string user);
    int GetUserCount();
}
