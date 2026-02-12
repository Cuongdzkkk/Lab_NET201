using System.ComponentModel.DataAnnotations;

namespace OrderManagement.Models
{
    /// <summary>
    /// ViewModel cho việc tạo đơn hàng mới
    /// Bao gồm thông tin Order và danh sách OrderDetails
    /// </summary>
    public class CreateOrderViewModel
    {
        [Required(ErrorMessage = "Tên khách hàng là bắt buộc")]
        [StringLength(100, ErrorMessage = "Tên khách hàng không được vượt quá 100 ký tự")]
        [Display(Name = "Tên Khách Hàng")]
        public string CustomerName { get; set; } = string.Empty;

        [Display(Name = "Ngày Đặt Hàng")]
        [DataType(DataType.DateTime)]
        public DateTime OrderDate { get; set; } = DateTime.Now;

        // Chi tiết đơn hàng (có thể thêm nhiều sản phẩm)
        public List<OrderDetailItem> Items { get; set; } = new List<OrderDetailItem>();
    }

    /// <summary>
    /// ViewModel cho từng dòng chi tiết đơn hàng
    /// </summary>
    public class OrderDetailItem
    {
        [Required(ErrorMessage = "Mã sản phẩm là bắt buộc")]
        [Display(Name = "Mã SP")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Tên sản phẩm là bắt buộc")]
        [StringLength(100)]
        [Display(Name = "Tên Sản Phẩm")]
        public string ProductName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Số lượng là bắt buộc")]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
        [Display(Name = "Số Lượng")]
        public int Quantity { get; set; } = 1;

        [Required(ErrorMessage = "Đơn giá là bắt buộc")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Đơn giá phải lớn hơn 0")]
        [Display(Name = "Đơn Giá (VNĐ)")]
        public decimal UnitPrice { get; set; }
    }
}
