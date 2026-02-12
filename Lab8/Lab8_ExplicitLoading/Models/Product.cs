// Models/Product.cs
// Entity đại diện cho sản phẩm trong cửa hàng
// Quan hệ: Thuộc về 1 Category

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lab8_ExplicitLoading.Models
{
    /// <summary>
    /// Sản phẩm - thuộc về 1 danh mục
    /// </summary>
    public class Product
    {
        // Khóa chính
        [Key]
        public int ProductId { get; set; }

        // Tên sản phẩm
        [Required(ErrorMessage = "Tên sản phẩm là bắt buộc")]
        [StringLength(200, ErrorMessage = "Tên không được quá 200 ký tự")]
        [Display(Name = "Tên sản phẩm")]
        public string Name { get; set; } = string.Empty;

        // Giá sản phẩm
        [Required]
        [Display(Name = "Giá")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        // Số lượng tồn kho
        [Display(Name = "Tồn kho")]
        public int Stock { get; set; }

        // Mô tả sản phẩm
        [Display(Name = "Mô tả")]
        public string? Description { get; set; }

        // Khóa ngoại liên kết với Category
        [Required]
        [Display(Name = "Danh mục")]
        public int CategoryId { get; set; }

        // Navigation property: Danh mục chứa sản phẩm này
        // KHÔNG dùng virtual - sẽ load thủ công với Explicit Loading
        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }
    }
}
