using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LibraryManagement.Data;
using LibraryManagement.Models;

namespace LibraryManagement.Controllers
{
    /// <summary>
    /// Controller quản lý sách sử dụng LINQ to Entities
    /// Tất cả operations dùng LINQ (Where, Include, FirstOrDefault, Any, v.v.)
    /// </summary>
    public class BooksController : Controller
    {
        private readonly LibraryContext _context;

        public BooksController(LibraryContext context)
        {
            _context = context;
        }

        // GET: Books
        // Hiển thị danh sách sách với Include để load Author và hỗ trợ tìm kiếm
        public async Task<IActionResult> Index(string? searchString)
        {
            try
            {
                // LINQ: Include để Eager Loading thông tin Author
                var query = _context.Books
                    .Include(b => b.Author)
                    .AsQueryable();
                
                // LINQ: Where để lọc theo điều kiện tìm kiếm
                if (!string.IsNullOrWhiteSpace(searchString))
                {
                    query = query.Where(b => 
                        b.BookTitle.Contains(searchString) || 
                        (b.Author != null && b.Author.AuthorName.Contains(searchString)));
                    
                    ViewBag.SearchString = searchString;
                }
                
                var books = await query
                    .OrderBy(b => b.BookTitle)
                    .ToListAsync();

                return View(books);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi tải danh sách sách: {ex.Message}";
                return View(new List<Book>());
            }
        }

        // GET: Books/Details/5
        // Xem chi tiết sách với Include Author
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                TempData["Error"] = "ID sách không hợp lệ";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                // LINQ: Include + FirstOrDefault để lấy 1 record với relationship
                var book = await _context.Books
                    .Include(b => b.Author)
                    .FirstOrDefaultAsync(b => b.BookId == id);

                if (book == null)
                {
                    TempData["Error"] = "Không tìm thấy sách";
                    return RedirectToAction(nameof(Index));
                }

                return View(book);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi tải thông tin sách: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Books/Create
        // Hiển thị form thêm sách mới với dropdown Authors
        public async Task<IActionResult> Create()
        {
            // LINQ: Select để tạo dropdown list từ Authors
            ViewBag.Authors = new SelectList(
                await _context.Authors.OrderBy(a => a.AuthorName).ToListAsync(),
                "AuthorId",
                "AuthorName"
            );
            return View();
        }

        // POST: Books/Create
        // Thêm sách mới sử dụng LINQ
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BookTitle,PublicationYear,AuthorId")] Book book)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // LINQ: Add và SaveChanges
                    _context.Books.Add(book);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Thêm sách thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"Lỗi khi thêm sách: {ex.Message}";
                }
            }

            // Reload dropdown nếu validation fail
            ViewBag.Authors = new SelectList(
                await _context.Authors.OrderBy(a => a.AuthorName).ToListAsync(),
                "AuthorId",
                "AuthorName",
                book.AuthorId
            );
            return View(book);
        }

        // GET: Books/Edit/5
        // Hiển thị form chỉnh sửa sách
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                TempData["Error"] = "ID sách không hợp lệ";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                // LINQ: Find để tìm theo Primary Key
                var book = await _context.Books.FindAsync(id);

                if (book == null)
                {
                    TempData["Error"] = "Không tìm thấy sách";
                    return RedirectToAction(nameof(Index));
                }

                // Dropdown với selected value
                ViewBag.Authors = new SelectList(
                    await _context.Authors.OrderBy(a => a.AuthorName).ToListAsync(),
                    "AuthorId",
                    "AuthorName",
                    book.AuthorId
                );

                return View(book);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi tải thông tin sách: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Books/Edit/5
        // Cập nhật sách sử dụng LINQ
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BookId,BookTitle,PublicationYear,AuthorId")] Book book)
        {
            if (id != book.BookId)
            {
                TempData["Error"] = "ID sách không khớp";
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // LINQ: Update và SaveChanges
                    _context.Books.Update(book);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Cập nhật sách thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    // LINQ: Any để kiểm tra tồn tại
                    if (!await _context.Books.AnyAsync(b => b.BookId == id))
                    {
                        TempData["Error"] = "Không tìm thấy sách để cập nhật";
                        return RedirectToAction(nameof(Index));
                    }
                    throw;
                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"Lỗi khi cập nhật sách: {ex.Message}";
                }
            }

            ViewBag.Authors = new SelectList(
                await _context.Authors.OrderBy(a => a.AuthorName).ToListAsync(),
                "AuthorId",
                "AuthorName",
                book.AuthorId
            );
            return View(book);
        }

        // GET: Books/Delete/5
        // Hiển thị trang xác nhận xóa
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                TempData["Error"] = "ID sách không hợp lệ";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                // LINQ: Include + FirstOrDefault
                var book = await _context.Books
                    .Include(b => b.Author)
                    .FirstOrDefaultAsync(b => b.BookId == id);

                if (book == null)
                {
                    TempData["Error"] = "Không tìm thấy sách";
                    return RedirectToAction(nameof(Index));
                }

                return View(book);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi tải thông tin sách: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Books/Delete/5
        // Xóa sách
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                // LINQ: Find + Remove + SaveChanges
                var book = await _context.Books.FindAsync(id);

                if (book == null)
                {
                    TempData["Error"] = "Không tìm thấy sách để xóa";
                    return RedirectToAction(nameof(Index));
                }

                _context.Books.Remove(book);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Xóa sách thành công!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi xóa sách: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
