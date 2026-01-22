using Lab5.Models;
using Lab5.Repositories;

namespace Lab5.Services
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _repository;
        private readonly ILogger<StudentService> _logger;

        public StudentService(IStudentRepository repository, ILogger<StudentService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<IEnumerable<Student>> GetAllStudentsAsync()
        {
            try
            {
                return await _repository.GetAllWithDetailsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all students");
                throw;
            }
        }

        public async Task<Student?> GetStudentByIdAsync(int id)
        {
            try
            {
                return await _repository.GetByIdWithDetailsAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving student with ID {StudentId}", id);
                throw;
            }
        }

        public async Task CreateStudentAsync(Student student)
        {
            try
            {
                await _repository.AddAsync(student);
                _logger.LogInformation("Created student with ID {StudentId}", student.StudentId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating student");
                throw;
            }
        }

        public async Task UpdateStudentAsync(Student student)
        {
            try
            {
                await _repository.UpdateAsync(student);
                _logger.LogInformation("Updated student with ID {StudentId}", student.StudentId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating student with ID {StudentId}", student.StudentId);
                throw;
            }
        }

        public async Task DeleteStudentAsync(int id)
        {
            try
            {
                var student = await _repository.GetByIdAsync(id);
                if (student != null)
                {
                    await _repository.DeleteAsync(student);
                    _logger.LogInformation("Deleted student with ID {StudentId}", id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting student with ID {StudentId}", id);
                throw;
            }
        }

        public async Task<bool> StudentExistsAsync(int id)
        {
            return await _repository.ExistsAsync(id);
        }
    }
}
