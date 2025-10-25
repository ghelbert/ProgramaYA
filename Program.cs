using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProgramaYA.Models;
using ProgramaYA.Areas.Identity.Data;
using ProgramaYA.Services;
using StackExchange.Redis;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
Env.Load();
builder.Configuration
    .AddEnvironmentVariables();
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
try
{
    var redisHost = builder.Configuration["Redis:Host"];
    var redisPort = builder.Configuration.GetValue<int>("Redis:Port");
    var redisUser = builder.Configuration["Redis:User"];
    var redisPassword = builder.Configuration["Redis:Password"];

    var configurationOptions = new ConfigurationOptions
    {
        EndPoints = { { redisHost!, redisPort } },
        User = redisUser,
        Password = redisPassword,
        Ssl = false,
        AbortOnConnectFail = false, // No fallar si Redis no está disponible
        ConnectTimeout = 5000
    };

    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.ConfigurationOptions = configurationOptions;
    });

    builder.Services.AddSingleton<IConnectionMultiplexer>(provider =>
        ConnectionMultiplexer.Connect(configurationOptions));
}
catch (Exception ex)
{
    // Si hay error con Redis, usar cache en memoria como fallback
    builder.Services.AddMemoryCache();
    Console.WriteLine($"Redis no disponible, usando cache en memoria: {ex.Message}");
}

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

// NewsAPI service registration
builder.Services.AddHttpClient<INewsApiService, NewsApiService>(client =>
{
    client.BaseAddress = new Uri("https://newsapi.org/v2/");
    client.Timeout = TimeSpan.FromSeconds(10);
    // NewsAPI requires a User-Agent header identifying the application.
    // Set a simple descriptive User-Agent so requests are accepted.
    client.DefaultRequestHeaders.Add("User-Agent", "ProgramaYA/1.0 (contact: gustavo@programa.ya)");
});

// GetGeoAPI (currency) service registration
builder.Services.AddHttpClient<ICurrencyService, CurrencyService>(client =>
{
    client.BaseAddress = new Uri("https://api.getgeoapi.com/v2/");
    client.Timeout = TimeSpan.FromSeconds(10);
});

// RapidAPI Weatherbit registration
var rapidWeatherKey = builder.Configuration["RapidApi:Weatherbit:ApiKey"];
var rapidWeatherHost = builder.Configuration["RapidApi:Weatherbit:Host"];
builder.Services.AddHttpClient<IWeatherService, WeatherService>(client =>
{
    client.BaseAddress = new Uri("https://weatherbit-v1-mashape.p.rapidapi.com/");
    client.Timeout = TimeSpan.FromSeconds(10);
    if (!string.IsNullOrEmpty(rapidWeatherKey)) client.DefaultRequestHeaders.Add("x-rapidapi-key", rapidWeatherKey);
    if (!string.IsNullOrEmpty(rapidWeatherHost)) client.DefaultRequestHeaders.Add("x-rapidapi-host", rapidWeatherHost);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
   .WithStaticAssets();
var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("USUARIO_1: gustavo@programa.ya" + "\n\t" + "CONTRASEÑA: @Gustavo+123");
logger.LogInformation("USUARIO_2: reinoso@programa.ya" + "\n\t" + "CONTRASEÑA: @Reinoso+123");
app.Run();
