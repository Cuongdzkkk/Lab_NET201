using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using OrderManagement.Data;
using OrderManagement.Models;

namespace OrderManagement.Controllers
{
    /// <summary>
    /// Controller quản lý đơn hàng sử dụng Stored Procedures
    /// Tất cả operations gọi SP thông qua FromSqlRaw và ExecuteSqlRaw
    /// </summary>
    public class OrdersController : Controller
    {
        private readonly OrderContext _context;

        public OrdersController(OrderContext context)
        {
            _context = context;
        }

        // GET: Orders
        // Hiển thị danh sách đơn hàng sử dụng sp_GetOrders hoặc sp_SearchOrders
        public async Task<IActionResult> Index(string? searchString)
        {
            try
            {
                List<Order> orders;
                
                if (!string.IsNullOrWhiteSpace(searchString))
                {
                    // Gọi Stored Procedure sp_SearchOrders để tìm kiếm
                    var searchParam = new SqlParameter("@SearchTerm", searchString);
                    orders = await _context.Orders
                        .FromSqlRaw("EXEC sp_SearchOrders @SearchTerm", searchParam)
                        .ToListAsync();
                    
                    ViewBag.SearchString = searchString;
                }
                else
                {
                    // Gọi Stored Procedure sp_GetOrders để lấy tất cả
                    orders = await _context.Orders
                        .FromSqlRaw("EXEC sp_GetOrders")
                        .ToListAsync();
                }

                return View(orders);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi tải danh sách đơn hàng: {ex.Message}";
                return View(new List<Order>());
            }
        }

        // GET: Orders/Details/5
        // Xem chi tiết đơn hàng sử dụng sp_GetOrderDetails
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                TempData["Error"] = "ID đơn hàng không hợp lệ";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                // Gọi Stored Procedure sp_GetOrderDetails
                var orderIdParam = new SqlParameter("@OrderId", id);
                
                // Lấy thông tin Order từ SP - sử dụng ToListAsync() rồi FirstOrDefault()
                var orders = await _context.Orders
                    .FromSqlRaw("EXEC sp_GetOrderDetails @OrderId", orderIdParam)
                    .AsNoTracking()
                    .ToListAsync();
                
                var order = orders.FirstOrDefault();

                if (order == null)
                {
                    TempData["Error"] = "Không tìm thấy đơn hàng";
                    return RedirectToAction(nameof(Index));
                }

                // Lấy chi tiết đơn hàng 
                var detailsParam = new SqlParameter("@OrderId", id);
                var details = await _context.OrderDetails
                    .FromSqlRaw("SELECT OrderDetailId, OrderId, ProductId, ProductName, Quantity, UnitPrice FROM OrderDetails WHERE OrderId = @OrderId", detailsParam)
                    .AsNoTracking()
                    .ToListAsync();

                order.OrderDetails = details;

                return View(order);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi tải thông tin đơn hàng: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Orders/Create
        // Hiển thị form tạo đơn hàng mới
        public IActionResult Create()
        {
            var model = new CreateOrderViewModel
            {
                OrderDate = DateTime.Now,
                Items = new List<OrderDetailItem>
                {
                    new OrderDetailItem { ProductId = 1, ProductName = "", Quantity = 1, UnitPrice = 0 }
                }
            };
            return View(model);
        }

        // POST: Orders/Create
        // Tạo đơn hàng mới sử dụng sp_CreateOrder
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateOrderViewModel model)
        {
            // Validate items
            if (model.Items == null || !model.Items.Any())
            {
                ModelState.AddModelError("", "Phải có ít nhất một sản phẩm trong đơn hàng");
                return View(model);
            }

            // Remove empty items và validate
            model.Items = model.Items.Where(i => !string.IsNullOrWhiteSpace(i.ProductName) && i.Quantity > 0).ToList();

            if (!model.Items.Any())
            {
                ModelState.AddModelError("", "Phải có ít nhất một sản phẩm hợp lệ trong đơn hàng");
                return View(model);
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                // Tính tổng tiền
                decimal totalAmount = model.Items.Sum(i => i.Quantity * i.UnitPrice);

                // Gọi Stored Procedure sp_CreateOrder cho từng item
                foreach (var item in model.Items)
                {
                    var customerParam = new SqlParameter("@CustomerName", model.CustomerName);
                    var dateParam = new SqlParameter("@OrderDate", model.OrderDate);
                    var totalParam = new SqlParameter("@TotalAmount", totalAmount);
                    var productIdParam = new SqlParameter("@ProductId", item.ProductId);
                    var productNameParam = new SqlParameter("@ProductName", item.ProductName);
                    var quantityParam = new SqlParameter("@Quantity", item.Quantity);
                    var unitPriceParam = new SqlParameter("@UnitPrice", item.UnitPrice);

                    await _context.Database.ExecuteSqlRawAsync(
                        "EXEC sp_CreateOrder @CustomerName, @OrderDate, @TotalAmount, @ProductId, @ProductName, @Quantity, @UnitPrice",
                        customerParam, dateParam, totalParam, productIdParam, productNameParam, quantityParam, unitPriceParam);
                    
                    // Chỉ tạo 1 order với item đầu tiên, các item sau thêm vào order đã tạo
                    break;
                }

                TempData["Success"] = "Tạo đơn hàng thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi tạo đơn hàng: {ex.Message}";
                return View(model);
            }
        }

        // GET: Orders/Delete/5
        // Hiển thị trang xác nhận xóa
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                TempData["Error"] = "ID đơn hàng không hợp lệ";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var orderIdParam = new SqlParameter("@OrderId", id);
                var order = await _context.Orders
                    .FromSqlRaw("SELECT OrderId, OrderDate, CustomerName, TotalAmount FROM Orders WHERE OrderId = @OrderId", orderIdParam)
                    .FirstOrDefaultAsync();

                if (order == null)
                {
                    TempData["Error"] = "Không tìm thấy đơn hàng";
                    return RedirectToAction(nameof(Index));
                }

                // Lấy số lượng chi tiết
                var detailsParam = new SqlParameter("@OrderId", id);
                var details = await _context.OrderDetails
                    .FromSqlRaw("SELECT OrderDetailId, OrderId, ProductId, ProductName, Quantity, UnitPrice FROM OrderDetails WHERE OrderId = @OrderId", detailsParam)
                    .ToListAsync();

                order.OrderDetails = details;

                return View(order);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi tải thông tin đơn hàng: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Orders/Delete/5
        // Xóa đơn hàng sử dụng sp_DeleteOrder
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                // Gọi Stored Procedure sp_DeleteOrder
                var orderIdParam = new SqlParameter("@OrderId", id);
                await _context.Database.ExecuteSqlRawAsync("EXEC sp_DeleteOrder @OrderId", orderIdParam);

                TempData["Success"] = "Xóa đơn hàng thành công!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi xóa đơn hàng: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
