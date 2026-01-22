using Lab5.Models;

namespace Lab5.Services
{
    public interface ICourseService
    {
        Task<IEnumerable<Course>> GetAllCoursesAsync();
        Task<Course?> GetCourseByIdAsync(int id);
        Task CreateCourseAsync(Course course);
        Task UpdateCourseAsync(Course course);
        Task DeleteCourseAsync(int id);
        Task<bool> CourseExistsAsync(int id);
        Task RegisterStudentToCourseAsync(int studentId, int courseId);
    }
}
