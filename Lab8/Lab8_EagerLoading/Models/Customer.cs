// Models/Customer.cs
// Entity đại diện cho khách hàng trong hệ thống đặt hàng nhà hàng
// Quan hệ: 1 Customer có nhiều Orders

using System.ComponentModel.DataAnnotations;

namespace Lab8_EagerLoading.Models
{
    /// <summary>
    /// Khách hàng - có thể đặt nhiều đơn hàng
    /// </summary>
    public class Customer
    {
        // Khóa chính
        [Key]
        public int CustomerId { get; set; }

        // Tên khách hàng
        [Required(ErrorMessage = "Tên khách hàng là bắt buộc")]
        [StringLength(100, ErrorMessage = "Tên không được quá 100 ký tự")]
        [Display(Name = "Tên khách hàng")]
        public string Name { get; set; } = string.Empty;

        // Số điện thoại khách hàng
        [Display(Name = "Số điện thoại")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string? Phone { get; set; }

        // Navigation property: Danh sách các đơn hàng của khách hàng
        // Đây là một-nhiều (1-N): 1 Customer có nhiều Orders
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
