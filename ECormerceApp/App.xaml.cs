using DataAccess.Data;
using DataAccess.Repository;
using DataAccess.Repository.IRepository;
using DataObject.Model;
using ECormerceApp.Admin;
using ECormerceApp.NormalUser;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Windows;
using Utility;

namespace ECormerceApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; }
        public IConfiguration Configuration { get; private set; }

        public App()
        {
            // Cấu hình `IConfiguration` để đọc từ `appsettings.json`
            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            Configuration = configurationBuilder.Build();

            // Cấu hình các dịch vụ cho ASP.NET Identity và các dịch vụ cần thiết khác
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            // Tạo ServiceProvider từ các dịch vụ đã cấu hình
            ServiceProvider = serviceCollection.BuildServiceProvider();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Lấy LoginWindow từ ServiceProvider để inject các dependency
            var loginWindow = ServiceProvider.GetRequiredService<LoginWindow>();
            loginWindow.Show();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // Lấy chuỗi kết nối từ cấu hình
            var connectionString = Configuration.GetConnectionString("DefaultConnection")
                                   ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            // Đăng ký DbContext với SQL Server
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            // Đăng ký logging
            services.AddLogging();

            // Đăng ký ASP.NET Identity
            services.AddIdentity<Accounts, IdentityRole>()
                    .AddEntityFrameworkStores<ApplicationDbContext>()
                    .AddDefaultTokenProviders();
            services.AddIdentity<Accounts, IdentityRole>(options =>
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

            });

            // Đăng ký các window
            services.AddTransient<LoginWindow>();
            services.AddTransient<RegisterWindow>();
            services.AddTransient<AdminMainWindow>();
            services.AddTransient<UserMainWindow>();

            // Đăng ký UnitOfWork và các repository
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddSingleton<IConfiguration>(Configuration);
            services.AddTransient<IEmailSender, EmailSender>();
        }
    }
}
