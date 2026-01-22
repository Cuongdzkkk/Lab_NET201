using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Lab3.Models;
using Lab3.Infrastructure;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace Lab3.Controllers
{
    public class ProductController : Controller
    {
        private readonly InventoryContext _context;
        private readonly IWebHostEnvironment _environment;

        public ProductController(InventoryContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // GET: Product/Index (Admin Dashboard)
        public async Task<IActionResult> Index()
        {
            var data = await _context.Products.ToListAsync();
            return View(data);
        }

        // GET: Product/List (Raw Data Sheet)
        public async Task<IActionResult> List()
        {
            var data = await _context.Products.ToListAsync();
            return View(data);
        }

        // Customer Shop View (Lumina)
        public async Task<IActionResult> Shop()
        {
            var data = await _context.Products.ToListAsync();
            return View(data);
        }

        // GET: Product/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var product = await _context.Products.FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null) return NotFound();
            return View(product);
        }

        // GET: Product/Create
        [HttpGet]
        public IActionResult Create()
        {
            // Provide an empty Product instance to avoid null reference errors in the view
            var model = new Product();
            return View(model);
        }

        // POST: Product/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, IFormFile? imageFile)
        {
            if (ModelState.IsValid)
            {
                // Image Handling Safe Block
                try
                {
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                        var folderPath = Path.Combine(_environment.WebRootPath, "images");
                        if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);
                        
                        var filePath = Path.Combine(folderPath, fileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(stream);
                        }
                        product.ImageUrl = "/images/" + fileName;
                    }
                    else if (string.IsNullOrEmpty(product.ImageUrl))
                    {
                        product.ImageUrl = "https://placehold.co/600x600?text=New+Item";
                    }
                }
                catch
                {
                    // Fallback if IO fails
                    product.ImageUrl = "https://placehold.co/600x600?text=Error+Image";
                }

                product.CreatedDate = DateTime.Now;
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // POST: Product/QuickCreate
        [HttpPost]
        public async Task<IActionResult> QuickCreate([ModelBinder(typeof(SmartProductModelBinder))] Product product)
        {
            if (ModelState.IsValid)
            {
                // Ensure defaults
                product.CreatedDate = DateTime.Now;
                if(string.IsNullOrEmpty(product.ImageUrl)) product.ImageUrl = "https://placehold.co/600?text=Quick+Item";
                
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            // Logic to handle failure (e.g. redirect back with error in TempData)
            return RedirectToAction(nameof(Create));
        }

        // GET: Product/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();
            return View(product);
        }

        // POST: Product/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product, IFormFile? imageFile)
        {
            if (id != product.ProductId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // Handle Image Update
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                         var folderPath = Path.Combine(_environment.WebRootPath, "images");
                        if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

                        var filePath = Path.Combine(folderPath, fileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(stream);
                        }
                        product.ImageUrl = "/images/" + fileName;
                    }
                    else 
                    {
                        // Safe Reload of existing ImageUrl if not provided
                        // This prevents nulling out the image on edit if hidden field missing
                        var existing = await _context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.ProductId == id);
                        if (existing != null) 
                        {
                            product.ImageUrl = existing.ImageUrl;
                            if (product.CreatedDate == default) product.CreatedDate = existing.CreatedDate;
                        }
                    }

                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Products.Any(e => e.ProductId == id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Product/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var product = await _context.Products.FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null) return NotFound();
            return View(product);
        }

        // POST: Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
