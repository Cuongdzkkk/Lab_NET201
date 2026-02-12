// Services/ProductService.cs
// Service triển khai EXPLICIT LOADING trong Entity Framework Core
// Explicit Loading: Load dữ liệu liên quan một cách thủ công, có kiểm soát

using Lab8_ExplicitLoading.Data;
using Lab8_ExplicitLoading.Models;
using Microsoft.EntityFrameworkCore;

namespace Lab8_ExplicitLoading.Services
{
    /// <summary>
    /// Service triển khai các method sử dụng Explicit Loading
    /// 
    /// EXPLICIT LOADING là gì?
    /// - Load dữ liệu liên quan một cách THỦ CÔNG khi cần
    /// - Sử dụng .Entry().Reference().Load() cho single navigation (1-1, N-1)
    /// - Sử dụng .Entry().Collection().Load() cho collection navigation (1-N)
    /// - Bạn kiểm soát hoàn toàn KHI NÀO dữ liệu được load
    /// 
    /// ƯU ĐIỂM:
    /// - Kiểm soát hoàn toàn thời điểm load
    /// - Có thể load có điều kiện (chỉ load khi cần)
    /// - Không bị N+1 tự động như Lazy Loading
    /// 
    /// NHƯỢC ĐIỂM:
    /// - Phải viết code thủ công cho mỗi relationship
    /// - Phức tạp hơn Eager và Lazy Loading
    /// </summary>
    public class ProductService : IProductService
    {
        private readonly StoreDbContext _context;
        private readonly ILogger<ProductService> _logger;

