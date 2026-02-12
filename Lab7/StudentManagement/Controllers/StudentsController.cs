using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using StudentManagement.Data;
using StudentManagement.Models;

namespace StudentManagement.Controllers
{
    /// <summary>
    /// Controller quản lý sinh viên sử dụng Native SQL (FromSqlRaw, ExecuteSqlRaw)
    /// KHÔNG sử dụng LINQ to Entities theo yêu cầu của bài lab
    /// </summary>
    public class StudentsController : Controller
    {
        private readonly StudentContext _context;

        public StudentsController(StudentContext context)
        {
            _context = context;
        }

        // GET: Students
        // Hiển thị danh sách tất cả sinh viên sử dụng FromSqlRaw với hỗ trợ tìm kiếm
        public async Task<IActionResult> Index(string? searchString)
        {
            try
            {
                List<Student> students;
                
                if (!string.IsNullOrWhiteSpace(searchString))
                {
                    // Sử dụng FromSqlRaw với tham số tìm kiếm
                    var searchParam = new SqlParameter("@SearchTerm", $"%{searchString}%");
                    students = await _context.Students
                        .FromSqlRaw(@"SELECT StudentId, FirstName, LastName, DateOfBirth, Email 
                                     FROM Students 
                                     WHERE FirstName LIKE @SearchTerm 
                                        OR LastName LIKE @SearchTerm 
                                        OR Email LIKE @SearchTerm 
                                     ORDER BY LastName, FirstName", searchParam)
                        .ToListAsync();
                    
                    ViewBag.SearchString = searchString;
                }
                else
                {
                    // Sử dụng FromSqlRaw để lấy danh sách sinh viên
                    students = await _context.Students
                        .FromSqlRaw("SELECT StudentId, FirstName, LastName, DateOfBirth, Email FROM Students ORDER BY LastName, FirstName")
                        .ToListAsync();
                }
                
                return View(students);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi tải danh sách sinh viên: {ex.Message}";
                return View(new List<Student>());
            }
        }

        // GET: Students/Details/5
        // Xem chi tiết sinh viên theo ID sử dụng FromSqlInterpolated
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                TempData["Error"] = "ID sinh viên không hợp lệ";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                // Sử dụng FromSqlInterpolated với tham số an toàn (tránh SQL Injection)
                var student = await _context.Students
                    .FromSqlInterpolated($"SELECT StudentId, FirstName, LastName, DateOfBirth, Email FROM Students WHERE StudentId = {id}")
                    .FirstOrDefaultAsync();

                if (student == null)
                {
                    TempData["Error"] = "Không tìm thấy sinh viên";
                    return RedirectToAction(nameof(Index));
                }

                return View(student);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi tải thông tin sinh viên: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Students/Create
        // Hiển thị form thêm mới sinh viên
        public IActionResult Create()
        {
            return View();
        }

        // POST: Students/Create
        // Thêm mới sinh viên sử dụng ExecuteSqlRaw với SqlParameter
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FirstName,LastName,DateOfBirth,Email")] Student student)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Sử dụng SqlParameter để tránh SQL Injection
                    var firstNameParam = new SqlParameter("@FirstName", student.FirstName);
                    var lastNameParam = new SqlParameter("@LastName", student.LastName);
                    var dobParam = new SqlParameter("@DateOfBirth", (object?)student.DateOfBirth ?? DBNull.Value);
                    var emailParam = new SqlParameter("@Email", (object?)student.Email ?? DBNull.Value);

                    // Thực thi INSERT sử dụng ExecuteSqlRaw
                    await _context.Database.ExecuteSqlRawAsync(
                        "INSERT INTO Students (FirstName, LastName, DateOfBirth, Email) VALUES (@FirstName, @LastName, @DateOfBirth, @Email)",
                        firstNameParam, lastNameParam, dobParam, emailParam);

                    TempData["Success"] = "Thêm sinh viên thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"Lỗi khi thêm sinh viên: {ex.Message}";
                }
            }
            return View(student);
        }

        // GET: Students/Edit/5
        // Hiển thị form chỉnh sửa sinh viên
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                TempData["Error"] = "ID sinh viên không hợp lệ";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                // Sử dụng FromSqlInterpolated để lấy thông tin sinh viên
                var student = await _context.Students
                    .FromSqlInterpolated($"SELECT StudentId, FirstName, LastName, DateOfBirth, Email FROM Students WHERE StudentId = {id}")
                    .FirstOrDefaultAsync();

                if (student == null)
                {
                    TempData["Error"] = "Không tìm thấy sinh viên";
                    return RedirectToAction(nameof(Index));
                }

                return View(student);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi tải thông tin sinh viên: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Students/Edit/5
        // Cập nhật sinh viên sử dụng ExecuteSqlRaw với SqlParameter
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("StudentId,FirstName,LastName,DateOfBirth,Email")] Student student)
        {
            if (id != student.StudentId)
            {
                TempData["Error"] = "ID sinh viên không khớp";
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Sử dụng SqlParameter để tránh SQL Injection
                    var idParam = new SqlParameter("@StudentId", student.StudentId);
                    var firstNameParam = new SqlParameter("@FirstName", student.FirstName);
                    var lastNameParam = new SqlParameter("@LastName", student.LastName);
                    var dobParam = new SqlParameter("@DateOfBirth", (object?)student.DateOfBirth ?? DBNull.Value);
                    var emailParam = new SqlParameter("@Email", (object?)student.Email ?? DBNull.Value);

                    // Thực thi UPDATE sử dụng ExecuteSqlRaw
                    var rowsAffected = await _context.Database.ExecuteSqlRawAsync(
                        "UPDATE Students SET FirstName = @FirstName, LastName = @LastName, DateOfBirth = @DateOfBirth, Email = @Email WHERE StudentId = @StudentId",
                        idParam, firstNameParam, lastNameParam, dobParam, emailParam);

                    if (rowsAffected == 0)
                    {
                        TempData["Error"] = "Không tìm thấy sinh viên để cập nhật";
                        return RedirectToAction(nameof(Index));
                    }

                    TempData["Success"] = "Cập nhật sinh viên thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"Lỗi khi cập nhật sinh viên: {ex.Message}";
                }
            }
            return View(student);
        }

        // GET: Students/Delete/5
        // Hiển thị trang xác nhận xóa sinh viên
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                TempData["Error"] = "ID sinh viên không hợp lệ";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                // Sử dụng FromSqlInterpolated để lấy thông tin sinh viên
                var student = await _context.Students
                    .FromSqlInterpolated($"SELECT StudentId, FirstName, LastName, DateOfBirth, Email FROM Students WHERE StudentId = {id}")
                    .FirstOrDefaultAsync();

                if (student == null)
                {
                    TempData["Error"] = "Không tìm thấy sinh viên";
                    return RedirectToAction(nameof(Index));
                }

                return View(student);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi tải thông tin sinh viên: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Students/Delete/5
        // Xóa sinh viên sử dụng ExecuteSqlInterpolated
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                // Sử dụng ExecuteSqlInterpolated để xóa sinh viên
                var rowsAffected = await _context.Database.ExecuteSqlInterpolatedAsync(
                    $"DELETE FROM Students WHERE StudentId = {id}");

                if (rowsAffected == 0)
                {
                    TempData["Error"] = "Không tìm thấy sinh viên để xóa";
                }
                else
                {
                    TempData["Success"] = "Xóa sinh viên thành công!";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi xóa sinh viên: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
