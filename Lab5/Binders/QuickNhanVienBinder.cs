using Microsoft.AspNetCore.Mvc.ModelBinding;
using Lab5.Models;

namespace Lab5.Binders
{
    public class QuickNhanVienBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            var valueProviderResult = bindingContext.ValueProvider.GetValue("nhanvien");

            if (valueProviderResult == ValueProviderResult.None)
            {
                return Task.CompletedTask;
            }

            bindingContext.ModelState.SetModelValue("nhanvien", valueProviderResult);

            var value = valueProviderResult.FirstValue;

            if (string.IsNullOrEmpty(value))
            {
                return Task.CompletedTask;
            }

            // Format: "HoNV|TenLot|TenNV|Phai|DChi|Luong|NgaySinh|PHG|Ma_NQL"
            var parts = value.Split('|');

            if (parts.Length != 9)
            {
                bindingContext.ModelState.TryAddModelError(
                    "nhanvien", "Format phải là 'HoNV|TenLot|TenNV|Phai|DChi|Luong|NgaySinh|PHG|Ma_NQL'");
                return Task.CompletedTask;
            }

            try
            {
                var nhanVien = new NhanVien
                {
                    HoNV = parts[0].Trim(),
                    TenLot = parts[1].Trim(),
                    TenNV = parts[2].Trim(),
                    Phai = parts[3].Trim(),
                    DChi = parts[4].Trim(),
                    Luong = decimal.Parse(parts[5].Trim()),
                    NgaySinh = DateTime.Parse(parts[6].Trim()),
                    PHG = int.Parse(parts[7].Trim()),
                    Ma_NQL = parts[8].Trim().Equals("NULL", StringComparison.OrdinalIgnoreCase) ? null : parts[8].Trim()
                };

                bindingContext.Result = ModelBindingResult.Success(nhanVien);
            }
            catch (Exception ex)
            {
                bindingContext.ModelState.TryAddModelError(
                    "nhanvien", $"Lỗi parse: {ex.Message}");
            }

            return Task.CompletedTask;
        }
    }
}
