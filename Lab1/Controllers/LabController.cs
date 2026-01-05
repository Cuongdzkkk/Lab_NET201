using Microsoft.AspNetCore.Mvc;

namespace Lab01_Vibe.Controllers
{
    public class LabController : Controller
    {
        // LAB 1 HUB - The menu for Lab 1 exercises
        public IActionResult Index()
        {
            return View();
        }

        // a. Action TestContent: Returns View for beautiful display
        public IActionResult TestContent()
        {
            ViewBag.Message = "Hello, this is pure content from CUONGDZ";
            return View(); // Returns Views/Lab/TestContent.cshtml
        }

        // b. Action TestJson: Returns View with JSON model
        public IActionResult TestJson()
        {
            var studentInfo = new
            {
                Name = "CUONGDZ User",
                MSSV = "PH12345",
                Score = 10.0,
                Subject = "ASP.NET Core MVC"
            };
            // Serialize to beautify in View or pass as Object
            return View(studentInfo); // Returns Views/Lab/TestJson.cshtml
        }

        // c. Action GoAway: Redirects with intermediate page
        public IActionResult GoAway()
        {
            // Returns a view that shows "Redirecting..." then Javascript does the redirect
            return View(); // Returns Views/Lab/GoAway.cshtml
        }

        // d. Action ViewImage: Returns View with Image
        public IActionResult ViewImage()
        {
            // Just pass the path logic to the view or let the view handle the source
            // We'll pass the filename to the view
            ViewBag.ImageFileName = "Myu7kvXj7XEfCY7NzDD495duNCzTeQR4HAR5UGbe.jpg";
            return View(); // Returns Views/Lab/ViewImage.cshtml
        }
    }
}
