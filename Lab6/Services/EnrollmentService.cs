using Lab6.Models;
using Lab6.Repositories;

namespace Lab6.Services;

public class EnrollmentService : IEnrollmentService
{
    private readonly IEnrollmentRepository _repository;

    public EnrollmentService(IEnrollmentRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Enrollment>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<Enrollment?> GetByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Enrollment>> GetByStudentIdAsync(int studentId)
    {
        return await _repository.GetByStudentIdAsync(studentId);
    }

    public async Task<IEnumerable<Enrollment>> GetByCourseIdAsync(int courseId)
    {
        return await _repository.GetByCourseIdAsync(courseId);
    }

    public async Task AddAsync(Enrollment enrollment)
    {
        await _repository.AddAsync(enrollment);
    }

    public async Task UpdateAsync(Enrollment enrollment)
    {
        await _repository.UpdateAsync(enrollment);
    }

    public async Task DeleteAsync(int id)
    {
        await _repository.DeleteAsync(id);
    }
}
