// Program.cs - Cấu hình ứng dụng Bài 4: Combined Loading
// Kết hợp Eager, Lazy và Explicit Loading trong 1 ứng dụng

using Lab8_CombinedLoading.Data;
using Lab8_CombinedLoading.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ========================================
// CẤU HÌNH SERVICES
// ========================================

// Đăng ký DbContext với LAZY LOADING PROXIES
// Cho phép sử dụng cả 3 loại loading:
// - Eager: .Include()
// - Lazy: virtual navigation + Proxies
// - Explicit: Entry().Collection().Load()
builder.Services.AddDbContext<AcademyDbContext>(options =>
    options.UseLazyLoadingProxies()  // Bật Lazy Loading
           .UseSqlServer(
               builder.Configuration.GetConnectionString("DefaultConnection"),
               sqlOptions => sqlOptions.EnableRetryOnFailure()
           )
           // Bật logging SQL queries để theo dõi
           .EnableSensitiveDataLogging(builder.Environment.IsDevelopment())
           .EnableDetailedErrors(builder.Environment.IsDevelopment())
           // Log SQL queries ra console
           .LogTo(Console.WriteLine, new[] { Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.CommandExecuted })
);

// Đăng ký Service với Dependency Injection
builder.Services.AddScoped<IAcademyService, AcademyService>();

// Add MVC services
builder.Services.AddControllersWithViews();

var app = builder.Build();

// ========================================
// TỰ ĐỘNG TẠO DATABASE VÀ SEED DATA
// ========================================
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AcademyDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    try
    {
        logger.LogInformation("Đang kiểm tra và tạo database...");
        context.Database.EnsureCreated();
        logger.LogInformation("Database đã sẵn sàng!");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Lỗi khi tạo database");
    }
}

// ========================================
// CẤU HÌNH MIDDLEWARE PIPELINE
// ========================================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Route mặc định - đi thẳng đến AcademyController
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Academy}/{action=Index}/{id?}");

app.Run();
