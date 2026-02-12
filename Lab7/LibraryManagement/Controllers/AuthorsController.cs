using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryManagement.Data;
using LibraryManagement.Models;

namespace LibraryManagement.Controllers
{
    /// <summary>
    /// Controller quản lý tác giả sử dụng LINQ to Entities
    /// Đặc biệt: Kiểm tra không cho xóa Author nếu có sách
    /// </summary>
    public class AuthorsController : Controller
    {
        private readonly LibraryContext _context;

        public AuthorsController(LibraryContext context)
        {
            _context = context;
        }

        // GET: Authors
        // Hiển thị danh sách tác giả với đếm số sách và hỗ trợ tìm kiếm
        public async Task<IActionResult> Index(string? searchString)
        {
            try
            {
                // LINQ: Include + Where để lọc theo tên tác giả
                var query = _context.Authors
                    .Include(a => a.Books)
                    .AsQueryable();
                
                if (!string.IsNullOrWhiteSpace(searchString))
                {
                    query = query.Where(a => a.AuthorName.Contains(searchString));
                    ViewBag.SearchString = searchString;
                }
                
                var authors = await query
                    .OrderBy(a => a.AuthorName)
                    .ToListAsync();

                return View(authors);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi tải danh sách tác giả: {ex.Message}";
                return View(new List<Author>());
            }
        }

        // GET: Authors/Details/5
        // Xem chi tiết tác giả + danh sách sách của tác giả
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                TempData["Error"] = "ID tác giả không hợp lệ";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                // LINQ: Include + Where + FirstOrDefault để load Author với Books
                var author = await _context.Authors
                    .Include(a => a.Books)
                    .FirstOrDefaultAsync(a => a.AuthorId == id);

                if (author == null)
                {
                    TempData["Error"] = "Không tìm thấy tác giả";
                    return RedirectToAction(nameof(Index));
                }

                return View(author);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi tải thông tin tác giả: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Authors/Create
        // Hiển thị form thêm tác giả mới
        public IActionResult Create()
        {
            return View();
        }

        // POST: Authors/Create
        // Thêm tác giả mới sử dụng LINQ
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AuthorName")] Author author)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // LINQ: Add + SaveChanges
                    _context.Authors.Add(author);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Thêm tác giả thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"Lỗi khi thêm tác giả: {ex.Message}";
                }
            }
            return View(author);
        }

        // GET: Authors/Edit/5
        // Hiển thị form chỉnh sửa tác giả
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                TempData["Error"] = "ID tác giả không hợp lệ";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                // LINQ: Find để tìm theo Primary Key
                var author = await _context.Authors.FindAsync(id);

                if (author == null)
                {
                    TempData["Error"] = "Không tìm thấy tác giả";
                    return RedirectToAction(nameof(Index));
                }

                return View(author);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi tải thông tin tác giả: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Authors/Edit/5
        // Cập nhật tác giả sử dụng LINQ
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AuthorId,AuthorName")] Author author)
        {
            if (id != author.AuthorId)
            {
                TempData["Error"] = "ID tác giả không khớp";
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // LINQ: Update + SaveChanges
                    _context.Authors.Update(author);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Cập nhật tác giả thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    // LINQ: Any để kiểm tra tồn tại
                    if (!await _context.Authors.AnyAsync(a => a.AuthorId == id))
                    {
                        TempData["Error"] = "Không tìm thấy tác giả để cập nhật";
                        return RedirectToAction(nameof(Index));
                    }
                    throw;
                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"Lỗi khi cập nhật tác giả: {ex.Message}";
                }
            }
            return View(author);
        }

        // GET: Authors/Delete/5
        // Hiển thị trang xác nhận xóa với cảnh báo nếu có sách
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                TempData["Error"] = "ID tác giả không hợp lệ";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                // LINQ: Include để load Books
                var author = await _context.Authors
                    .Include(a => a.Books)
                    .FirstOrDefaultAsync(a => a.AuthorId == id);

                if (author == null)
                {
                    TempData["Error"] = "Không tìm thấy tác giả";
                    return RedirectToAction(nameof(Index));
                }

                // LINQ: Any để kiểm tra có sách không
                if (await _context.Books.AnyAsync(b => b.AuthorId == id))
                {
                    ViewBag.HasBooks = true;
                    ViewBag.BookCount = await _context.Books.CountAsync(b => b.AuthorId == id);
                }

                return View(author);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi tải thông tin tác giả: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Authors/Delete/5
        // Xóa tác giả - KIỂM TRA NẾU CÓ SÁCH THÌ KHÔNG CHO XÓA
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                // LINQ: Any để kiểm tra có sách liên quan không
                var hasBooks = await _context.Books.AnyAsync(b => b.AuthorId == id);
                
                if (hasBooks)
                {
                    // Không cho xóa nếu có sách
                    TempData["Error"] = "Không thể xóa tác giả này vì vẫn còn sách trong hệ thống. Vui lòng xóa hết sách trước!";
                    return RedirectToAction(nameof(Index));
                }

                // LINQ: Find + Remove + SaveChanges
                var author = await _context.Authors.FindAsync(id);

                if (author == null)
                {
                    TempData["Error"] = "Không tìm thấy tác giả để xóa";
                    return RedirectToAction(nameof(Index));
                }

                _context.Authors.Remove(author);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Xóa tác giả thành công!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi xóa tác giả: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Authors/Statistics
        // Hiển thị thống kê tác giả sử dụng Custom Type với LINQ Select
        public async Task<IActionResult> Statistics()
        {
            try
            {
                // LINQ: Select để project sang Custom Type AuthorStatisticsViewModel
                var statistics = await _context.Authors
                    .Include(a => a.Books)
                    .Select(a => new AuthorStatisticsViewModel
                    {
                        AuthorId = a.AuthorId,
                        AuthorName = a.AuthorName,
                        BookCount = a.Books != null ? a.Books.Count : 0,
                        EarliestPublicationYear = a.Books != null && a.Books.Any() 
                            ? a.Books.Min(b => b.PublicationYear) 
                            : null,
                        LatestPublicationYear = a.Books != null && a.Books.Any() 
                            ? a.Books.Max(b => b.PublicationYear) 
                            : null
                    })
                    .OrderByDescending(a => a.BookCount)
                    .ThenBy(a => a.AuthorName)
                    .ToListAsync();

                return View(statistics);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi tải thống kê: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