        public ProductService(StoreDbContext context, ILogger<ProductService> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Lấy tất cả products - KHÔNG load category
        /// </summary>
        public async Task<List<Product>> GetAllProductsAsync()
        {
            _logger.LogInformation("Lấy tất cả products (không load Category)");

            // Chỉ lấy Products, Category = null
            return await _context.Products.ToListAsync();
        }

        /// <summary>
        /// EXPLICIT LOADING: Load Category cho 1 Product
        /// 
        /// Sử dụng Entry().Reference().Load() cho single navigation property
        /// Reference() dùng cho quan hệ N-1 hoặc 1-1
        /// </summary>
        public async Task<Product?> GetProductWithCategoryExplicitAsync(int productId)
        {
            _logger.LogInformation("=== EXPLICIT LOADING - Reference ===");

            // Bước 1: Lấy Product (Category chưa được load)
            _logger.LogInformation("Query 1: Lấy Product với ID = {Id}", productId);
            var product = await _context.Products.FindAsync(productId);

            if (product == null)
            {
                return null;
            }

            _logger.LogInformation("Đã lấy product: {Name}, Category hiện tại = {Cat}", 
                product.Name, 
                product.Category?.CategoryName ?? "NULL (chưa load)");

            // Bước 2: EXPLICIT LOADING - Load Category thủ công
            _logger.LogInformation("Query 2: EXPLICIT LOAD - Load Category cho Product");

            // Entry() lấy tracking entry của entity
            // Reference() chỉ định navigation property (single entity, không phải collection)
            // LoadAsync() thực thi query để load dữ liệu
            await _context.Entry(product)
                .Reference(p => p.Category)
                .LoadAsync();

            _logger.LogInformation("Đã load Category: {Cat}", product.Category?.CategoryName);

            return product;
        }

        /// <summary>
        /// EXPLICIT LOADING: Load Category cho tất cả Products
        /// </summary>
        public async Task<List<Product>> GetAllProductsWithExplicitCategoryAsync()
        {
            _logger.LogInformation("=== EXPLICIT LOADING - Nhiều entities ===");

            // Bước 1: Lấy tất cả Products
            var products = await _context.Products.ToListAsync();
            _logger.LogInformation("Đã lấy {Count} products", products.Count);

            // Bước 2: Load Category cho từng Product
            foreach (var product in products)
            {
                // Kiểm tra xem Category đã được load chưa
                if (!_context.Entry(product).Reference(p => p.Category).IsLoaded)
                {
                    // Load thủ công
                    await _context.Entry(product)
                        .Reference(p => p.Category)
                        .LoadAsync();
                }
            }

            _logger.LogInformation("Đã load Category cho tất cả products");

            return products;
        }

        /// <summary>
        /// EXPLICIT LOADING: Load Products collection cho Category
        /// 
        /// Sử dụng Entry().Collection().Load() cho collection navigation
        /// Collection() dùng cho quan hệ 1-N
        /// </summary>
        public async Task<Category?> GetCategoryWithProductsExplicitAsync(int categoryId)
        {
            _logger.LogInformation("=== EXPLICIT LOADING - Collection ===");

            // Bước 1: Lấy Category (Products chưa được load)
            var category = await _context.Categories.FindAsync(categoryId);

            if (category == null)
            {
                return null;
            }

            _logger.LogInformation("Đã lấy category: {Name}", category.CategoryName);

            // Bước 2: EXPLICIT LOADING - Load Products thủ công
            _logger.LogInformation("EXPLICIT LOAD - Load Products collection");

            // Collection() dùng cho collection navigation property
            await _context.Entry(category)
                .Collection(c => c.Products)
                .LoadAsync();

            _logger.LogInformation("Đã load {Count} products cho category", category.Products.Count);

            return category;
        }

        /// <summary>
        /// Demo chi tiết Explicit Loading với log từng bước
        /// </summary>
        public async Task<(Product? Product, List<string> QueryLogs)> 
            GetProductWithExplicitLoadingDemoAsync(int productId)
        {
            var logs = new List<string>();

            logs.Add("=== BẮT ĐẦU DEMO EXPLICIT LOADING ===");
            logs.Add("");

            // Query 1: Lấy Product
            logs.Add("📌 BƯỚC 1: Lấy Product từ database");
            logs.Add("   Query: SELECT * FROM Products WHERE ProductId = @id");

            var product = await _context.Products.FindAsync(productId);

            if (product == null)
            {
                logs.Add("   ❌ Không tìm thấy product!");
                return (null, logs);
            }

            logs.Add($"   ✅ Đã lấy: {product.Name}");
            logs.Add($"   📝 Category lúc này: {(product.Category == null ? "NULL (chưa load)" : product.Category.CategoryName)}");
            logs.Add("");

            // Kiểm tra trạng thái
            logs.Add("📌 BƯỚC 2: Kiểm tra trạng thái loading");
            var isLoaded = _context.Entry(product).Reference(p => p.Category).IsLoaded;
            logs.Add($"   Category IsLoaded: {isLoaded}");
            logs.Add("");

            // Explicit Load
            logs.Add("📌 BƯỚC 3: EXPLICIT LOADING - Load Category thủ công");
            logs.Add("   Code: await _context.Entry(product)");
            logs.Add("            .Reference(p => p.Category)");
            logs.Add("            .LoadAsync();");
            logs.Add("");
            logs.Add("   Query: SELECT * FROM Categories WHERE CategoryId = @id");

            await _context.Entry(product)
                .Reference(p => p.Category)
                .LoadAsync();

            logs.Add($"   ✅ Đã load: {product.Category?.CategoryName}");
            logs.Add("");

            // Kiểm tra lại
            logs.Add("📌 BƯỚC 4: Kiểm tra lại trạng thái");
            isLoaded = _context.Entry(product).Reference(p => p.Category).IsLoaded;
            logs.Add($"   Category IsLoaded: {isLoaded}");
            logs.Add("");

            logs.Add("=== TỔNG KẾT ===");
            logs.Add("📊 Tổng số queries: 2");
            logs.Add("   1. SELECT Product");
            logs.Add("   2. SELECT Category (khi gọi LoadAsync)");
            logs.Add("");
            logs.Add("💡 Ưu điểm Explicit Loading:");
            logs.Add("   - Kiểm soát hoàn toàn thời điểm load");
            logs.Add("   - Có thể kiểm tra IsLoaded trước khi load");
            logs.Add("   - Có thể load có điều kiện");

            return (product, logs);
        }
    }
}
