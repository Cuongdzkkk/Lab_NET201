using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Lab01_Vibe.Models;

namespace Lab01_Vibe.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IActionResult GetSystemMetrics()
    {
        var rnd = new Random();
        // Simulate data: CPU 10-99%, RAM 4-64GB, Temp 40-95C
        var cpu = rnd.Next(10, 100); 
        var ram = rnd.Next(4, 64);
        var temp = rnd.Next(40, 95);
        
        return Json(new { cpu, ram, temp });
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Lab1Hub()
    {
        return View(); // Returns Views/Home/Lab1Hub.cshtml
    }

    public IActionResult Lab1Control()
    {
        return View(); // New Test Interface
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
