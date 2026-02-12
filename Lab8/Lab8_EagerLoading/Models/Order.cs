// Models/Order.cs
// Entity đại diện cho đơn hàng trong nhà hàng
// Quan hệ: Thuộc về 1 Customer, có nhiều Dishes

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lab8_EagerLoading.Models
{
    /// <summary>
    /// Đơn hàng - thuộc về 1 khách hàng, chứa nhiều món ăn
    /// </summary>
    public class Order
    {
        // Khóa chính
        [Key]
        public int OrderId { get; set; }

        // Khóa ngoại liên kết với Customer
        [Required]
        [Display(Name = "Khách hàng")]
        public int CustomerId { get; set; }

        // Ngày đặt hàng
        [Required]
        [Display(Name = "Ngày đặt hàng")]
        [DataType(DataType.DateTime)]
        public DateTime OrderDate { get; set; } = DateTime.Now;

        // Tổng tiền đơn hàng
        [Display(Name = "Tổng tiền")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        // Trạng thái đơn hàng
        [Display(Name = "Trạng thái")]
        public string Status { get; set; } = "Đang xử lý";

        // Navigation property: Khách hàng đã đặt đơn này
        // Đây là quan hệ ngược (Inverse navigation)
        [ForeignKey("CustomerId")]
        public Customer? Customer { get; set; }

        // Navigation property: Danh sách các món ăn trong đơn hàng
        // Đây là một-nhiều (1-N): 1 Order có nhiều Dishes
        public ICollection<Dish> Dishes { get; set; } = new List<Dish>();
    }
}
