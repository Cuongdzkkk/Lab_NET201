// Services/StudentService.cs
// Service triển khai LAZY LOADING trong Entity Framework Core
// Lazy Loading: Tự động load dữ liệu liên quan khi truy cập navigation property

using Lab8_LazyLoading.Data;
using Lab8_LazyLoading.Models;
using Microsoft.EntityFrameworkCore;

namespace Lab8_LazyLoading.Services
{
    /// <summary>
    /// Service triển khai các method sử dụng Lazy Loading
    /// 
    /// LAZY LOADING là gì?
    /// - Dữ liệu liên quan được load TỰ ĐỘNG khi truy cập navigation property
    /// - KHÔNG cần dùng .Include()
    /// - Cần:
    ///   1. Cài package Microsoft.EntityFrameworkCore.Proxies
    ///   2. Cấu hình .UseLazyLoadingProxies() trong DbContext
    ///   3. Navigation properties phải là 'virtual'
    /// 
    /// ƯU ĐIỂM:
    /// - Đơn giản, không cần lo về Include
    /// - Chỉ load dữ liệu khi thực sự cần
    /// 
    /// NHƯỢC ĐIỂM:
    /// - N+1 Query Problem: Mỗi lần truy cập property = 1 query mới
    /// - Không kiểm soát được số lượng queries
    /// - Có thể gây performance issues với dữ liệu lớn
    /// </summary>
    public class StudentService : IStudentService
    {
        private readonly SchoolDbContext _context;
        private readonly ILogger<StudentService> _logger;

        public StudentService(SchoolDbContext context, ILogger<StudentService> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Lấy tất cả students - KHÔNG load courses ngay
        /// Courses sẽ được load sau khi truy cập property
        /// </summary>
        public async Task<List<Student>> GetAllStudentsAsync()
        {
            _logger.LogInformation("=== LAZY LOADING DEMO ===");
            _logger.LogInformation("Bước 1: Chỉ load Students (không có Courses)");

            // Query 1: Chỉ lấy Students
            // Courses chưa được load!
            var students = await _context.Students.ToListAsync();

            _logger.LogInformation("Đã load {Count} students", students.Count);
            _logger.LogInformation("Lúc này Courses chưa được load từ database");

            /*
             * QUAN TRỌNG:
             * Khi bạn truy cập student.Courses trong View hoặc code,
             * EF Core sẽ TỰ ĐỘNG chạy thêm query để load Courses
             * 
             * Ví dụ:
             * foreach (var student in students)
             * {
             *     // ĐÂY là lúc query được chạy để load Courses
             *     Console.WriteLine($"Courses: {student.Courses.Count}");
             * }
             * 
             * -> Nếu có 4 students, sẽ có 4 queries riêng biệt để load Courses
             * -> Đây gọi là N+1 Problem
             */

            return students;
        }

        /// <summary>
        /// Lấy 1 student theo ID
        /// </summary>
        public async Task<Student?> GetStudentByIdAsync(int studentId)
        {
            _logger.LogInformation("Lấy student với ID = {Id}", studentId);

            // Query: Chỉ lấy Student, không load Courses
            return await _context.Students
                .FirstOrDefaultAsync(s => s.StudentId == studentId);
        }

        /// <summary>
        /// Demo chi tiết về Lazy Loading
        /// Trả về Student, số courses, và log các queries đã chạy
        /// </summary>
        public async Task<(Student? Student, int CourseCount, List<string> QueryLogs)> 
            GetStudentWithLazyLoadingDemoAsync(int studentId)
        {
            var queryLogs = new List<string>();

            queryLogs.Add("=== BẮT ĐẦU DEMO LAZY LOADING ===");

            // Query 1: Lấy Student
            queryLogs.Add("📌 Query 1: SELECT * FROM Students WHERE StudentId = @id");
            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.StudentId == studentId);

            if (student == null)
            {
                queryLogs.Add("❌ Không tìm thấy student");
                return (null, 0, queryLogs);
            }

            queryLogs.Add($"✅ Đã load student: {student.Name}");
            queryLogs.Add("");
            queryLogs.Add("--- ĐẾN ĐÂY CHƯA CÓ COURSES ---");
            queryLogs.Add("");

            // Truy cập Courses -> Trigger Lazy Loading
            queryLogs.Add("📌 Query 2: SELECT * FROM Courses WHERE StudentId = @id");
            queryLogs.Add("⚡ LAZY LOADING được kích hoạt khi truy cập student.Courses!");
            
            // ĐOẠN NÀY trigger Lazy Loading
            int courseCount = student.Courses.Count;

            queryLogs.Add($"✅ Đã load {courseCount} courses");
            queryLogs.Add("");
            queryLogs.Add("=== TỔNG KẾT ===");
            queryLogs.Add($"📊 Tổng số queries: 2 (1 cho Student + 1 cho Courses)");
            queryLogs.Add("");
            queryLogs.Add("⚠️ N+1 PROBLEM:");
            queryLogs.Add("Nếu load 10 students và truy cập Courses của mỗi student");
            queryLogs.Add("-> Sẽ có 11 queries (1 + 10)");
            queryLogs.Add("-> Gây chậm khi dữ liệu lớn!");

            return (student, courseCount, queryLogs);
        }
    }
}
