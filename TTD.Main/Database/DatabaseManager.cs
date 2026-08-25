using System;
using System.IO;

namespace TTD.Main.Database
{
    public static class DatabaseManager
    {
        private static string DbPath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ttd_database.db");

        public static void BackupDatabase()
        {
            if (!File.Exists(DbPath))
            {
                Console.WriteLine("   ⚠️ Plik bazy danych nie istnieje.");
                return;
            }

            string backupPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                $"ttd_backup_{DateTime.Now:yyyyMMdd_HHmmss}.db"
            );

            try
            {
                File.Copy(DbPath, backupPath);
                Console.WriteLine($"   ✅ Backup utworzony: {backupPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ❌ Błąd backupu: {ex.Message}");
            }
        }

        public static void DropDatabase()
        {
            if (!File.Exists(DbPath))
            {
                Console.WriteLine("   ⚠️ Plik bazy danych nie istnieje.");
                return;
            }

            try
            {
                File.Delete(DbPath);
                Console.WriteLine("   ✅ Baza danych została usunięta.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ❌ Błąd usuwania: {ex.Message}");
            }
        }

        public static void AddMigration()
        {
            Console.WriteLine("   ⚠️ Ta funkcja wymaga uruchomienia z poziomu Package Manager Console:");
            Console.WriteLine("   > Add-Migration [NazwaMigracji]");
        }

        public static void UpdateDatabase()
        {
            Console.WriteLine("   ⚠️ Ta funkcja wymaga uruchomienia z poziomu Package Manager Console:");
            Console.WriteLine("   > Update-Database");
        }
    }
}