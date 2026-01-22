using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Lab4.Data;
using Lab4.Controllers;

namespace Lab4.Controllers;

public class ProductController : Controller
{
    private CompanyDbContext _context;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public ProductController(CompanyDbContext context, IWebHostEnvironment webHostEnvironment)
    {
        _context = context;
        _webHostEnvironment = webHostEnvironment;
    }

    public IActionResult Index(string? searchText, string? quantityRange, string? priceRange)
    {
        // 🔍 START WITH BASE QUERY
        var query = _context.Products.AsQueryable();

        // 🔍 FILTER 1: SMART KEYWORD SEARCH (SPLIT & CONTAINS ALL)
        if (!string.IsNullOrWhiteSpace(searchText))
        {
            var keywords = searchText.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            foreach (var k in keywords)
            {
                query = query.Where(p => p.Name.Contains(k));
            }
            ViewBag.CurrentSearchText = searchText;
        }

        // 🔍 FILTER 2: QUANTITY RANGE (EXTENDED)
        if (!string.IsNullOrEmpty(quantityRange))
        {
            query = quantityRange switch
            {
                "1-10" => query.Where(p => p.Quantity >= 1 && p.Quantity <= 10),
                "10-30" => query.Where(p => p.Quantity > 10 && p.Quantity <= 30),
                "30-40" => query.Where(p => p.Quantity > 30 && p.Quantity <= 40),
                "40+" => query.Where(p => p.Quantity > 40),
                _ => query
            };
            ViewBag.CurrentQuantityRange = quantityRange;
        }

        // 🔍 FILTER 3: PRICE RANGE (EXTENDED)
        if (!string.IsNullOrEmpty(priceRange))
        {
            query = priceRange switch
            {
                "10k-50k" => query.Where(p => p.Price >= 10000 && p.Price <= 50000),
                "50k-500k" => query.Where(p => p.Price > 50000 && p.Price <= 500000), // Updated range
                "500k+" => query.Where(p => p.Price > 500000),
                _ => query
            };
            ViewBag.CurrentPriceRange = priceRange;
        }

        var products = query.ToList();
        return View(products);
    }
    
