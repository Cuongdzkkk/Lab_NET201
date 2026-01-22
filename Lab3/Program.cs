using Lab3.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession(); // Enable Session

builder.Services.AddDbContext<InventoryContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("InventoryContext")));

var app = builder.Build();

// Configure the HTTP request pipeline.
// Enable detailed error pages for debugging
app.UseDeveloperExceptionPage();
// Production error handling (commented out for now)
// app.UseExceptionHandler("/Home/Error");
// app.UseHsts();

// app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession(); // Use Session Middleware

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
