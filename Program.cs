using ConsoleApp1.Data;
using ConsoleApp1.Models;

using var db = new AppDbContext();

db.Database.EnsureCreated();

if (!db.Users.Any())
{
    db.Users.Add(new User
    {
        Name = "Marcel",
        Email = "marcel@example.com"
    });

    db.SaveChanges();
}

Console.WriteLine("Baza danych działa.");
Console.WriteLine($"Liczba użytkowników: {db.Users.Count()}");