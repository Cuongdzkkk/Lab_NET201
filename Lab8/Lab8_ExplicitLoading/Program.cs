// Program.cs - Cấu hình ứng dụng Bài 3: Explicit Loading
// KHÔNG cần cấu hình đặc biệt - Explicit Loading hoạt động mặc định

using Lab8_ExplicitLoading.Data;
using Lab8_ExplicitLoading.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ========================================
// CẤU HÌNH SERVICES
// ========================================

// Đăng ký DbContext - KHÔNG cần UseLazyLoadingProxies
// Explicit Loading hoạt động mặc định trong EF Core
builder.Services.AddDbContext<StoreDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure()
    )
    // Bật logging SQL queries
    .EnableSensitiveDataLogging(builder.Environment.IsDevelopment())
    .EnableDetailedErrors(builder.Environment.IsDevelopment())
    // Log SQL queries ra console
    .LogTo(Console.WriteLine, new[] { Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.CommandExecuted })
);

// Đăng ký Service với Dependency Injection
builder.Services.AddScoped<IProductService, ProductService>();

// Add MVC services
builder.Services.AddControllersWithViews();

var app = builder.Build();

// ========================================
// TỰ ĐỘNG TẠO DATABASE VÀ SEED DATA
// ========================================
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<StoreDbContext>();
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

// Route mặc định - đi thẳng đến ProductController
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Product}/{action=Index}/{id?}");

app.Run();
