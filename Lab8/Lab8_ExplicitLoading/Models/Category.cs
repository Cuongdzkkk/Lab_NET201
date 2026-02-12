// Models/Category.cs
// Entity đại diện cho danh mục sản phẩm trong cửa hàng
// Quan hệ: 1 Category có nhiều Products

using System.ComponentModel.DataAnnotations;

namespace Lab8_ExplicitLoading.Models
{
    /// <summary>
    /// Danh mục sản phẩm - có nhiều sản phẩm thuộc về
    /// </summary>
    public class Category
    {
        // Khóa chính
        [Key]
        public int CategoryId { get; set; }

        // Tên danh mục
        [Required(ErrorMessage = "Tên danh mục là bắt buộc")]
        [StringLength(100, ErrorMessage = "Tên không được quá 100 ký tự")]
        [Display(Name = "Tên danh mục")]
        public string CategoryName { get; set; } = string.Empty;

        // Mô tả danh mục
        [Display(Name = "Mô tả")]
        public string? Description { get; set; }

        // Navigation property: Danh sách sản phẩm thuộc danh mục này
        // KHÔNG dùng virtual vì không cần Lazy Loading
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
