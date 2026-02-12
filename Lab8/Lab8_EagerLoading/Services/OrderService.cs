// Services/OrderService.cs
// Service triển khai EAGER LOADING trong Entity Framework Core
// Eager Loading: Load tất cả dữ liệu liên quan trong 1 query duy nhất

using Lab8_EagerLoading.Data;
using Lab8_EagerLoading.Models;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Lab8_EagerLoading.Services
{
    /// <summary>
    /// Service triển khai các method sử dụng Eager Loading
    /// EAGER LOADING là gì?
    /// - Load tất cả dữ liệu liên quan ngay lập tức trong 1 query
    /// - Sử dụng .Include() và .ThenInclude() 
    /// - Tạo ra 1 câu SQL với JOIN
    /// - Hiệu quả khi biết trước cần dữ liệu gì
    /// </summary>
    public class OrderService : IOrderService
    {
        private readonly RestaurantDbContext _context;
        private readonly ILogger<OrderService> _logger;

        public OrderService(RestaurantDbContext context, ILogger<OrderService> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// EAGER LOADING: Lấy tất cả Customers với Orders và Dishes
        /// 
        /// Giải thích:
        /// 1. .Include(c => c.Orders) - Load collection Orders của mỗi Customer
        /// 2. .ThenInclude(o => o.Dishes) - Load collection Dishes của mỗi Order
        /// 
        /// Kết quả: 1 câu SQL với LEFT JOIN thay vì nhiều query riêng lẻ
        /// </summary>
        public async Task<List<Customer>> GetAllCustomersWithOrdersAndDishesAsync()
        {
            // Đo thời gian thực thi để so sánh performance
            var stopwatch = Stopwatch.StartNew();

            // ========================================
            // EAGER LOADING với Include() và ThenInclude()
            // ========================================
            var customers = await _context.Customers
                .Include(c => c.Orders)           // Load Orders của Customer
                    .ThenInclude(o => o.Dishes)   // Load Dishes của mỗi Order
                .AsNoTracking()                    // Không cần tracking (chỉ đọc)
                .ToListAsync();

            stopwatch.Stop();

            // Log thời gian thực thi và câu SQL được tạo ra
            _logger.LogInformation(
                "[EAGER LOADING] Đã load {CustomerCount} customers với {OrderCount} orders trong {ElapsedMs}ms",
                customers.Count,
                customers.Sum(c => c.Orders.Count),
                stopwatch.ElapsedMilliseconds
            );

            /*
             * CÂU SQL ĐƯỢC TẠO RA (tương đương):
             * 
             * SELECT c.*, o.*, d.*
             * FROM Customers c
             * LEFT JOIN Orders o ON c.CustomerId = o.CustomerId
             * LEFT JOIN Dishes d ON o.OrderId = d.OrderId
             * ORDER BY c.CustomerId, o.OrderId, d.DishId
             * 
             * -> Chỉ 1 query duy nhất, load tất cả dữ liệu cần thiết
             */

            return customers;
        }

        /// <summary>
        /// Lấy 1 Customer với đầy đủ thông tin
        /// </summary>
        public async Task<Customer?> GetCustomerWithOrdersAndDishesAsync(int customerId)
        {
            return await _context.Customers
                .Include(c => c.Orders)
                    .ThenInclude(o => o.Dishes)
                .Where(c => c.CustomerId == customerId)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Lấy tất cả Orders với thông tin chi tiết
        /// Demo Eager Loading từ góc nhìn Order
        /// </summary>
        public async Task<List<Order>> GetAllOrdersWithDetailsAsync()
        {
            return await _context.Orders
                .Include(o => o.Customer)   // Load Customer của Order
                .Include(o => o.Dishes)     // Load Dishes của Order
                .OrderByDescending(o => o.OrderDate)
                .AsNoTracking()
                .ToListAsync();
        }

        /// <summary>
        /// Thực thi Raw SQL để so sánh với Eager Loading
        /// 
        /// LƯU Ý: Raw SQL không tự động map vào navigation properties
        /// Phải xử lý thủ công hoặc sử dụng với các entity đơn giản
        /// </summary>
        public async Task<List<Customer>> GetCustomersWithRawSqlAsync()
        {
            var stopwatch = Stopwatch.StartNew();

            // Raw SQL query - lấy customers cơ bản
            // Không thể dùng Raw SQL để load nested entities trực tiếp
            var customers = await _context.Customers
                .FromSqlRaw(@"
                    SELECT DISTINCT c.*
                    FROM Customers c
                    LEFT JOIN Orders o ON c.CustomerId = o.CustomerId
                    LEFT JOIN Dishes d ON o.OrderId = d.OrderId
                ")
                .AsNoTracking()
                .ToListAsync();

            // Sau đó vẫn phải dùng Include để load relationships
            // Hoặc xử lý thủ công

            stopwatch.Stop();

            _logger.LogInformation(
                "[RAW SQL] Đã load {CustomerCount} customers trong {ElapsedMs}ms",
                customers.Count,
                stopwatch.ElapsedMilliseconds
            );

            /*
             * SO SÁNH EAGER LOADING vs RAW SQL:
             * 
             * EAGER LOADING:
             * + Tự động map vào objects và navigation properties
             * + Type-safe, compile-time checking
             * + Dễ bảo trì, dễ thay đổi
             * - Đôi khi tạo ra SQL không tối ưu
             * 
             * RAW SQL:
             * + Có thể viết SQL tối ưu hơn
             * + Linh hoạt với complex queries
             * - Không tự động map nested objects
             * - Không type-safe, dễ lỗi khi thay đổi schema
             * - Khó bảo trì
             */

            return customers;
        }
    }
}
