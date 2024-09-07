using DataAccess.Data;
using DataAccess.Repository.IRepository;
using DataAccess.Repository;
using DataObject.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Utility;
using DataAccess.Service;
using DataAccess.Service.IService;
using OfficeOpenXml;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddIdentity<Accounts, IdentityRole>(options =>
{
    // Password settings
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;

    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.Zero; // No lockout duration
    options.Lockout.MaxFailedAccessAttempts = int.MaxValue; // Set to a very high number to effectively disable lockout
    options.Lockout.AllowedForNewUsers = false; // Optional: Disable lockout for new users

})
.AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = new Microsoft.AspNetCore.Http.PathString("/Identity/Account/Login");
    options.LogoutPath = new Microsoft.AspNetCore.Http.PathString("/Identity/Account/Logout");
    options.AccessDeniedPath = new Microsoft.AspNetCore.Http.PathString("/Identity/Account/AccessDenied");
});

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Set EPPlus license context to NonCommercial
ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

// Register the ExportService
builder.Services.AddScoped<IOrderExportService, ExportService>();


builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddSingleton<IEmailSender, EmailSender>();

//add realtime signalR
builder.Services.AddSignalR();

builder.Services.AddRazorPages();


var app = builder.Build();

// Call the role seeding method
await SeedRoles(app);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseSession();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapRazorPages();

app.MapHub<ChatHub>("/chathub");
app.Run();

async Task SeedRoles(IHost app)
{
    using (var scope = app.Services.CreateScope())
    {
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        string[] roleNames = { "Staff", "NormalUser" };
        IdentityResult roleResult;

        foreach (var roleName in roleNames)
        {
            var roleExist = await roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
            {
                // Create the roles and seed them to the database
                roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }
    }
}