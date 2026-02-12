// Program.cs - Cấu hình ứng dụng Bài 2: Lazy Loading
// QUAN TRỌNG: Cấu hình UseLazyLoadingProxies() để bật Lazy Loading

using Lab8_LazyLoading.Data;
using Lab8_LazyLoading.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ========================================
// CẤU HÌNH SERVICES
// ========================================

// Đăng ký DbContext với LAZY LOADING PROXIES
// QUAN TRỌNG: UseLazyLoadingProxies() là key để bật Lazy Loading
builder.Services.AddDbContext<SchoolDbContext>(options =>
    options.UseLazyLoadingProxies()  // ⚡ BẬT LAZY LOADING
           .UseSqlServer(
               builder.Configuration.GetConnectionString("DefaultConnection"),
               sqlOptions => sqlOptions.EnableRetryOnFailure()
           )
           // Bật logging SQL queries để thấy Lazy Loading hoạt động
           .EnableSensitiveDataLogging(builder.Environment.IsDevelopment())
           .EnableDetailedErrors(builder.Environment.IsDevelopment())
           // Log SQL queries ra console
           .LogTo(Console.WriteLine, new[] { Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.CommandExecuted })
);

// Đăng ký Service với Dependency Injection
builder.Services.AddScoped<IStudentService, StudentService>();

// Add MVC services
builder.Services.AddControllersWithViews();

var app = builder.Build();

// ========================================
// TỰ ĐỘNG TẠO DATABASE VÀ SEED DATA
// ========================================
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<SchoolDbContext>();
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

// Route mặc định - đi thẳng đến StudentController
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Student}/{action=Index}/{id?}");

app.Run();