    // 🛍️ GALLERY ACTION (USER VIEW) - SAME LOGIC AS INDEX
    public IActionResult Gallery(string? searchText, string? quantityRange, string? priceRange)
    {
        // Reuse logic (duplicated for safety/additive rule compliance)
        var query = _context.Products.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchText))
        {
            var keywords = searchText.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            foreach (var k in keywords)
            {
                query = query.Where(p => p.Name.Contains(k));
            }
            ViewBag.CurrentSearchText = searchText;
        }

        if (!string.IsNullOrEmpty(quantityRange))
        {
            query = quantityRange switch
            {
                "1-10" => query.Where(p => p.Quantity >= 1 && p.Quantity <= 10),
                "10-30" => query.Where(p => p.Quantity > 10 && p.Quantity <= 30),
                "30-40" => query.Where(p => p.Quantity > 30 && p.Quantity <= 40),
                "40+" => query.Where(p => p.Quantity > 40),
                _ => query
            };
            ViewBag.CurrentQuantityRange = quantityRange;
        }

        if (!string.IsNullOrEmpty(priceRange))
        {
            query = priceRange switch
            {
                "10k-50k" => query.Where(p => p.Price >= 10000 && p.Price <= 50000),
                "50k-500k" => query.Where(p => p.Price > 50000 && p.Price <= 500000),
                "500k+" => query.Where(p => p.Price > 500000),
                _ => query
            };
            ViewBag.CurrentPriceRange = priceRange;
        }

        var products = query.ToList();
        return View(products);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(CreateProductVM vm)
    {
        string? rawName = null;
        decimal? rawPrice = null;
        int? rawQuantity = null;
        bool? rawStatus = null;

        // ===== 1. PARSE CUSTOM TYPE NẾU CÓ =====
        if (!string.IsNullOrWhiteSpace(vm.RawProduct))
        {
            var parts = vm.RawProduct.Split('-');

            if (parts.Length != 4)
            {
                ModelState.AddModelError("",
                    "Nhập nhanh đúng định dạng: Tên - Giá - Số lượng - Trạng thái");
                return View(vm);
            }

            rawName = parts[0].Trim();
            rawPrice = decimal.TryParse(parts[1], out var p) ? p : null;
            rawQuantity = int.TryParse(parts[2], out var q) ? q : null;
            rawStatus = bool.TryParse(parts[3], out var s) ? s : null;
        }

        // ===== 2. ƯU TIÊN FORM THƯỜNG, FALLBACK CUSTOM =====
        var product = new Product
        {
            Name = !string.IsNullOrWhiteSpace(vm.Name) ? vm.Name : rawName,
            Price = vm.Price ?? rawPrice,
            Quantity = vm.Quantity ?? rawQuantity,
            Status = vm.Status ?? rawStatus
        };

        // ===== 3. VALIDATE SAU KHI GHÉP =====
        if (string.IsNullOrWhiteSpace(product.Name))
            ModelState.AddModelError("Name", "Tên không được để trống");

        if (product.Price == null || product.Price <= 0)
            ModelState.AddModelError("Price", "Giá không hợp lệ");

        if (product.Quantity == null || product.Quantity <= 0)
            ModelState.AddModelError("Quantity", "Số lượng không hợp lệ");

        if (product.Status == null)
            ModelState.AddModelError("Status", "Trạng thái không hợp lệ");

        if (!ModelState.IsValid)
            return View(vm);

        // 📷 ===== 3.5. XỬ LÝ UPLOAD ẢNH (ADDITIVE) =====
        if (vm.ImageFile != null && vm.ImageFile.Length > 0)
        {
            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "products");
            
            // 🛡️ FIX CRITICAL BUG: CHECK IF DIRECTORY EXISTS
            if (!Directory.Exists(uploadsFolder)) 
                Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(vm.ImageFile.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);
            
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                vm.ImageFile.CopyTo(fileStream);
            }
            
            product.ImagePath = "/images/products/" + uniqueFileName;
        }

        // ===== 4. LƯU DB CODE FIRST =====
        _context.Products.Add(product);
        _context.SaveChanges();

        return RedirectToAction("Index");
    }


    public IActionResult Edit(int Id)
    {
        var product = _context.Products.Find(Id);
        if (product == null) return NotFound();

        var vm = new CreateProductVM
        {
            Name = product.Name,
            Price = product.Price,
            Quantity = product.Quantity,
            Status = product.Status,
        };

        ViewBag.ProductId = product.Id;
        ViewBag.CurrentImagePath = product.ImagePath; // 📷 Pass current image to view
        return View(vm);
    }


    [HttpPost]
    public IActionResult Edit(int id, CreateProductVM vm)
    {
        var product = _context.Products.Find(id);
        if (product == null) return NotFound();

        string? rawName = null;
        decimal? rawPrice = null;
        int? rawQuantity = null;
        bool? rawStatus = null;

        // ===== PARSE CUSTOM TYPE =====
        if (!string.IsNullOrWhiteSpace(vm.RawProduct))
        {
            var parts = vm.RawProduct.Split('-');

            if (parts.Length != 4)
            {
                ModelState.AddModelError("", "Định dạng: Tên - Giá - Số lượng - Trạng thái");
                return View(vm);
            }

            rawName = parts[0].Trim();
            rawPrice = decimal.TryParse(parts[1], out var p) ? p : null;
            rawQuantity = int.TryParse(parts[2], out var q) ? q : null;
            rawStatus = bool.TryParse(parts[3], out var s) ? s : null;
        }

        // ===== GHÉP DỮ LIỆU =====
        product.Name = !string.IsNullOrWhiteSpace(vm.Name) ? vm.Name : rawName;
        product.Price = vm.Price ?? rawPrice;
        product.Quantity = vm.Quantity ?? rawQuantity;
        product.Status = vm.Status ?? rawStatus;

        // ===== VALIDATE =====
        if (string.IsNullOrWhiteSpace(product.Name))
            ModelState.AddModelError("Name", "Tên bắt buộc");

        if (product.Price == null || product.Price <= 0)
            ModelState.AddModelError("Price", "Giá không hợp lệ");

        if (product.Quantity == null || product.Quantity <= 0)
            ModelState.AddModelError("Quantity", "Số lượng không hợp lệ");

        if (product.Status == null)
            ModelState.AddModelError("Status", "Trạng thái bắt buộc");

        if (!ModelState.IsValid)
            return View(vm);

        // 📷 ===== XỬ LÝ UPLOAD ẢNH MỚI (NẾU CÓ) =====
        if (vm.ImageFile != null && vm.ImageFile.Length > 0)
        {
            // Xóa ảnh cũ nếu có
            if (!string.IsNullOrEmpty(product.ImagePath))
            {
                var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, product.ImagePath.TrimStart('/'));
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
            }
            
            // Lưu ảnh mới
            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "products");

            // 🛡️ FIX CRITICAL BUG: CHECK IF DIRECTORY EXISTS
            if (!Directory.Exists(uploadsFolder)) 
                Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(vm.ImageFile.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);
            
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                vm.ImageFile.CopyTo(fileStream);
            }
            
            product.ImagePath = "/images/products/" + uniqueFileName;
        }
        // Nếu không upload ảnh mới -> Giữ nguyên ImagePath hiện tại (PRESERVE EXISTING)

        _context.Products.Update(product);
        _context.SaveChanges();

        return RedirectToAction("Index");
    }



    public IActionResult Delete(int id)
    {
        var product = _context.Products.Find(id);
        if (product == null) return NotFound();

        return View(product);
    }

    [HttpPost]
    public IActionResult DeleteConfirmed(int id)
    {
        var product = _context.Products.Find(id);
        if (product == null) return NotFound();

        _context.Products.Remove(product);
        _context.SaveChanges();

        return RedirectToAction("Index");
    }

}