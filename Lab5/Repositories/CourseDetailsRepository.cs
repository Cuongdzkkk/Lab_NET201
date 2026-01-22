using Lab5.Data;
using Lab5.Models;
using Microsoft.EntityFrameworkCore;

namespace Lab5.Repositories
{
    public class CourseDetailsRepository : Repository<CourseDetails>, ICourseDetailsRepository
    {
        public CourseDetailsRepository(AppDbContext context) : base(context) { }

        public async Task<CourseDetails?> GetByCourseIdAsync(int courseId)
        {
            return await _dbSet
                .Include(cd => cd.Course)
                .FirstOrDefaultAsync(cd => cd.CourseId == courseId);
        }
    }
}
