using EPano2.Interfaces;
using EPano2.Manager;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();
builder.Services.AddMemoryCache();
builder.Services.Configure<EPano2.Models.Options.OpenWeatherOptions>(
    builder.Configuration.GetSection("OpenWeather"));
builder.Services.AddScoped<EPano2.Interfaces.IWeatherService, EPano2.Services.WeatherService>();

builder.Services.AddHttpClient<IAnnouncementService, AnnouncementService>();
builder.Services.AddSingleton<IVideoService, VideoService>(); //scope e çevir db baðlayýnca


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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
