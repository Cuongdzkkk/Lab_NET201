// Services/IOrderService.cs
// Interface định nghĩa các phương thức service cho Order
// Sử dụng Dependency Injection pattern

using Lab8_EagerLoading.Models;

namespace Lab8_EagerLoading.Services
{
    /// <summary>
    /// Interface cho OrderService - định nghĩa các method cần thiết
    /// </summary>
    public interface IOrderService
    {
        /// <summary>
        /// Lấy tất cả khách hàng cùng đơn hàng và món ăn sử dụng EAGER LOADING
        /// Đây là phương thức chính để demo Eager Loading với .Include()
        /// </summary>
        /// <returns>Danh sách Customer với Orders và Dishes đã được load sẵn</returns>
        Task<List<Customer>> GetAllCustomersWithOrdersAndDishesAsync();

        /// <summary>
        /// Lấy 1 khách hàng theo ID với đầy đủ thông tin đơn hàng và món ăn
        /// </summary>
        /// <param name="customerId">ID của khách hàng</param>
        /// <returns>Customer với Orders và Dishes</returns>
        Task<Customer?> GetCustomerWithOrdersAndDishesAsync(int customerId);

        /// <summary>
        /// Lấy tất cả đơn hàng với thông tin khách hàng và món ăn
        /// </summary>
        /// <returns>Danh sách Order với Customer và Dishes</returns>
        Task<List<Order>> GetAllOrdersWithDetailsAsync();

        /// <summary>
        /// Thực thi câu SQL thuần tương đương để so sánh
        /// </summary>
        /// <returns>Kết quả từ Raw SQL query</returns>
        Task<List<Customer>> GetCustomersWithRawSqlAsync();
    }
}
