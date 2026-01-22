using Microsoft.AspNetCore.Mvc;
using Lab6.Models;
using Lab6.Services;

namespace Lab6.Controllers;

public class CourseController : Controller
{
    private readonly ICourseService _courseService;

    public CourseController(ICourseService courseService)
    {
        _courseService = courseService;
    }

    // GET: Course
    public async Task<IActionResult> Index()
    {
        var courses = await _courseService.GetAllAsync();
        return View(courses);
    }

    // GET: Course/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var course = await _courseService.GetByIdAsync(id);
        if (course == null)
        {
            return NotFound();
        }
        return View(course);
    }

    // GET: Course/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Course/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Course course)
    {
        if (ModelState.IsValid)
        {
            try
            {
                await _courseService.AddAsync(course);
                TempData["Success"] = "Thêm khóa học thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi thêm khóa học: {ex.Message}";
                return View(course);
            }
        }
        return View(course);
    }

    // GET: Course/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var course = await _courseService.GetByIdAsync(id);
        if (course == null)
        {
            return NotFound();
        }
        return View(course);
    }

    // POST: Course/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Course course)
    {
        if (course == null)
        {
            TempData["Error"] = "Dữ liệu khóa học không hợp lệ!";
            return RedirectToAction(nameof(Index));
        }

        if (id != course.CourseId)
        {
            TempData["Error"] = "ID khóa học không khớp!";
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                await _courseService.UpdateAsync(course);
                TempData["Success"] = "Cập nhật khóa học thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi cập nhật: {ex.Message}";
                return View(course);
            }
        }
        return View(course);
    }

    // GET: Course/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        var course = await _courseService.GetByIdAsync(id);
        if (course == null)
        {
            return NotFound();
        }
        return View(course);
    }

    // POST: Course/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            await _courseService.DeleteAsync(id);
            TempData["Success"] = "Xóa khóa học thành công!";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Lỗi khi xóa khóa học: {ex.Message}";
            return RedirectToAction(nameof(Index));
        }
    }
}
