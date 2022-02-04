using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using PasswordManager.Database.Entities;

namespace PasswordManager.Database
{
    public class PasswordManagerContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public string DbPath { get; }

        public PasswordManagerContext()
        {
            //var path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            DbPath = Path.Join("C:\\Temp", "passwords.db");
        }

        public PasswordManagerContext(string databaseFile)
        {
            DbPath = databaseFile;
        }

        // The following configures EF to create a Sqlite database file in the special "local" folder for your platform.
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($"Data Source={DbPath}");
    }
}