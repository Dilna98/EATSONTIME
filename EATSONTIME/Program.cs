using EATSONTIME.DATA;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ── Database ──────────────────────────────────────────────────
var connectionString = builder.Configuration.GetConnectionString("constring");
builder.Services.AddDbContext<ApplicationDBContext>(options =>
     options.UseSqlServer(connectionString));

// ── Session ───────────────────────────────────────────────────
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// ── MVC ───────────────────────────────────────────────────────
builder.Services.AddControllersWithViews();

var app = builder.Build();

// ── Pipeline ──────────────────────────────────────────────────
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseRouting();
app.UseSession();
app.UseAuthorization();
app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
