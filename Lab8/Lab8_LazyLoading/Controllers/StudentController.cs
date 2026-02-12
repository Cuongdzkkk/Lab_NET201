// Controllers/StudentController.cs
// Controller xử lý các request liên quan đến học sinh
// Demo Lazy Loading với Entity Framework Core

using Lab8_LazyLoading.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lab8_LazyLoading.Controllers
{
    /// <summary>
    /// Controller quản lý hiển thị học sinh với Lazy Loading
    /// </summary>
    public class StudentController : Controller
    {
        private readonly IStudentService _studentService;
        private readonly ILogger<StudentController> _logger;

        public StudentController(IStudentService studentService, ILogger<StudentController> logger)
        {
            _studentService = studentService;
            _logger = logger;
        }

        /// <summary>
        /// Trang chủ - Hiển thị tất cả students
        /// LAZY LOADING: Courses sẽ được load khi render View
        /// </summary>
        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("=== BÀI 2: LAZY LOADING DEMO ===");
            _logger.LogInformation("Đang gọi GetAllStudentsAsync()...");
            _logger.LogInformation("CHÚ Ý: Courses sẽ được load SAU khi View truy cập property!");

            // Lấy students - chưa load Courses
            var students = await _studentService.GetAllStudentsAsync();

            _logger.LogInformation("Trả về View với {Count} students", students.Count);
            _logger.LogInformation("Khi View render @student.Courses.Count -> Lazy Loading kích hoạt!");

            return View(students);
        }

        /// <summary>
        /// Chi tiết 1 student - Demo Lazy Loading
        /// </summary>
        public async Task<IActionResult> Details(int id)
        {
            var (student, courseCount, queryLogs) = 
                await _studentService.GetStudentWithLazyLoadingDemoAsync(id);
            
            if (student == null)
            {
                return NotFound("Không tìm thấy học sinh");
            }

            ViewBag.QueryLogs = queryLogs;
            ViewBag.CourseCount = courseCount;

            return View(student);
        }

        /// <summary>
        /// Demo N+1 Problem
        /// </summary>
        public async Task<IActionResult> N1Problem()
        {
            _logger.LogInformation("=== DEMO N+1 PROBLEM ===");

            var students = await _studentService.GetAllStudentsAsync();

            // Mô phỏng truy cập Courses của mỗi student
            var stats = new List<(string Name, int CourseCount)>();
            foreach (var student in students)
            {
                // MỖI LẦN TRUY CẬP NÀY = 1 QUERY MỚI ĐẾN DATABASE!
                _logger.LogWarning("Query mới để load Courses của {Name}", student.Name);
                stats.Add((student.Name, student.Courses.Count));
            }

            ViewBag.Stats = stats;
            ViewBag.TotalQueries = students.Count + 1; // 1 + N

            return View();
        }
    }
}
