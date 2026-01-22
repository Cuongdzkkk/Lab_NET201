using Lab5.Models;

namespace Lab5.Repositories
{
    public interface ICourseDetailsRepository : IRepository<CourseDetails>
    {
        Task<CourseDetails?> GetByCourseIdAsync(int courseId);
    }
}
