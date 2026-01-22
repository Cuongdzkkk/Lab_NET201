using Lab5.Data;
using Lab5.Models;
using Microsoft.EntityFrameworkCore;

namespace Lab5.Repositories
{
    public class StudentRepository : Repository<Student>, IStudentRepository
    {
        public StudentRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<Student>> GetAllWithDetailsAsync()
        {
            return await _dbSet
                .Include(s => s.StudentDetails)
                .Include(s => s.StudentCourses)
                    .ThenInclude(sc => sc.Course)
                .ToListAsync();
        }

        public async Task<Student?> GetByIdWithDetailsAsync(int id)
        {
            return await _dbSet
                .Include(s => s.StudentDetails)
                .Include(s => s.StudentCourses)
                    .ThenInclude(sc => sc.Course)
                .FirstOrDefaultAsync(s => s.StudentId == id);
        }
    }
}
