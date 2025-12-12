using Microsoft.EntityFrameworkCore;
using Project;
using Project.Areas.Admin.Services;
using Project.Data;
using Project.Services;
using Project.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });

// Session configuration
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60); // Сесията е активна 60 минути
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.Name = ".BidMotors.Session";
});

// Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Services with Interfaces
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<PartService>();
builder.Services.AddScoped<CarService>();
builder.Services.AddScoped<RepairService>();
builder.Services.AddScoped<ServiceRequestService>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<ClientService>();
builder.Services.AddScoped<NotificationService>();

// Admin Services
builder.Services.AddScoped<ServiceTypeService>();
builder.Services.AddScoped<EmployeeService>();
builder.Services.AddScoped<AdminPartService>();
builder.Services.AddScoped<AdminSettingsService>();
builder.Services.AddScoped<ServiceService>();

builder.Services.AddSignalR();

var app = builder.Build();

// Seed Admin Account
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    SeedAdmin.Initialize(context);
}

app.MapHub<NotificationHub>("/notifyHub");
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

// Добавяне на Session middleware
app.UseSession();

app.UseAuthorization();

app.MapStaticAssets();

// Map API Controllers
app.MapControllers();

// Admin Area route
app.MapControllerRoute(
    name: "admin",
    pattern: "{area:exists}/{controller=Index}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
