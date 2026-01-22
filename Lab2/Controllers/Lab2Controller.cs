using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Lab2.Data;
using Lab2.Models;
using System.Diagnostics;
using Lab2.Infrastructure.Binders;

namespace Lab2.Controllers
{
    public class Lab2Controller : Controller
    {
        private readonly ApplicationDbContext _context;

        public Lab2Controller(ApplicationDbContext context)
        {
            _context = context;
        }

        // === DASHBOARD ===
        public IActionResult Index()
        {
            return View();
        }

        // === MODULE 1: PRODUCT MANAGEMENT ===
        [HttpGet]
        public async Task<IActionResult> ProductList()
        {
            // Always return full list for management
            var products = await _context.Products.ToListAsync();
            return View(products);
        }

        [HttpGet]
        public async Task<IActionResult> UpdateProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return RedirectToAction(nameof(ProductList));
            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProduct([FromForm] Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Update(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ProductList));
            }
            return View(product);
        }

        // === MODULE 2: SEARCH ENGINE ===
        [HttpGet]
        public async Task<IActionResult> SearchProduct([FromQuery] ProductSearchModel searchModel)
        {
            var query = _context.Products.AsQueryable();

            if (searchModel != null)
            {
                if (!string.IsNullOrEmpty(searchModel.Name))
                {
                    query = query.Where(p => p.Name.Contains(searchModel.Name));
                }
                if (searchModel.MinPrice.HasValue)
                {
                    query = query.Where(p => p.Price >= searchModel.MinPrice.Value);
                }
                if (searchModel.MaxPrice.HasValue)
                {
                    query = query.Where(p => p.Price <= searchModel.MaxPrice.Value);
                }
            }

            var results = await query.ToListAsync();
            ViewBag.SearchModel = searchModel;
            return View(results);
        }

        // === MODULE 3: LOGISTICS FILTER ===
        [HttpGet]
        public async Task<IActionResult> FilterOrders([FromQuery] OrderFilterModel filterModel)
        {
            var query = _context.Orders.Include(o => o.OrderDetails).AsQueryable();

            if (filterModel != null)
            {
                if (filterModel.StartDate.HasValue)
                {
                    query = query.Where(o => o.OrderDate >= filterModel.StartDate.Value);
                }
                if (filterModel.EndDate.HasValue)
                {
                    query = query.Where(o => o.OrderDate <= filterModel.EndDate.Value);
                }
                if (!string.IsNullOrEmpty(filterModel.Status))
                {
                    query = query.Where(o => o.Status == filterModel.Status);
                }
            }

            var results = await query.ToListAsync();
            ViewBag.FilterModel = filterModel;
            return View(results);
        }

        // === MODULE 4: CREATE ORDER ===
        [HttpGet]
        public IActionResult CreateOrder()
        {
            return View(new Order());
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromForm] Order order)
        {
            if (ModelState.IsValid)
            {
                order.OrderDate = DateTime.Now;
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(order);
        }
        // === MODULE 5: CUSTOM BINDER (QUICK IMPORT) ===
        [HttpPost("QuickParse")]
        public async Task<IActionResult> QuickParse([ModelBinder(BinderType = typeof(ProductStringBinder))] Product productData)
        {
            if (ModelState.IsValid)
            {
                _context.Products.Add(productData);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Parsed & Saved via Custom Binder!", product = productData });
            }
            // Return validation errors
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return BadRequest(new { message = "Parsing Failed. Format: Name-Decription-Price", errors = errors });
        }
    }
}
