using Lab5.Data;
using Lab5.Models;
using Lab5.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Lab5.Services
{
    public class CourseService : ICourseService
    {
        private readonly ICourseRepository _repository;
        private readonly AppDbContext _context;
        private readonly ILogger<CourseService> _logger;

        public CourseService(ICourseRepository repository, AppDbContext context, ILogger<CourseService> logger)
        {
            _repository = repository;
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Course>> GetAllCoursesAsync()
        {
            try
            {
                return await _repository.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all courses");
                throw;
            }
        }

        public async Task<Course?> GetCourseByIdAsync(int id)
        {
            try
            {
                return await _repository.GetByIdWithStudentsAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving course with ID {CourseId}", id);
                throw;
            }
        }

        public async Task CreateCourseAsync(Course course)
        {
            try
            {
                await _repository.AddAsync(course);
                _logger.LogInformation("Created course with ID {CourseId}", course.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating course");
                throw;
            }
        }

        public async Task UpdateCourseAsync(Course course)
        {
            try
            {
                await _repository.UpdateAsync(course);
                _logger.LogInformation("Updated course with ID {CourseId}", course.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating course with ID {CourseId}", course.Id);
                throw;
            }
        }

        public async Task DeleteCourseAsync(int id)
        {
            try
            {
                var course = await _repository.GetByIdAsync(id);
                if (course != null)
                {
                    await _repository.DeleteAsync(course);
                    _logger.LogInformation("Deleted course with ID {CourseId}", id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting course with ID {CourseId}", id);
                throw;
            }
        }

        public async Task<bool> CourseExistsAsync(int id)
        {
            return await _repository.ExistsAsync(id);
        }

        public async Task RegisterStudentToCourseAsync(int studentId, int courseId)
        {
            try
            {
                var existing = await _context.StudentCourses
                    .FirstOrDefaultAsync(sc => sc.StudentId == studentId && sc.CourseId == courseId);

                if (existing == null)
                {
                    var studentCourse = new StudentCourse
                    {
                        StudentId = studentId,
                        CourseId = courseId
                    };
                    _context.StudentCourses.Add(studentCourse);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Registered student {StudentId} to course {CourseId}", studentId, courseId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering student to course");
                throw;
            }
        }
    }
}
