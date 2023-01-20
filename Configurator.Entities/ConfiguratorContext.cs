using Microsoft.EntityFrameworkCore;
using Configurator.Database.Entities;

namespace Configurator.Database
{
    public class ConfiguratorContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<Device> Devices { get; set; }
        public DbSet<LookupData> LookupData { get; set; }

        public string DbPath { get; }

        public ConfiguratorContext()
        {
            //var path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            DbPath = Path.Join("C:\\Temp", "configurator.db");
        }

        public ConfiguratorContext(string databaseFile)
        {
            DbPath = databaseFile;
        }

        // The following configures EF to create a Sqlite database file in the special "local" folder for your platform.
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($"Data Source={DbPath}");
    }
}