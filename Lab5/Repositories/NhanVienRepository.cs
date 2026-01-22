using Lab5.Data;
using Lab5.Models;
using Microsoft.EntityFrameworkCore;

namespace Lab5.Repositories
{
    public class NhanVienRepository : Repository<NhanVien>, INhanVienRepository
    {
        public NhanVienRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<NhanVien>> GetAllWithRelativesAsync()
        {
            return await _dbSet
                .Include(n => n.ThanNhans)
                .ToListAsync();
        }

        public async Task<NhanVien?> GetByIdWithRelativesAsync(int id)
        {
            return await _dbSet
                .Include(n => n.ThanNhans)
                .FirstOrDefaultAsync(n => n.MaNV == id);
        }
    }
}
