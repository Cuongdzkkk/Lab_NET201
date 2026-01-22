using Lab6.Models;
using Lab6.Repositories;

namespace Lab6.Services;

public class CourseService : ICourseService
{
    private readonly ICourseRepository _repository;

    public CourseService(ICourseRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Course>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<Course?> GetByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task AddAsync(Course course)
    {
        await _repository.AddAsync(course);
    }

    public async Task UpdateAsync(Course course)
    {
        await _repository.UpdateAsync(course);
    }

    public async Task DeleteAsync(int id)
    {
        await _repository.DeleteAsync(id);
    }
}
