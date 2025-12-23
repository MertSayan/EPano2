using EPano2.Interfaces;
using EPano2.Manager;
using EPano2.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Video upload için request size limitini kaldır (sınırsız - maksimum değer)
builder.Services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = long.MaxValue; // Maksimum değer (yaklaşık 9 exabytes)
    options.ValueLengthLimit = int.MaxValue;
    options.MultipartHeadersLengthLimit = int.MaxValue;
    options.MultipartBoundaryLengthLimit = int.MaxValue;
});

// Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddHttpClient();
builder.Services.AddMemoryCache();
builder.Services.Configure<EPano2.Models.Options.OpenWeatherOptions>(
    builder.Configuration.GetSection("OpenWeather"));
builder.Services.AddScoped<EPano2.Interfaces.IWeatherService, EPano2.Services.WeatherService>();

builder.Services.AddHttpClient<IAnnouncementService, AnnouncementService>();
builder.Services.AddSingleton<IVideoService, VideoService>(); //scope e �evir db ba�lay�nca

// Cookie Authentication (form tabanlı login için)
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login";
        options.AccessDeniedPath = "/Login";
    });


// Kestrel server için request body size limitini kaldır
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = null; // Sınırsız
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
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
