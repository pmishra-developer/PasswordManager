using AutoMapper;
using Configurator.Application.Dialogs;
using Configurator.Database;
using Configurator.Repositories;
using Configurator.Repositories.Contracts;
using Configurator.Services;
using Configurator.Services.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Configurator.Application
{
    public static class Program
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

            LogWriter.CreateLogFile();
            LogWriter.AddSeperator();
            LogWriter.WriteToLog("iCode BootLoader Configurator Started...");

            var services = host.Services;
            var mainForm = services.GetRequiredService<DeviceTestForm>();
            System.Windows.Forms.Application.Run(mainForm);
        }


        private static void ConfigureServices(IConfiguration configuration,
            IServiceCollection services)
        {
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: false).Build();
            var dbPath = config.GetValue<string>("AppSettings:DatabasePath");

            services.AddDbContext<ConfiguratorContext>(options => options.UseSqlite(dbPath));
            services.Configure<AppSettings>(configuration.GetSection(nameof(AppSettings)));

            // Auto Mapper Configurations
            var mapperConfig = new MapperConfiguration(mc => { mc.AddProfile(new MappingProfile()); });
            IMapper mapper = mapperConfig.CreateMapper();
            services.AddSingleton(mapper);

            // Map your Repositories Here
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IDeviceRepository, DeviceRepository>();
            services.AddScoped<ILookupRepository, LookupRepository>();
            services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
            services.AddScoped<IApplicationRepository, ApplicationRepository>();
                     
            // Map your Services Here
            services.AddScoped<ISampleService, SampleService>();
            services.AddScoped<IUserService, UserService>();

            services.AddScoped<IFunctionApp, FunctionApp>();

            // Register Our Dialogs Here
            services.AddSingleton<MainForm>();
            services.AddSingleton<SecondForm>();

            services.AddSingleton<DeviceTestForm>();
            services.AddSingleton<CreateApplicationForm>();
            services.AddSingleton<LookupForm>();
            services.AddSingleton<SubscriptionsForm>();
            services.AddSingleton<SubscriptionForm>();
            services.AddSingleton<UsersForm>();
            services.AddSingleton<UserForm>();
        }
    }
}