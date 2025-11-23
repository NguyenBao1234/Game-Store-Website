using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using POWStudio.Models;
using POWStudio.Services;
using POWStudio.Utils;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<IGameService, GameService>();// Configure the HTTP request pipeline.

builder.Services.AddControllersWithViews().AddViewOptions(options =>
    {
        options.HtmlHelperOptions.ClientValidationEnabled = true;
    });


//SetUp Identity _____________________________________
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                       throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<GameStoreDbContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = true;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
        options.Lockout.MaxFailedAccessAttempts = 5;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<GameStoreDbContext>();


builder.Services.AddAuthorizationBuilder().AddPolicy("IsAdmin", policy => policy.RequireRole("Admin"));
//____________________________________

builder.Services.AddMvc();

var app = builder.Build();
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

// app.MapControllerRoute(
//     name: "page",
//     pattern: "{slug}",
//     defaults: new { controller = "Page", action = "Show" });
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapGet("/healthz", () => Results.Ok("Healthy"));

// DbUtils.HardCodeInsertGame();
//DbUtils.HardCodeInsertCategory();
//DbUtils.HardCodeInsertGameCate();
//DbUtils.ChangeGameReleasedDateHardCode();
// DbUtils.HardcodeInsertGameScreenshot();
app.Run();
