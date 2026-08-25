using System;
using TTD.Data;
using TTD.Data.Models;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("OpenTTD Manager - Test połączenia z bazą danych");

        // Test połączenia
        using var context = new AppDbContext();
        Console.WriteLine($"Baza danych: {context.Database.ProviderName}");

        Console.WriteLine("Naciśnij dowolny klawisz...");
        Console.ReadKey(); // <- TO ZATRZYMUJE KONSOLĘ
    }
}