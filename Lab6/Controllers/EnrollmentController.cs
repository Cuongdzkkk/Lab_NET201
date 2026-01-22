using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Lab6.Models;
using Lab6.Services;

namespace Lab6.Controllers;

public class EnrollmentController : Controller
{
    private readonly IEnrollmentService _enrollmentService;
    private readonly IStudentService _studentService;
    private readonly ICourseService _courseService;

    public EnrollmentController(
        IEnrollmentService enrollmentService,
        IStudentService studentService,
        ICourseService courseService)
    {
        _enrollmentService = enrollmentService;
        _studentService = studentService;
        _courseService = courseService;
    }

    // GET: Enrollment
    public async Task<IActionResult> Index()
    {
        var enrollments = await _enrollmentService.GetAllAsync();
        return View(enrollments);
    }

    // GET: Enrollment/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var enrollment = await _enrollmentService.GetByIdAsync(id);
        if (enrollment == null)
        {
            return NotFound();
        }
        return View(enrollment);
    }

    // GET: Enrollment/Create
    public async Task<IActionResult> Create()
    {
        var students = await _studentService.GetAllAsync();
        var courses = await _courseService.GetAllAsync();

        ViewBag.Students = new SelectList(students, "StudentId", "StudentName");
        ViewBag.Courses = new SelectList(courses, "CourseId", "CourseName");

        return View();
    }

    // POST: Enrollment/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Enrollment enrollment)
    {
        // DEBUG: Log received values
        Console.WriteLine($"[ENROLLMENT CREATE] StudentId: {enrollment?.StudentId}, CourseId: {enrollment?.CourseId}");
        
        // DEBUG: Log ModelState errors
        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .Where(x => x.Value.Errors.Count > 0)
                .Select(x => new { 
                    Field = x.Key, 
                    Errors = string.Join(", ", x.Value.Errors.Select(e => e.ErrorMessage)) 
                })
                .ToList();
            
            var errorMessage = string.Join(" | ", errors.Select(e => $"{e.Field}: {e.Errors}"));
            TempData["Error"] = $"Validation failed: {errorMessage}";
            Console.WriteLine($"[ENROLLMENT CREATE] ModelState Errors: {errorMessage}");
            
            // Reload dropdowns
            var students = await _studentService.GetAllAsync();
            var courses = await _courseService.GetAllAsync();
            ViewBag.Students = new SelectList(students, "StudentId", "StudentName");
            ViewBag.Courses = new SelectList(courses, "CourseId", "CourseName");
            
            return View(enrollment);
        }

        try
        {
            await _enrollmentService.AddAsync(enrollment);
            TempData["Success"] = "Thêm đăng ký khóa học thành công!";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Lỗi khi thêm đăng ký: {ex.Message}";
            Console.WriteLine($"[ENROLLMENT CREATE] Exception: {ex.Message}");

            // Reload dropdowns before returning
            var studentsError = await _studentService.GetAllAsync();
            var coursesError = await _courseService.GetAllAsync();
            ViewBag.Students = new SelectList(studentsError, "StudentId", "StudentName");
            ViewBag.Courses = new SelectList(coursesError, "CourseId", "CourseName");

            return View(enrollment);
        }
    }

    // GET: Enrollment/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var enrollment = await _enrollmentService.GetByIdAsync(id);
        if (enrollment == null)
        {
            return NotFound();
        }

        var students = await _studentService.GetAllAsync();
        var courses = await _courseService.GetAllAsync();

        ViewBag.Students = new SelectList(students, "StudentId", "StudentName", enrollment.StudentId);
        ViewBag.Courses = new SelectList(courses, "CourseId", "CourseName", enrollment.CourseId);

        return View(enrollment);
    }

    // POST: Enrollment/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Enrollment enrollment)
    {
        Console.WriteLine($"[ENROLLMENT EDIT] Received: StudentId={enrollment.StudentId}, CourseId={enrollment.CourseId}, EnrollmentDate={enrollment.EnrollmentDate}, Grade={enrollment.Grade}");
        // Null check for enrollment parameter to prevent NullReferenceException
        if (enrollment == null)
        {
            TempData["Error"] = "Dữ liệu đăng ký không hợp lệ!";
            return RedirectToAction(nameof(Index));
        }

        // Check if ID matches
        if (id != enrollment.EnrollmentId)
        {
            TempData["Error"] = "ID đăng ký không khớp!";
            return NotFound();
        }

        // DEBUG: Log received values
        Console.WriteLine($"[ENROLLMENT EDIT] ID: {id}, StudentId: {enrollment.StudentId}, CourseId: {enrollment.CourseId}");
        
        // DEBUG: Log ModelState errors
        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .Where(x => x.Value.Errors.Count > 0)
                .Select(x => new { 
                    Field = x.Key, 
                    Errors = string.Join(", ", x.Value.Errors.Select(e => e.ErrorMessage)) 
                })
                .ToList();
            
            var errorMessage = string.Join(" | ", errors.Select(e => $"{e.Field}: {e.Errors}"));
            TempData["Error"] = $"Validation failed: {errorMessage}";
            Console.WriteLine($"[ENROLLMENT EDIT] ModelState Errors: {errorMessage}");
            
            // Reload dropdowns before returning
            var students = await _studentService.GetAllAsync();
            var courses = await _courseService.GetAllAsync();
            ViewBag.Students = new SelectList(students, "StudentId", "StudentName", enrollment.StudentId);
            ViewBag.Courses = new SelectList(courses, "CourseId", "CourseName", enrollment.CourseId);
            
            return View(enrollment);
        }

        try
        {
            await _enrollmentService.UpdateAsync(enrollment);
            TempData["Success"] = "Cập nhật đăng ký thành công!";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Lỗi khi cập nhật: {ex.Message}";
            Console.WriteLine($"[ENROLLMENT EDIT] Exception: {ex.Message}");

            // Reload dropdowns before returning to view
            var studentsError = await _studentService.GetAllAsync();
            var coursesError = await _courseService.GetAllAsync();
            ViewBag.Students = new SelectList(studentsError, "StudentId", "StudentName", enrollment.StudentId);
            ViewBag.Courses = new SelectList(coursesError, "CourseId", "CourseName", enrollment.CourseId);

            return View(enrollment);
        }
    }

    // GET: Enrollment/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        var enrollment = await _enrollmentService.GetByIdAsync(id);
        if (enrollment == null)
        {
            return NotFound();
        }
        return View(enrollment);
    }

    // POST: Enrollment/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            await _enrollmentService.DeleteAsync(id);
            TempData["Success"] = "Xóa đăng ký thành công!";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Lỗi khi xóa đăng ký: {ex.Message}";
            return RedirectToAction(nameof(Index));
        }
    }
}
