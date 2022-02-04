using PasswordManager.Database;
using PasswordManager.Database.Entities;
using PasswordManager.Entities;

namespace PasswordManager.Console;

internal class Program
{
    private static void Main()
    {
        using var db = new PasswordManagerContext();
        // Note: This sample requires the database to be created before running.
        System.Console.WriteLine($"Database path: {db.DbPath}.");

        // Create
        System.Console.WriteLine("Inserting a new User");
        db.Users.Add(new User { Name = "Pankaj Mishra" });
        db.SaveChanges();

        System.Console.WriteLine("Done");
    }
}