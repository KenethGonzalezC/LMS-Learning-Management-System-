using LMS.Data;
using LMS.Services;

//using BitacoraAlfipac.Data.Seed;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ==========================
// KESTREL - ESCUCHAR EN TODA LA RED
// ==========================
builder.WebHost.UseUrls("http://0.0.0.0:5000");

// ==========================
// PDF
// ==========================
QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

// ==========================
// DATABASE
// ==========================
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

// ==========================
// SERVICES
// ==========================
builder.Services.AddScoped<CertificadoService>();

// ==========================
// AUTHENTICATION (Cookies)
// ==========================
//builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
//    .AddCookie(options =>
//    {
//        options.LoginPath = "/Account/Login";
//        options.AccessDeniedPath = "/Account/AccessDenied";
//        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
//        options.SlidingExpiration = true;
//    });

// ==========================
// MVC
// ==========================
builder.Services.AddControllersWithViews();

builder.Services.AddSession();

var app = builder.Build();

// ==========================
// DATABASE MIGRATION + SEED
// ==========================
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    DbInitializer.Seed(context);
}

// ==========================
// MIDDLEWARE PIPELINE
// ==========================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

//app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

// ==========================
// ROUTING
// ==========================
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();