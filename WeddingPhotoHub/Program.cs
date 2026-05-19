using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WeddingPhotoHub.Data;
using WeddingPhotoHub.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.AccessDeniedPath = "/Auth/Login";
        options.ExpireTimeSpan = TimeSpan.FromHours(5);
        options.SlidingExpiration = true;
    });

builder.Services.AddScoped<CloudinaryService>();

builder.Services.AddAuthorization();

var raw = Environment.GetEnvironmentVariable("DATABASE_URL");

if (string.IsNullOrWhiteSpace(raw))
    throw new Exception("DATABASE_URL no configurada");

// Convertir formato Render -> Npgsql
var connectionString = raw.Replace("postgresql://", "Host=")
    .Replace("/", ";Database=")
    .Replace("@", ";Username=")
    .Replace(":", ";Port=")
    .Replace(";", " ")
    .Replace(" ", ";Password=");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 52428800; // 50MB ✔ consistente
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Upload}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    // 🔥 crea automáticamente la base de datos y tablas
    try
    {
        context.Database.Migrate();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Migration error: {ex.Message}");
    }

    if (!context.Users.Any())
    {
        context.Users.Add(new User
        {
            Nombre = "Esposo",
            PasswordHash = WeddingPhotoHub.Hasher.PasswordHasher.HashPassword("admin1234"),
            Role = "Admin"
        });

        context.Users.Add(new User
        {
            Nombre = "Esposa",
            PasswordHash = WeddingPhotoHub.Hasher.PasswordHasher.HashPassword("admin1234"),
            Role = "Admin"
        });

        context.SaveChanges();
    }
}
app.Run();
