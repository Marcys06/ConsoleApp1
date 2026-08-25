/*
using ConsoleApp1.Temp;
using Microsoft.EntityFrameworkCore;

namespace ConsoleApp1.Data;

public class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(
            "Server=(localdb)\\MSSQLLocalDB;Database=ConsoleApp1Db;Trusted_Connection=True;TrustServerCertificate=True;");
    }
}
*/