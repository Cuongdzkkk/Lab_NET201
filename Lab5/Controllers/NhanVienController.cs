using Lab5.Models;
using Lab5.Services;
using Lab5.Binders;
using Microsoft.AspNetCore.Mvc;

namespace Lab5.Controllers
{
    public class NhanVienController : Controller
    {
        private readonly INhanVienService _nhanVienService;
        private readonly ILogger<NhanVienController> _logger;

        public NhanVienController(INhanVienService nhanVienService, ILogger<NhanVienController> logger)
        {
            _nhanVienService = nhanVienService;
            _logger = logger;
        }

        // GET: NhanVien
        public async Task<IActionResult> Index()
        {
            var nhanViens = await _nhanVienService.GetAllNhanViensAsync();
            return View(nhanViens);
        }

        // GET: NhanVien/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nhanVien = await _nhanVienService.GetNhanVienByIdAsync(id.Value);
            if (nhanVien == null)
            {
                return NotFound();
            }

            return View(nhanVien);
        }

        // GET: NhanVien/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: NhanVien/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NhanVien nhanVien)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                                              .ToList();
                ViewBag.Errors = errors;
                return View(nhanVien);
            }

            try
            {
                await _nhanVienService.CreateNhanVienAsync(nhanVien);
                TempData["Success"] = "Thêm nhân viên thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Errors = new List<string> { ex.InnerException?.Message ?? ex.Message };
                return View(nhanVien);
            }
        }

        // GET: NhanVien/QuickCreate
        public IActionResult QuickCreate()
        {
            return View();
        }

        // POST: NhanVien/QuickCreate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> QuickCreate([ModelBinder(typeof(QuickNhanVienBinder))] NhanVien nhanVien)
        {
            if (nhanVien == null)
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
                return View(nhanVien);
            }

            try
            {
                await _nhanVienService.CreateNhanVienAsync(nhanVien);
                TempData["Success"] = "Thêm nhân viên thành công từ Quick Create!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Errors = new List<string> { ex.InnerException?.Message ?? ex.Message };
                return View(nhanVien);
            }
        }


        // GET: NhanVien/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nhanVien = await _nhanVienService.GetNhanVienByIdAsync(id.Value);
            if (nhanVien == null)
            {
                return NotFound();
            }
            return View(nhanVien);
        }

        // POST: NhanVien/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, NhanVien nhanVien)
        {
            if (id != nhanVien.MaNV)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                                              .ToList();
                ViewBag.Errors = errors;
                return View(nhanVien);
            }

            try
            {
                await _nhanVienService.UpdateNhanVienAsync(nhanVien);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                if (!await _nhanVienService.NhanVienExistsAsync(id))
                {
                    return NotFound();
                }

                ViewBag.Errors = new List<string> { ex.InnerException?.Message ?? ex.Message };
                return View(nhanVien);
            }
        }

        // GET: NhanVien/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nhanVien = await _nhanVienService.GetNhanVienByIdAsync(id.Value);
            if (nhanVien == null)
            {
                return NotFound();
            }

            return View(nhanVien);
        }

        // POST: NhanVien/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _nhanVienService.DeleteNhanVienAsync(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting NhanVien {MaNV}", id);
                TempData["Error"] = "Error deleting NhanVien: " + (ex.InnerException?.Message ?? ex.Message);
                return RedirectToAction(nameof(Index));
            }
        }

        // ThanNhan Management
        public IActionResult AddRelative(int id)
        {
            ViewBag.MaNV = id;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddRelative(ThanNhan thanNhan)
        {
            if (!ModelState.IsValid)
            {
                 var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                                              .ToList();
                ViewBag.Errors = errors;
                ViewBag.MaNV = thanNhan.MaNV;
                return View(thanNhan);
            }

            try
            {
                await _nhanVienService.AddThanNhanAsync(thanNhan);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Errors = new List<string> { ex.InnerException?.Message ?? ex.Message };
                ViewBag.MaNV = thanNhan.MaNV;
                return View(thanNhan);
            }
        }

        public async Task<IActionResult> DeleteRelative(int maNv, string tenTn)
        {
            try
            {
                await _nhanVienService.DeleteThanNhanAsync(maNv, tenTn);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting ThanNhan");
                TempData["Error"] = "Error deleting relative";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
