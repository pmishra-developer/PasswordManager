using Configurator.Database;
using Configurator.Database.Entities;

namespace Configurator.Console;

public class Program
{
    private static void Main()
    {
        using var db = new ConfiguratorContext();
      
        // Note: This sample requires the database to be created before running.
        System.Console.WriteLine($"Database path: {db.DbPath}.");

        // Create User
        System.Console.WriteLine("Inserting a new User");
        db.Users.Add(new User { FirstName = "Karmjit", LastName = "Kalsi"});
        db.SaveChanges();
        System.Console.WriteLine("New User Added.");

        // Create User
        System.Console.WriteLine("Inserting a new Device");
        db.Devices.Add(new Device() { Id = 0, Created = DateTime.Now, SerialNumber = "00001", TargetMarket = "Asia", UUID = "iCode_000001" });
        db.SaveChanges();
        System.Console.WriteLine("New Device Added.");

        System.Console.WriteLine("Done, Press Any key to continue");
        System.Console.ReadKey();
    }
}