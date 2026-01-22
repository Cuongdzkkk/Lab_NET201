using Microsoft.AspNetCore.Mvc.ModelBinding;
using Lab5.Models;

namespace Lab5.Binders
{
    public class NhanVienModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            // Try to get custom format value first
            var customValueResult = bindingContext.ValueProvider.GetValue("customNhanVien");
            
            // If custom format provided, parse it
            if (customValueResult != ValueProviderResult.None && !string.IsNullOrEmpty(customValueResult.FirstValue))
            {
                return BindFromCustomFormat(bindingContext, customValueResult.FirstValue);
            }

            // Otherwise, use default model binding (standard form)
            return BindFromForm(bindingContext);
        }

        private Task BindFromCustomFormat(ModelBindingContext bindingContext, string value)
        {
            // Format: "MaNV|HoNV|TenLot|TenNV|Phai|DChi|Luong|NgaySinh|PHG|Ma_NQL"
            // Example: "123|Nguyen|Van|A|Nam|Ha Noi|15000000|1990-01-15|1|NULL"
            
            var parts = value.Split('|');

            if (parts.Length != 10)
            {
                bindingContext.ModelState.TryAddModelError(
                    bindingContext.ModelName, 
                    "Custom format must be 'MaNV|HoNV|TenLot|TenNV|Phai|DChi|Luong|NgaySinh|PHG|Ma_NQL'");
                return Task.CompletedTask;
            }

            try
            {
                var nhanVien = new NhanVien
                {
                    MaNV = int.Parse(parts[0]),
                    HoNV = parts[1],
                    TenLot = parts[2],
                    TenNV = parts[3],
                    Phai = parts[4],
                    DChi = parts[5],
                    Luong = decimal.Parse(parts[6]),
                    NgaySinh = DateTime.Parse(parts[7]),
                    PHG = int.Parse(parts[8]),
                    Ma_NQL = parts[9] == "NULL" ? null : parts[9]
                };

                bindingContext.Result = ModelBindingResult.Success(nhanVien);
            }
            catch (Exception ex)
            {
                bindingContext.ModelState.TryAddModelError(
                    bindingContext.ModelName, 
                    $"Error parsing custom format: {ex.Message}");
            }

            return Task.CompletedTask;
        }

        private Task BindFromForm(ModelBindingContext bindingContext)
        {
            // Get values from standard form fields
            var maNVResult = bindingContext.ValueProvider.GetValue("MaNV");
            var hoNVResult = bindingContext.ValueProvider.GetValue("HoNV");
            var tenLotResult = bindingContext.ValueProvider.GetValue("TenLot");
            var tenNVResult = bindingContext.ValueProvider.GetValue("TenNV");
            var phaiResult = bindingContext.ValueProvider.GetValue("Phai");
            var dChiResult = bindingContext.ValueProvider.GetValue("DChi");
            var luongResult = bindingContext.ValueProvider.GetValue("Luong");
            var ngaySinhResult = bindingContext.ValueProvider.GetValue("NgaySinh");
            var phgResult = bindingContext.ValueProvider.GetValue("PHG");
            var maNQLResult = bindingContext.ValueProvider.GetValue("Ma_NQL");

            if (maNVResult == ValueProviderResult.None)
            {
                return Task.CompletedTask; // No form data provided
            }

            try
            {
                var nhanVien = new NhanVien
                {
                    MaNV = int.Parse(maNVResult.FirstValue ?? "0"),
                    HoNV = hoNVResult.FirstValue,
                    TenLot = tenLotResult.FirstValue,
                    TenNV = tenNVResult.FirstValue,
                    Phai = phaiResult.FirstValue,
                    DChi = dChiResult.FirstValue,
                    Luong = decimal.Parse(luongResult.FirstValue ?? "0"),
                    NgaySinh = DateTime.Parse(ngaySinhResult.FirstValue ?? DateTime.Now.ToString()),
                    PHG = int.Parse(phgResult.FirstValue ?? "0"),
                    Ma_NQL = maNQLResult.FirstValue
                };

                bindingContext.Result = ModelBindingResult.Success(nhanVien);
            }
            catch (Exception ex)
            {
                bindingContext.ModelState.TryAddModelError(
                    bindingContext.ModelName, 
                    $"Error binding from form: {ex.Message}");
            }

            return Task.CompletedTask;
        }
    }
}
