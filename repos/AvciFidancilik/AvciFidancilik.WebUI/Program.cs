
using Eticaret.Data;
using Eticaret.Service;
using Eticaret.Service.Abstract;
using Eticaret.Service.ChatKds.Abstract;
using Eticaret.Service.ChatKds.Abstract; // ITarimChatService burada
using Eticaret.Service.ChatKds.Concrete; // TarimChatService burada
using Eticaret.Service.Concrete;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// ======================
// SERVICES
// ======================

// MVC + API
builder.Services.AddControllersWithViews();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});
builder.Services.AddScoped<IAdminKdsService, AdminKdsService>();
// DATABASE
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("DefaultConnection bulunamadı.");
}

builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseSqlServer(connectionString)
);


// SESSION & MEMORY CACHE
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.Name = "AvciFidancilik.Session";
});

// AUTHENTICATION
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/SignIn";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.LogoutPath = "/Account/Logout";
        options.Cookie.Name = "AvciFidancilik.Auth";
        options.ExpireTimeSpan = TimeSpan.FromHours(2);
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization();
builder.Services.AddScoped<ITarimChatService, TarimChatService>();

// FORM LIMITS
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 50 * 1024 * 1024; // 50 MB
});

// JSON
builder.Services.AddControllers()
    .AddJsonOptions(opt =>
    {
        opt.JsonSerializerOptions.PropertyNamingPolicy = null;
        opt.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

// LOG
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();

// ======================
// MIDDLEWARE
// ======================

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCors("AllowAll");
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

// ======================
// ROUTES
// ======================

// API CONTROLLERS
app.MapControllers();

// ADMIN AREA
app.MapAreaControllerRoute(
    name: "Admin",
    areaName: "admin",
    pattern: "admin/{controller=Main}/{action=Index}/{id?}");

// DEFAULT ROUTE
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// HEALTH / DB TEST
app.MapGet("/api/db-test", async (DatabaseContext db) =>
{
    return Results.Ok(new
    {
        canConnect = await db.Database.CanConnectAsync(),
        database = db.Database.GetDbConnection().Database
    });
});

app.Run();
