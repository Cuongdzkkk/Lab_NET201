using Lab5.Models;

namespace Lab5.Repositories
{
    public interface ICourseRepository : IRepository<Course>
    {
        Task<IEnumerable<Course>> GetAllWithStudentsAsync();
        Task<Course?> GetByIdWithStudentsAsync(int id);
    }
}
