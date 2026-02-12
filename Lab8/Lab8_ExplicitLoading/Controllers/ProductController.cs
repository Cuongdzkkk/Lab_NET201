// Controllers/ProductController.cs
// Controller xử lý các request liên quan đến sản phẩm
// Demo Explicit Loading với Entity Framework Core

using Lab8_ExplicitLoading.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lab8_ExplicitLoading.Controllers
{
    /// <summary>
    /// Controller quản lý hiển thị sản phẩm với Explicit Loading
    /// </summary>
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductController> _logger;

        public ProductController(IProductService productService, ILogger<ProductController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        /// <summary>
        /// Trang chủ - Hiển thị tất cả products với Category
        /// Sử dụng EXPLICIT LOADING
        /// </summary>
        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("=== BÀI 3: EXPLICIT LOADING DEMO ===");

            // Lấy products và load Category thủ công
            var products = await _productService.GetAllProductsWithExplicitCategoryAsync();

            return View(products);
        }

        /// <summary>
        /// Chi tiết 1 product - Demo Explicit Loading
        /// </summary>
        public async Task<IActionResult> Details(int id)
        {
            var (product, queryLogs) = 
                await _productService.GetProductWithExplicitLoadingDemoAsync(id);
            
            if (product == null)
            {
                return NotFound("Không tìm thấy sản phẩm");
            }

            ViewBag.QueryLogs = queryLogs;

            return View(product);
        }

        /// <summary>
        /// Hiển thị Category với Products
        /// Demo Explicit Loading cho Collection
        /// </summary>
        public async Task<IActionResult> Category(int id)
        {
            var category = await _productService.GetCategoryWithProductsExplicitAsync(id);
            
            if (category == null)
            {
                return NotFound("Không tìm thấy danh mục");
            }

            return View(category);
        }

        /// <summary>
        /// Demo products không load Category
        /// </summary>
        public async Task<IActionResult> WithoutCategory()
        {
            _logger.LogInformation("Lấy products KHÔNG load Category");

            var products = await _productService.GetAllProductsAsync();

            return View(products);
        }
    }
}
