using Microsoft.AspNetCore.Mvc;
using Lab6.Models.ViewModels;
using Lab6.Services.DI;

namespace Lab6.Controllers;

public class UserController : Controller
{
    private readonly IUserService _userService;
    private readonly ILoggingService _loggingService;
    private readonly IEmailService _emailService1;
    private readonly IEmailService _emailService2;

    public UserController(
        IUserService userService,
        ILoggingService loggingService,
        IEmailService emailService1,
        IEmailService emailService2)
    {
        _userService = userService;
        _loggingService = loggingService;
        _emailService1 = emailService1;
        _emailService2 = emailService2;
    }

    // GET: User
    public IActionResult Index()
    {
        _loggingService.Log("Truy cập trang User/Index");

        var model = new UserViewModel
        {
            Users = _userService.GetUsers(),
            UserServiceId = _userService.ServiceId,
            LoggingServiceId = _loggingService.ServiceId,
            EmailService1Id = _emailService1.ServiceId,
            EmailService2Id = _emailService2.ServiceId,
            Logs = _loggingService.GetLogs()
        };

        return View(model);
    }

    // POST: User/AddUser
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult AddUser(string userName)
    {
        if (!string.IsNullOrWhiteSpace(userName))
        {
            _userService.AddUser(userName);
            _loggingService.Log($"Đã thêm user: {userName}");
            _emailService1.SendEmail("admin@test.com", "New User", $"User {userName} đã được thêm");
            TempData["Success"] = "Thêm user thành công!";
        }
        else
        {
            TempData["Error"] = "Tên user không được để trống!";
        }

        return RedirectToAction(nameof(Index));
    }

    // GET: User/LifetimeExplanation
    public IActionResult LifetimeExplanation()
    {
        return View();
    }
}
