using Lab5.Data;
using Lab5.Models;
using Lab5.Repositories;

namespace Lab5.Services
{
    public class NhanVienService : INhanVienService
    {
        private readonly INhanVienRepository _repository;
        private readonly AppDbContext _context;
        private readonly ILogger<NhanVienService> _logger;

        public NhanVienService(INhanVienRepository repository, AppDbContext context, ILogger<NhanVienService> logger)
        {
            _repository = repository;
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<NhanVien>> GetAllNhanViensAsync()
        {
            try
            {
                return await _repository.GetAllWithRelativesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all Nhan Viens");
                throw;
            }
        }

        public async Task<NhanVien?> GetNhanVienByIdAsync(int id)
        {
            try
            {
                return await _repository.GetByIdWithRelativesAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving Nhan Vien with ID {MaNV}", id);
                throw;
            }
        }

        public async Task CreateNhanVienAsync(NhanVien nhanVien)
        {
            try
            {
                await _repository.AddAsync(nhanVien);
                _logger.LogInformation("Created Nhan Vien with ID {MaNV}", nhanVien.MaNV);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating Nhan Vien");
                throw;
            }
        }

        public async Task UpdateNhanVienAsync(NhanVien nhanVien)
        {
            try
            {
                await _repository.UpdateAsync(nhanVien);
                _logger.LogInformation("Updated Nhan Vien with ID {MaNV}", nhanVien.MaNV);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating Nhan Vien with ID {MaNV}", nhanVien.MaNV);
                throw;
            }
        }

        public async Task DeleteNhanVienAsync(int id)
        {
            try
            {
                var nhanVien = await _repository.GetByIdAsync(id);
                if (nhanVien != null)
                {
                    await _repository.DeleteAsync(nhanVien);
                    _logger.LogInformation("Deleted Nhan Vien with ID {MaNV}", id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting Nhan Vien with ID {MaNV}", id);
                throw;
            }
        }

        public async Task<bool> NhanVienExistsAsync(int id)
        {
            return await _repository.ExistsAsync(id);
        }

        public async Task AddThanNhanAsync(ThanNhan thanNhan)
        {
            try
            {
                _context.ThanNhans.Add(thanNhan);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Added Than Nhan {TenTN} for Nhan Vien {MaNV}", thanNhan.TenTN, thanNhan.MaNV);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding Than Nhan");
                throw;
            }
        }

        public async Task DeleteThanNhanAsync(int maNv, string tenTn)
        {
            try
            {
                var thanNhan = await _context.ThanNhans.FindAsync(maNv, tenTn);
                if (thanNhan != null)
                {
                    _context.ThanNhans.Remove(thanNhan);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Deleted Than Nhan {TenTN} for Nhan Vien {MaNV}", tenTn, maNv);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting Than Nhan");
                throw;
            }
        }
    }
}
