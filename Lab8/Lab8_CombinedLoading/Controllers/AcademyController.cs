// Controllers/AcademyController.cs
// Controller xử lý các request liên quan đến quản lý học sinh nâng cao
// Demo tất cả 3 loại loading: Eager, Lazy, Explicit

using Lab8_CombinedLoading.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lab8_CombinedLoading.Controllers
{
    /// <summary>
    /// Controller quản lý Academy - demo combined loading
    /// </summary>
    public class AcademyController : Controller
    {
        private readonly IAcademyService _academyService;
        private readonly ILogger<AcademyController> _logger;

        public AcademyController(IAcademyService academyService, ILogger<AcademyController> logger)
        {
            _academyService = academyService;
            _logger = logger;
        }

        /// <summary>
        /// Trang chủ - Tổng quan về Lab 8
        /// </summary>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// EAGER LOADING: Hiển thị tất cả students với courses
        /// Sử dụng .Include().ThenInclude()
        /// </summary>
        public async Task<IActionResult> EagerLoading()
        {
            _logger.LogInformation("=== DEMO EAGER LOADING ===");
            var students = await _academyService.GetAllStudentsWithCoursesEagerAsync();
            return View(students);
        }

        /// <summary>
        /// LAZY LOADING: Demo với 1 student cụ thể
        /// </summary>
        public async Task<IActionResult> LazyLoading(int id = 1)
        {
            _logger.LogInformation("=== DEMO LAZY LOADING ===");
            var (student, logs) = await _academyService.GetStudentWithLazyLoadingDemoAsync(id);
            
            // Lấy danh sách cho dropdown
            ViewBag.StudentList = await _academyService.GetAllStudentsLookupAsync();

            if (student == null)
            {
                return NotFound("Không tìm thấy học sinh");
            }

            ViewBag.QueryLogs = logs;
            return View(student);
        }

        /// <summary>
        /// EXPLICIT LOADING: Demo với 1 student cụ thể
        /// </summary>
        public async Task<IActionResult> ExplicitLoading(int id = 1)
        {
            _logger.LogInformation("=== DEMO EXPLICIT LOADING ===");
            var student = await _academyService.GetStudentWithExplicitLoadingAsync(id);
            
            // Lấy danh sách cho dropdown
            ViewBag.StudentList = await _academyService.GetAllStudentsLookupAsync();
            
            if (student == null)
            {
                return NotFound("Không tìm thấy học sinh");
            }

            return View(student);
        }

        /// <summary>
        /// So sánh EF Core vs Raw SQL
        /// </summary>
        public async Task<IActionResult> Compare()
        {
            _logger.LogInformation("=== SO SÁNH EF CORE VS SQL ===");
            var result = await _academyService.CompareEfCoreVsSqlAsync();
            return View(result);
        }
    }
}
