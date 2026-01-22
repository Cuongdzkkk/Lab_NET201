using Lab5.Models;

namespace Lab5.Services
{
    public interface INhanVienService
    {
        Task<IEnumerable<NhanVien>> GetAllNhanViensAsync();
        Task<NhanVien?> GetNhanVienByIdAsync(int id);
        Task CreateNhanVienAsync(NhanVien nhanVien);
        Task UpdateNhanVienAsync(NhanVien nhanVien);
        Task DeleteNhanVienAsync(int id);
        Task<bool> NhanVienExistsAsync(int id);
        
        // ThanNhan operations
        Task AddThanNhanAsync(ThanNhan thanNhan);
        Task DeleteThanNhanAsync(int maNv, string tenTn);
    }
}
