// Models/Dish.cs
// Entity đại diện cho món ăn trong đơn hàng
// Quan hệ: Thuộc về 1 Order

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lab8_EagerLoading.Models
{
    /// <summary>
    /// Món ăn - thuộc về 1 đơn hàng
    /// </summary>
    public class Dish
    {
        // Khóa chính
        [Key]
        public int DishId { get; set; }

        // Tên món ăn
        [Required(ErrorMessage = "Tên món ăn là bắt buộc")]
        [StringLength(200, ErrorMessage = "Tên không được quá 200 ký tự")]
        [Display(Name = "Tên món")]
        public string Name { get; set; } = string.Empty;

        // Giá món ăn
        [Required]
        [Display(Name = "Giá")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        // Số lượng
        [Display(Name = "Số lượng")]
        public int Quantity { get; set; } = 1;

        // Khóa ngoại liên kết với Order
        [Required]
        [Display(Name = "Đơn hàng")]
        public int OrderId { get; set; }

        // Navigation property: Đơn hàng chứa món này
        [ForeignKey("OrderId")]
        public Order? Order { get; set; }
    }
}
