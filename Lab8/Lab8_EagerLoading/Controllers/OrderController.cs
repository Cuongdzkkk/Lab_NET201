// Controllers/OrderController.cs
// Controller xử lý các request liên quan đến đơn hàng
// Demo Eager Loading với Entity Framework Core

using Lab8_EagerLoading.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lab8_EagerLoading.Controllers
{
    /// <summary>
    /// Controller quản lý hiển thị đơn hàng với Eager Loading
    /// </summary>
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<OrderController> _logger;

        public OrderController(IOrderService orderService, ILogger<OrderController> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }

        /// <summary>
        /// Trang chủ - Hiển thị tất cả customers với orders và dishes
        /// Sử dụng EAGER LOADING
        /// </summary>
        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("=== BÀI 1: EAGER LOADING DEMO ===");
            _logger.LogInformation("Đang gọi GetAllCustomersWithOrdersAndDishesAsync()...");

            // Gọi service method sử dụng Eager Loading
            var customers = await _orderService.GetAllCustomersWithOrdersAndDishesAsync();

            _logger.LogInformation("Kết quả: {Count} customers đã được load với tất cả orders và dishes", customers.Count);

            return View(customers);
        }

        /// <summary>
        /// Chi tiết 1 customer với tất cả orders
        /// </summary>
        public async Task<IActionResult> Details(int id)
        {
            var customer = await _orderService.GetCustomerWithOrdersAndDishesAsync(id);
            
            if (customer == null)
            {
                return NotFound("Không tìm thấy khách hàng");
            }

            return View(customer);
        }

        /// <summary>
        /// So sánh Eager Loading vs Raw SQL
        /// </summary>
        public async Task<IActionResult> Compare()
        {
            _logger.LogInformation("=== SO SÁNH EAGER LOADING VS RAW SQL ===");

            // Eager Loading
            var stopwatch1 = System.Diagnostics.Stopwatch.StartNew();
            var eagerResult = await _orderService.GetAllCustomersWithOrdersAndDishesAsync();
            stopwatch1.Stop();

            // Raw SQL
            var stopwatch2 = System.Diagnostics.Stopwatch.StartNew();
            var rawSqlResult = await _orderService.GetCustomersWithRawSqlAsync();
            stopwatch2.Stop();

            ViewBag.EagerTime = stopwatch1.ElapsedMilliseconds;
            ViewBag.RawSqlTime = stopwatch2.ElapsedMilliseconds;
            ViewBag.EagerCount = eagerResult.Count;
            ViewBag.RawSqlCount = rawSqlResult.Count;

            return View(eagerResult);
        }

        /// <summary>
        /// Hiển thị tất cả orders với thông tin chi tiết
        /// </summary>
        public async Task<IActionResult> AllOrders()
        {
            var orders = await _orderService.GetAllOrdersWithDetailsAsync();
            return View(orders);
        }
    }
}
