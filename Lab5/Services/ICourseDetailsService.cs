using Lab5.Models;

namespace Lab5.Services
{
    public interface ICourseDetailsService
    {
        Task<IEnumerable<CourseDetails>> GetAllCourseDetailsAsync();
        Task<CourseDetails?> GetCourseDetailsByIdAsync(int id);
        Task<CourseDetails?> GetCourseDetailsByCourseIdAsync(int courseId);
        Task CreateCourseDetailsAsync(CourseDetails courseDetails);
        Task UpdateCourseDetailsAsync(CourseDetails courseDetails);
        Task DeleteCourseDetailsAsync(int id);
        Task<bool> CourseDetailsExistsAsync(int id);
    }
}
