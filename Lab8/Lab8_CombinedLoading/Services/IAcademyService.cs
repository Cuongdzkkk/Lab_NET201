// Services/IAcademyService.cs
// Interface định nghĩa các phương thức service
// Demo tất cả 3 loại loading: Eager, Lazy, Explicit

using Lab8_CombinedLoading.Models;

namespace Lab8_CombinedLoading.Services
{
    /// <summary>
    /// DTO chứa thống kê khóa học
    /// </summary>
    public class CourseStatDto
    {
        public int CourseId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Instructor { get; set; }
        public int StudentCount { get; set; }
        public decimal? AverageGrade { get; set; }
    }

    /// <summary>
    /// DTO kết quả so sánh
    /// </summary>
    public class ComparisonResult
    {
        public List<CourseStatDto> EfCoreResults { get; set; } = new();
        public List<CourseStatDto> SqlResults { get; set; } = new();
        public long EfCoreTimeMs { get; set; }
        public long SqlTimeMs { get; set; }
        public string EfCoreQuery { get; set; } = string.Empty;
        public string SqlQuery { get; set; } = string.Empty;
    }

    /// <summary>
    /// Interface cho AcademyService
    /// </summary>
    public interface IAcademyService
    {
        // ========================================
        // METHOD 1: EAGER LOADING
        // ========================================

        /// <summary>
        /// EAGER LOADING: Lấy tất cả students với courses đã đăng ký
        /// Sử dụng .Include() và .ThenInclude()
        /// </summary>
        Task<List<Student>> GetAllStudentsWithCoursesEagerAsync();

        // ========================================
        // METHOD 2: LAZY LOADING
        // ========================================

        /// <summary>
        /// LAZY LOADING: Lấy 1 student, courses được load tự động khi truy cập
        /// Proxies package tự động xử lý
        /// </summary>
        Task<Student?> GetStudentWithLazyLoadingAsync(int studentId);

        /// <summary>
        /// Demo Lazy Loading với log chi tiết
        /// </summary>
        Task<(Student? Student, List<string> QueryLogs)> GetStudentWithLazyLoadingDemoAsync(int studentId);

        // ========================================
        // METHOD 3: EXPLICIT LOADING
        // ========================================

        /// <summary>
        /// EXPLICIT LOADING: Lấy student rồi load enrollments thủ công
        /// Sử dụng Entry().Collection().Load()
        /// </summary>
        Task<Student?> GetStudentWithExplicitLoadingAsync(int studentId);

        // ========================================
        // SO SÁNH EF CORE VS SQL THUẦN
        // ========================================

        /// <summary>
        /// So sánh kết quả giữa EF Core và SQL thuần
        /// Query: Lấy courses với số học sinh đăng ký
        /// </summary>
        Task<ComparisonResult> CompareEfCoreVsSqlAsync();

        /// <summary>
        /// Lấy tất cả courses với số học sinh (EF Core)
        /// </summary>
        Task<List<CourseStatDto>> GetCourseStatsEfCoreAsync();

        /// <summary>
        /// Lấy danh sách student rút gọn (Id, Name) cho dropdown
        /// </summary>
        Task<List<Student>> GetAllStudentsLookupAsync();

        /// <summary>
        /// Lấy tất cả courses với số học sinh (Raw SQL)
        /// </summary>
        Task<List<CourseStatDto>> GetCourseStatsSqlAsync();
    }
}
