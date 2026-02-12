// Services/IStudentService.cs
// Interface định nghĩa các phương thức service cho Student
// Demo Lazy Loading

using Lab8_LazyLoading.Models;

namespace Lab8_LazyLoading.Services
{
    /// <summary>
    /// Interface cho StudentService
    /// </summary>
    public interface IStudentService
    {
        /// <summary>
        /// Lấy tất cả students - KHÔNG load courses ngay
        /// Courses sẽ được load khi truy cập property (Lazy Loading)
        /// </summary>
        Task<List<Student>> GetAllStudentsAsync();

        /// <summary>
        /// Lấy 1 student theo ID
        /// Courses sẽ được load tự động khi truy cập
        /// </summary>
        Task<Student?> GetStudentByIdAsync(int studentId);

        /// <summary>
        /// Demo Lazy Loading: Lấy student và đếm số courses
        /// </summary>
        Task<(Student? Student, int CourseCount, List<string> QueryLogs)> GetStudentWithLazyLoadingDemoAsync(int studentId);
    }
}
