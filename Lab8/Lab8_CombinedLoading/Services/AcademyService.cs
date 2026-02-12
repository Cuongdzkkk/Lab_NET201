// Services/AcademyService.cs
// Service triển khai TẤT CẢ 3 LOẠI LOADING trong Entity Framework Core
// Bao gồm: Eager Loading, Lazy Loading, Explicit Loading và Raw SQL

using Lab8_CombinedLoading.Data;
using Lab8_CombinedLoading.Models;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Lab8_CombinedLoading.Services
{
    /// <summary>
    /// Service triển khai các method sử dụng tất cả loại loading
    /// 
    /// SO SÁNH 3 LOẠI LOADING:
    /// 
    /// 1. EAGER LOADING (Include)
    ///    - Load tất cả dữ liệu trong 1 query
    ///    - Dùng khi BIẾT TRƯỚC cần dữ liệu nào
    ///    - Tốt cho hiển thị danh sách
    /// 
    /// 2. LAZY LOADING (Virtual + Proxies)
    ///    - Load tự động khi truy cập property
    ///    - Dùng khi KHÔNG CHẮC có cần dữ liệu không
    ///    - Cẩn thận N+1 problem!
    /// 
    /// 3. EXPLICIT LOADING (Entry.Load)
    ///    - Load thủ công, kiểm soát hoàn toàn
    ///    - Dùng khi cần LOAD CÓ ĐIỀU KIỆN
    ///    - Linh hoạt nhất
    /// </summary>
    public class AcademyService : IAcademyService
    {
        private readonly AcademyDbContext _context;
        private readonly ILogger<AcademyService> _logger;

        public AcademyService(AcademyDbContext context, ILogger<AcademyService> logger)
        {
            _context = context;
            _logger = logger;
        }

        // ========================================
        // METHOD 1: EAGER LOADING
        // Load tất cả dữ liệu trong 1 query với .Include()
        // ========================================

        public async Task<List<Student>> GetAllStudentsWithCoursesEagerAsync()
        {
            _logger.LogInformation("=== EAGER LOADING ===");
            _logger.LogInformation("Query với .Include().ThenInclude()");

            // EAGER LOADING: 1 query lấy tất cả
            // Include Enrollments, mỗi Enrollment lại Include Course
            var students = await _context.Students
                .Include(s => s.Enrollments)  // Load Enrollments collection
                    .ThenInclude(e => e.Course)  // Mỗi Enrollment load Course
                .OrderBy(s => s.Name)
                .AsNoTracking()  // Không track changes vì chỉ đọc
                .ToListAsync();

            _logger.LogInformation("Đã load {Count} students với đầy đủ Courses", students.Count);

            /*
            SQL Generated (đại loại):
            SELECT s.*, e.*, c.*
            FROM Students s
            LEFT JOIN Enrollments e ON s.StudentId = e.StudentId
            LEFT JOIN Courses c ON e.CourseId = c.CourseId
            ORDER BY s.Name
            */

            return students;
        }

        // ========================================
        // METHOD 2: LAZY LOADING
        // Dữ liệu được load tự động khi truy cập navigation property
        // ========================================

        public async Task<Student?> GetStudentWithLazyLoadingAsync(int studentId)
        {
            _logger.LogInformation("=== LAZY LOADING ===");

            // Query 1: Chỉ lấy Student
            var student = await _context.Students.FindAsync(studentId);

            // LAZY LOADING: Khi truy cập student.Enrollments
            // EF Core tự động chạy query 2 để lấy Enrollments
            // Rồi khi truy cập enrollment.Course
            // EF Core tự động chạy query 3, 4... để lấy từng Course

            return student;
        }

        public async Task<(Student? Student, List<string> QueryLogs)> 
            GetStudentWithLazyLoadingDemoAsync(int studentId)
        {
            var logs = new List<string>();

            logs.Add("=== DEMO LAZY LOADING ===");
            logs.Add("");
            logs.Add("📌 Lazy Loading yêu cầu:");
            logs.Add("   1. Navigation property phải là 'virtual'");
            logs.Add("   2. Package Microsoft.EntityFrameworkCore.Proxies");
            logs.Add("   3. Config: UseLazyLoadingProxies()");
            logs.Add("");

            // Query 1
            logs.Add("📌 QUERY 1: Lấy Student");
            logs.Add("   SELECT * FROM Students WHERE StudentId = @id");

            var student = await _context.Students.FindAsync(studentId);

            if (student == null)
            {
                logs.Add("   ❌ Không tìm thấy student!");
                return (null, logs);
            }

            logs.Add($"   ✅ Đã lấy: {student.Name}");
            logs.Add("");

            // Lazy Load Enrollments
            logs.Add("📌 QUERY 2: LAZY LOAD Enrollments");
            logs.Add("   Truy cập student.Enrollments.Count...");
            logs.Add("   SELECT * FROM Enrollments WHERE StudentId = @id");

            var enrollmentCount = student.Enrollments.Count;  // ⚡ LAZY LOAD HAPPENS HERE

            logs.Add($"   ✅ Đã load: {enrollmentCount} enrollments");
            logs.Add("");

            // Lazy Load từng Course
            logs.Add("📌 QUERY 3, 4, 5...: LAZY LOAD Course cho từng Enrollment");
            int queryNum = 3;
            foreach (var enrollment in student.Enrollments)
            {
                logs.Add($"   Query {queryNum}: SELECT * FROM Courses WHERE CourseId = @id");
                var courseName = enrollment.Course?.Title;  // ⚡ LAZY LOAD HAPPENS HERE
                logs.Add($"   → Loaded Course: {courseName}");
                queryNum++;
            }

            logs.Add("");
            logs.Add("=== TỔNG KẾT ===");
            logs.Add($"📊 Tổng số queries: {queryNum - 1}");
            logs.Add("⚠️ ĐÂY LÀ VẤN ĐỀ N+1!");
            logs.Add("💡 Giải pháp: Dùng Eager Loading với .Include()");

            return (student, logs);
        }

        // ========================================
        // METHOD 3: EXPLICIT LOADING
        // Load thủ công khi cần thiết
        // ========================================

        public async Task<Student?> GetStudentWithExplicitLoadingAsync(int studentId)
        {
            _logger.LogInformation("=== EXPLICIT LOADING ===");

            // Query 1: Lấy Student
            var student = await _context.Students.FindAsync(studentId);

            if (student == null) return null;

            // Query 2: EXPLICIT LOAD Enrollments
            await _context.Entry(student)
                .Collection(s => s.Enrollments)
                .Query()
                .Include(e => e.Course)  // Có thể kết hợp Include
                .LoadAsync();

            return student;
        }

        // ========================================
        // SO SÁNH EF CORE VS SQL THUẦN
        // ========================================

        public async Task<ComparisonResult> CompareEfCoreVsSqlAsync()
        {
            var result = new ComparisonResult();

            // EF Core Query
            var sw = Stopwatch.StartNew();
            result.EfCoreResults = await GetCourseStatsEfCoreAsync();
            sw.Stop();
            result.EfCoreTimeMs = sw.ElapsedMilliseconds;

            result.EfCoreQuery = @"
_context.Courses
    .Select(c => new CourseStatDto
    {
        CourseId = c.CourseId,
        Title = c.Title,
        Instructor = c.Instructor,
        StudentCount = c.Enrollments.Count,
        AverageGrade = c.Enrollments
            .Where(e => e.Grade.HasValue)
            .Average(e => (decimal?)e.Grade)
    })
    .ToListAsync()";

            // SQL Query
            sw.Restart();
            result.SqlResults = await GetCourseStatsSqlAsync();
            sw.Stop();
            result.SqlTimeMs = sw.ElapsedMilliseconds;

            result.SqlQuery = @"
SELECT 
    c.CourseId,
    c.Title,
    c.Instructor,
    COUNT(e.EnrollmentId) AS StudentCount,
    AVG(e.Grade) AS AverageGrade
FROM Courses c
LEFT JOIN Enrollments e ON c.CourseId = e.CourseId
GROUP BY c.CourseId, c.Title, c.Instructor
ORDER BY c.Title";

            return result;
        }

        public async Task<List<CourseStatDto>> GetCourseStatsEfCoreAsync()
        {
            _logger.LogInformation("=== EF CORE LINQ QUERY ===");

            return await _context.Courses
                .Select(c => new CourseStatDto
                {
                    CourseId = c.CourseId,
                    Title = c.Title,
                    Instructor = c.Instructor,
                    StudentCount = c.Enrollments.Count,
                    AverageGrade = c.Enrollments
                        .Where(e => e.Grade.HasValue)
                        .Average(e => (decimal?)e.Grade)
                })
                .OrderBy(c => c.Title)
                .ToListAsync();
        }

        public async Task<List<CourseStatDto>> GetCourseStatsSqlAsync()
        {
            _logger.LogInformation("=== RAW SQL QUERY ===");

            var sql = @"
                SELECT 
                    c.CourseId,
                    c.Title,
                    c.Instructor,
                    COUNT(e.EnrollmentId) AS StudentCount,
                    AVG(e.Grade) AS AverageGrade
                FROM Courses c
                LEFT JOIN Enrollments e ON c.CourseId = e.CourseId
                GROUP BY c.CourseId, c.Title, c.Instructor
                ORDER BY c.Title";

            return await _context.Database
                .SqlQueryRaw<CourseStatDto>(sql)
                .ToListAsync();
        }

        public async Task<List<Student>> GetAllStudentsLookupAsync()
        {
            // Chỉ lấy Id và Name để fill dropdown
            return await _context.Students
                .Select(s => new Student { StudentId = s.StudentId, Name = s.Name })
                .OrderBy(s => s.Name)
                .ToListAsync();
        }
    }
}
