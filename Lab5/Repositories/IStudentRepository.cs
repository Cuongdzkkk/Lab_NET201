using Lab5.Models;

namespace Lab5.Repositories
{
    public interface IStudentRepository : IRepository<Student>
    {
        Task<IEnumerable<Student>> GetAllWithDetailsAsync();
        Task<Student?> GetByIdWithDetailsAsync(int id);
    }
}
