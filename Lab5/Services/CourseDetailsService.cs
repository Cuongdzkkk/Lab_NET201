using Lab5.Models;
using Lab5.Repositories;

namespace Lab5.Services
{
    public class CourseDetailsService : ICourseDetailsService
    {
        private readonly ICourseDetailsRepository _repository;
        private readonly ILogger<CourseDetailsService> _logger;

        public CourseDetailsService(ICourseDetailsRepository repository, ILogger<CourseDetailsService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<IEnumerable<CourseDetails>> GetAllCourseDetailsAsync()
        {
            try
            {
                return await _repository.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all course details");
                throw;
            }
        }

        public async Task<CourseDetails?> GetCourseDetailsByIdAsync(int id)
        {
            try
            {
                return await _repository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving course details with ID {Id}", id);
                throw;
            }
        }

        public async Task<CourseDetails?> GetCourseDetailsByCourseIdAsync(int courseId)
        {
            try
            {
                return await _repository.GetByCourseIdAsync(courseId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving course details for course {CourseId}", courseId);
                throw;
            }
        }

        public async Task CreateCourseDetailsAsync(CourseDetails courseDetails)
        {
            try
            {
                await _repository.AddAsync(courseDetails);
                _logger.LogInformation("Created course details with ID {Id}", courseDetails.CourseDetailsId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating course details");
                throw;
            }
        }

        public async Task UpdateCourseDetailsAsync(CourseDetails courseDetails)
        {
            try
            {
                await _repository.UpdateAsync(courseDetails);
                _logger.LogInformation("Updated course details with ID {Id}", courseDetails.CourseDetailsId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating course details with ID {Id}", courseDetails.CourseDetailsId);
                throw;
            }
        }

        public async Task DeleteCourseDetailsAsync(int id)
        {
            try
            {
                var courseDetails = await _repository.GetByIdAsync(id);
                if (courseDetails != null)
                {
                    await _repository.DeleteAsync(courseDetails);
                    _logger.LogInformation("Deleted course details with ID {Id}", id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting course details with ID {Id}", id);
                throw;
            }
        }

        public async Task<bool> CourseDetailsExistsAsync(int id)
        {
            return await _repository.ExistsAsync(id);
        }
    }
}
