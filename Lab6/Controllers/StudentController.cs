using Microsoft.AspNetCore.Mvc;
using Lab6.Models;
using Lab6.Services;

namespace Lab6.Controllers;

public class StudentController : Controller
{
    private readonly IStudentService _studentService;

    public StudentController(IStudentService studentService)
    {
        _studentService = studentService;
    }

    // GET: Student
    public async Task<IActionResult> Index()
    {
        var students = await _studentService.GetAllAsync();
        return View(students);
    }

    // GET: Student/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var student = await _studentService.GetByIdAsync(id);
        if (student == null)
        {
            return NotFound();
        }
        return View(student);
    }

    // GET: Student/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Student/Create (Standard form)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Student student)
    {
        if (student == null)
        {
            Console.WriteLine("[STUDENT CREATE] Received null student object");
            TempData["Error"] = "Dữ liệu sinh viên không hợp lệ!";
            return RedirectToAction(nameof(Index));
        }
        Console.WriteLine($"[STUDENT CREATE] Received: {student.StudentName}, {student.Email}, {student.PhoneNumber}, {student.DateOfBirth}, {student.Address}, {student.GPA}");
        Console.WriteLine($"[STUDENT CREATE] Received: {student.StudentName}, {student.Email}, {student.PhoneNumber}, {student.DateOfBirth}, {student.Address}, {student.GPA}");
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
            Console.WriteLine($"[STUDENT CREATE] ModelState Errors: {errorMessage}");
            
            return View(student);
        }

        try
        {
            await _studentService.AddAsync(student);
            TempData["Success"] = "Thêm sinh viên thành công!";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Lỗi khi thêm sinh viên: {ex.Message}";
            Console.WriteLine($"[STUDENT CREATE] Exception: {ex.Message}");
            return View(student);
        }
    }

    // POST: Student/CreateCustom (Custom format string)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateCustom([ModelBinder(BinderType = typeof(CustomBinders.StudentModelBinder))] Student student)
    {
        if (ModelState.IsValid)
        {
            await _studentService.AddAsync(student);
            TempData["Success"] = "Thêm sinh viên từ custom format thành công!";
            return RedirectToAction(nameof(Index));
        }
        
        // If validation failed, redirect back to Create with error
        TempData["Error"] = "Lỗi khi thêm sinh viên. Vui lòng kiểm tra lại định dạng dữ liệu.";
        return RedirectToAction(nameof(Create));
    }

    // GET: Student/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var student = await _studentService.GetByIdAsync(id);
        if (student == null)
        {
            return NotFound();
        }
        return View(student);
    }

    // POST: Student/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Student student)
    {
        if (student == null)
        {
            Console.WriteLine("[STUDENT EDIT] Received null student object");
            TempData["Error"] = "Dữ liệu sinh viên không hợp lệ!";
            return RedirectToAction(nameof(Index));
        }
        Console.WriteLine($"[STUDENT EDIT] Received: {student.StudentName}, {student.Email}, {student.PhoneNumber}, {student.DateOfBirth}, {student.Address}, {student.GPA}");
        Console.WriteLine($"[STUDENT EDIT] Received: {student.StudentName}, {student.Email}, {student.PhoneNumber}, {student.DateOfBirth}, {student.Address}, {student.GPA}");
        // Null check for student parameter to prevent NullReferenceException
        if (student == null)
        {
            TempData["Error"] = "Dữ liệu sinh viên không hợp lệ!";
            return RedirectToAction(nameof(Index));
        }

        // Check if ID matches
        if (id != student.StudentId)
        {
            TempData["Error"] = "ID sinh viên không khớp!";
            return NotFound();
        }

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
            Console.WriteLine($"[STUDENT EDIT] ModelState Errors: {errorMessage}");
            
            return View(student);
        }

        try
        {
            await _studentService.UpdateAsync(student);
            TempData["Success"] = "Cập nhật sinh viên thành công!";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Lỗi khi cập nhật: {ex.Message}";
            Console.WriteLine($"[STUDENT EDIT] Exception: {ex.Message}");
            return View(student);
        }
    }

    // GET: Student/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        var student = await _studentService.GetByIdAsync(id);
        if (student == null)
        {
            return NotFound();
        }
        return View(student);
    }

    // POST: Student/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            await _studentService.DeleteAsync(id);
            TempData["Success"] = "Xóa sinh viên thành công!";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Lỗi khi xóa sinh viên: {ex.Message}";
            return RedirectToAction(nameof(Index));
        }
    }
}
