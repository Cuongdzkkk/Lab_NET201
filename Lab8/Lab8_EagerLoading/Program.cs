// Program.cs - Cấu hình ứng dụng Bài 1: Eager Loading
// Bao gồm: DbContext, Services, và tự động tạo database

using Lab8_EagerLoading.Data;
using Lab8_EagerLoading.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ========================================
// CẤU HÌNH SERVICES
// ========================================

// Đăng ký DbContext với SQL Server
// Connection string sử dụng LocalDB (có sẵn với Visual Studio)
builder.Services.AddDbContext<RestaurantDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure()
    )
    // Bật logging SQL queries trong Development
    .EnableSensitiveDataLogging(builder.Environment.IsDevelopment())
    .EnableDetailedErrors(builder.Environment.IsDevelopment())
);

// Đăng ký Service với Dependency Injection
// Scoped: Tạo mới instance cho mỗi HTTP request
builder.Services.AddScoped<IOrderService, OrderService>();

// Add MVC services
builder.Services.AddControllersWithViews();

var app = builder.Build();

// ========================================
// TỰ ĐỘNG TẠO DATABASE VÀ SEED DATA
// ========================================
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<RestaurantDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    try
    {
        // Tạo database nếu chưa tồn tại và apply migrations
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

// Route mặc định - đi thẳng đến OrderController
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Order}/{action=Index}/{id?}");

app.Run();
