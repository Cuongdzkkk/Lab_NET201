using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderManagement.Models
{
    /// <summary>
    /// Entity đại diện cho bảng OrderDetails trong database
    /// Mối quan hệ N-1 với Orders
    /// </summary>
    [Table("OrderDetails")]
    public class OrderDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderDetailId { get; set; }

        [Required]
        [Display(Name = "Mã Đơn Hàng")]
        public int OrderId { get; set; }

        [Required(ErrorMessage = "Mã sản phẩm là bắt buộc")]
        [Display(Name = "Mã Sản Phẩm")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Tên sản phẩm là bắt buộc")]
        [StringLength(100, ErrorMessage = "Tên sản phẩm không được vượt quá 100 ký tự")]
        [Display(Name = "Tên Sản Phẩm")]
        public string ProductName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Số lượng là bắt buộc")]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
        [Display(Name = "Số Lượng")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Đơn giá là bắt buộc")]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Đơn giá phải lớn hơn 0")]
        [Display(Name = "Đơn Giá")]
        [DisplayFormat(DataFormatString = "{0:N0} VNĐ")]
        public decimal UnitPrice { get; set; }

        // Computed property: Thành tiền = Số lượng x Đơn giá
        [NotMapped]
        [Display(Name = "Thành Tiền")]
        [DisplayFormat(DataFormatString = "{0:N0} VNĐ")]
        public decimal LineTotal => Quantity * UnitPrice;

        // Navigation property cho quan hệ N-1
        [ForeignKey("OrderId")]
        public virtual Order? Order { get; set; }
    }
}
