using Lab5.Binders;
using Lab5.Models;
using Lab5.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lab5.Controllers
{
    public class StudentsController : Controller
    {
        private readonly IStudentService _studentService;
        private readonly ILogger<StudentsController> _logger;

        public StudentsController(IStudentService studentService, ILogger<StudentsController> logger)
        {
            _studentService = studentService;
            _logger = logger;
        }

        // GET: Students
        public async Task<IActionResult> Index()
        {
            var students = await _studentService.GetAllStudentsAsync();
            return View(students);
        }

        // GET: Students/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _studentService.GetStudentByIdAsync(id.Value);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // GET: Students/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Students/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Student student)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                                              .ToList();
                ViewBag.Errors = errors;
                return View(student);
            }

            try
            {
                // Ensure StudentDetails exists
                if (student.StudentDetails == null)
                {
                    student.StudentDetails = new StudentDetails();
                }
                
                await _studentService.CreateStudentAsync(student);
                TempData["Success"] = "Thêm sinh viên thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Errors = new List<string> { ex.InnerException?.Message ?? ex.Message };
                return View(student);
            }
        }

        // GET: Students/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _studentService.GetStudentByIdAsync(id.Value);
            if (student == null)
            {
                return NotFound();
            }
            return View(student);
        }

        // POST: Students/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Student student)
        {
            // Null check first
            if (student == null)
            {
                TempData["Error"] = "Invalid student data";
                return RedirectToAction(nameof(Index));
            }

            if (id != student.StudentId)
            {
                TempData["Error"] = "Student ID mismatch";
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                                              .ToList();
                ViewBag.Errors = errors;
                return View(student);
            }

            try
            {
                // Ensure StudentDetails exists
                if (student.StudentDetails == null)
                {
                    student.StudentDetails = new StudentDetails
                    {
                        StudentId = student.StudentId
                    };
                }
                else
                {
                    student.StudentDetails.StudentId = student.StudentId;
                }
                
                await _studentService.UpdateStudentAsync(student);
                TempData["Success"] = "Cập nhật sinh viên thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating student {StudentId}", id);
                
                if (!await _studentService.StudentExistsAsync(id))
                {
                    TempData["Error"] = "Student not found";
                    return RedirectToAction(nameof(Index));
                }

                ViewBag.Errors = new List<string> { ex.InnerException?.Message ?? ex.Message };
                return View(student);
            }
        }

        // GET: Students/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _studentService.GetStudentByIdAsync(id.Value);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _studentService.DeleteStudentAsync(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting student {StudentId}", id);
                TempData["Error"] = "Error deleting student: " + (ex.InnerException?.Message ?? ex.Message);
                return RedirectToAction(nameof(Index));
            }
        }

        // Quick Create with Custom Binder
        public IActionResult QuickCreate()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> QuickCreate([ModelBinder(typeof(QuickStudentBinder))] Student student)
        {
            if (student == null)
            {
                ViewBag.Errors = new List<string> { "Invalid input format" };
                return View();
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                                              .ToList();
                ViewBag.Errors = errors;
                return View(student);
            }

            try
            {
                await _studentService.CreateStudentAsync(student);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                 ViewBag.Errors = new List<string> { ex.InnerException?.Message ?? ex.Message };
                 return View(student);
            }
        }
    }
}
