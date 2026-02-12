using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderManagement.Models
{
    /// <summary>
    /// Entity đại diện cho bảng Orders trong database
    /// Mối quan hệ 1-N với OrderDetails
    /// </summary>
    [Table("Orders")]
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderId { get; set; }

        [Required(ErrorMessage = "Ngày đặt hàng là bắt buộc")]
        [Display(Name = "Ngày Đặt Hàng")]
        [DataType(DataType.DateTime)]
        public DateTime OrderDate { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Tên khách hàng là bắt buộc")]
        [StringLength(100, ErrorMessage = "Tên khách hàng không được vượt quá 100 ký tự")]
        [Display(Name = "Tên Khách Hàng")]
        public string CustomerName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tổng tiền là bắt buộc")]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Tổng Tiền")]
        [DisplayFormat(DataFormatString = "{0:N0} VNĐ")]
        public decimal TotalAmount { get; set; }

        // Navigation property cho quan hệ 1-N
        public virtual ICollection<OrderDetail>? OrderDetails { get; set; }
    }
}
