using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Models
{
    /// <summary>
    /// Custom Type: ViewModel để hiển thị thống kê tác giả
    /// Sử dụng LINQ Select để project dữ liệu từ Author entity
    /// </summary>
    public class AuthorStatisticsViewModel
    {
        public int AuthorId { get; set; }
        
        [Display(Name = "Tên Tác Giả")]
        public string AuthorName { get; set; } = string.Empty;
        
        [Display(Name = "Số Sách")]
        public int BookCount { get; set; }
        
        [Display(Name = "Năm XB Sớm Nhất")]
        public int? EarliestPublicationYear { get; set; }
        
        [Display(Name = "Năm XB Mới Nhất")]
        public int? LatestPublicationYear { get; set; }
        
        // Computed property
        [Display(Name = "Trạng Thái")]
        public string Status => BookCount > 0 ? "Có sách" : "Chưa có sách";
    }
    
    /// <summary>
    /// Custom Type: ViewModel để hiển thị báo cáo sách theo năm
    /// Sử dụng LINQ GroupBy + Select để tổng hợp dữ liệu
    /// </summary>
    public class BookYearReportViewModel
    {
        [Display(Name = "Năm Xuất Bản")]
        public int Year { get; set; }
        
        [Display(Name = "Số Lượng Sách")]
        public int BookCount { get; set; }
        
        [Display(Name = "Danh Sách Tác Giả")]
        public List<string> AuthorNames { get; set; } = new();
        
        // Computed property
        public string AuthorList => string.Join(", ", AuthorNames.Distinct());
    }
}
