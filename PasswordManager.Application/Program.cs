using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PasswordManager.Application.Dialogs;
using PasswordManager.Application.Models;
using PasswordManager.Database;
using PasswordManager.Repositories;
using PasswordManager.Repositories.Contracts;
using PasswordManager.Services;
using PasswordManager.Services.Contracts;

namespace PasswordManager.Application
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var host = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration((context, builder) =>
                {
                    // Add other configuration files...
                    builder.AddJsonFile("appsettings.local.json", optional: true);
                })
                .ConfigureServices((context, services) =>
                {
                    ConfigureServices(context.Configuration, services);
                })
                .ConfigureLogging(logging =>
                {
                    // Add other loggers...
                })
                .Build();

            var services = host.Services;
            var mainForm = services.GetRequiredService<MainForm>();
            System.Windows.Forms.Application.Run(mainForm);
        }


        private static void ConfigureServices(IConfiguration configuration,
            IServiceCollection services)
        {
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: false).Build();
            var dbPath = config.GetValue<string>("AppSettings:DatabasePath");

            services.AddDbContext<PasswordManagerContext>(options => options.UseSqlite(dbPath));
            services.Configure<AppSettings>(configuration.GetSection(nameof(AppSettings)));

            services.AddScoped<IUserRepository, UserRepository>();

            services.AddScoped<ISampleService, SampleService>();
            services.AddScoped<IUserService, UserService>();

            // Auto Mapper Configurations
            var mapperConfig = new MapperConfiguration(mc => { mc.AddProfile(new MappingProfile()); });
            IMapper mapper = mapperConfig.CreateMapper();
            services.AddSingleton(mapper);

            services.AddSingleton<MainForm>();
            services.AddSingleton<SecondForm>();
        }
    }
}