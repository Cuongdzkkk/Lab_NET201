using Lab5.Models;

namespace Lab5.Repositories
{
    public interface INhanVienRepository : IRepository<NhanVien>
    {
        Task<IEnumerable<NhanVien>> GetAllWithRelativesAsync();
        Task<NhanVien?> GetByIdWithRelativesAsync(int id);
    }
}
