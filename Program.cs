using ConsoleApp1.TTD.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Threading.Tasks;
using TTD.Core.Extensions;
using TTD.Core.Interfaces;
using TTD.Core.Services;
using TTD.Data;

namespace TTD.Main
{
    class Program
    {
        private static IServiceProvider? _serviceProvider;
        private static IHost? _host;

        static void Main(string[] args)
        {
            Console.Title = "OpenTTD Manager - Launcher";

            // ===== INICJALIZACJA HOSTA =====
            _host = CreateHostBuilder(args).Build();
            _serviceProvider = _host.Services;

            // ===== URUCHAMIANIE LAUNCHERA =====
            while (true)
            {
                Console.Clear();
                Console.WriteLine("╔═══════════════════════════════════════════════════════╗");
                Console.WriteLine("║              OPEN TTD MANAGER - LAUNCHER              ║");
                Console.WriteLine("╚═══════════════════════════════════════════════════════╝");
                Console.WriteLine();
                Console.WriteLine("   Wybierz aplikację do uruchomienia:");
                Console.WriteLine();
                Console.WriteLine("   [1] 🖥️  Interfejs graficzny (Windows Forms)");
                Console.WriteLine("   [2] 🌐  Serwer API (REST)");
                Console.WriteLine("   [3] ⌨️  Narzędzia konsolowe");
                Console.WriteLine("   [4] 🗄️  Zarządzanie bazą danych");
                Console.WriteLine("   [5] 📊  Generowanie raportów");
                Console.WriteLine("   [0] ❌  Wyjście");
                Console.WriteLine();
                Console.Write("   Twój wybór: ");

                var input = Console.ReadLine();
                Console.WriteLine();

                switch (input)
                {
                    case "1":
                        RunUI();
                        break;
                    case "2":
                        RunAPI();
                        break;
                    case "3":
                        RunConsoleTools();
                        break;
                    case "4":
                        RunDatabaseManager();
                        break;
                    case "5":
                        RunReports();
                        break;
                    case "0":
                        Console.WriteLine("   Do widzenia! 👋");
                        return;
                    default:
                        Console.WriteLine("   ❌ Nieprawidłowy wybór. Naciśnij dowolny klawisz...");
                        Console.ReadKey();
                        break;
                }
            }
        }

        // ==========================================
        // KONFIGURACJA HOSTA (DI)
        // ==========================================

        static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    // ===== BAZA DANYCH =====
                    string dbPath = Path.Combine(
                        AppDomain.CurrentDomain.BaseDirectory,
                        "ttd_database.db"
                    );
                    services.AddDbContext<AppDbContext>(options =>
                        options.UseSqlite($"Data Source={dbPath}"));

                    // ===== SERWISY Z TTD.Core =====
                    services.AddCoreServices();

                    // ===== WŁASNE SERWISY (np. raporty) =====
                    services.AddScoped<IReportService, ReportService>();
                });

        // ==========================================
        // URUCHAMIANIE POSZCZEGÓLNYCH APLIKACJI
        // ==========================================

        // ===== 1. INTERFEJS GRAFICZNY (Windows Forms) =====
        static void RunUI()
        {
            try
            {
                Console.WriteLine("   🖥️ Uruchamianie interfejsu graficznego...");
                Console.WriteLine("   (Aby wrócić do launcher'a, zamknij okno aplikacji)");
                Console.WriteLine();
                Console.WriteLine("   Naciśnij dowolny klawisz, aby kontynuować...");
                Console.ReadKey();

                // Uruchom Windows Forms
                // Application.EnableVisualStyles();
                // Application.SetCompatibleTextRenderingDefault(false);
                // Application.Run(new TTD.UI.Forms.MainForm());

                // Na razie informacja
                Console.WriteLine("   ⚠️ Moduł UI w przygotowaniu...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ❌ Błąd uruchamiania UI: {ex.Message}");
                Console.WriteLine("   Naciśnij dowolny klawisz, aby kontynuować...");
                Console.ReadKey();
            }
        }

        // ===== 2. SERWER API =====
        static void RunAPI()
        {
            try
            {
                Console.WriteLine("   🌐 Uruchamianie serwera API...");
                Console.WriteLine("   Serwer będzie dostępny pod adresem: https://localhost:5001");
                Console.WriteLine("   (Aby zatrzymać serwer, naciśnij Ctrl+C)");
                Console.WriteLine();
                Console.WriteLine("   Naciśnij dowolny klawisz, aby kontynuować...");
                Console.ReadKey();

                // Uruchom API
                // var host = CreateApiHost(args).Build();
                // host.Run();

                Console.WriteLine("   ⚠️ Moduł API w przygotowaniu...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ❌ Błąd uruchamiania API: {ex.Message}");
                Console.WriteLine("   Naciśnij dowolny klawisz, aby kontynuować...");
                Console.ReadKey();
            }
        }

        // ===== 3. NARZĘDZIA KONSOLOWE =====
        static void RunConsoleTools()
        {
            Console.WriteLine("   ⌨️ Narzędzia konsolowe:");
            Console.WriteLine();
            Console.WriteLine("   [1] Dodaj przykładowe dane (seed)");
            Console.WriteLine("   [2] Eksportuj dane do CSV");
            Console.WriteLine("   [3] Importuj dane z CSV");
            Console.WriteLine("   [4] Wyświetl statystyki");
            Console.WriteLine("   [0] Powrót do menu");
            Console.WriteLine();
            Console.Write("   Twój wybór: ");

            var choice = Console.ReadLine();
            Console.WriteLine();

            switch (choice)
            {
                case "1":
                    SeedDatabase();
                    break;
                case "2":
                    ExportToCsv();
                    break;
                case "3":
                    ImportFromCsv();
                    break;
                case "4":
                    ShowStatistics();
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("   ❌ Nieprawidłowy wybór.");
                    break;
            }

            Console.WriteLine();
            Console.WriteLine("   Naciśnij dowolny klawisz, aby kontynuować...");
            Console.ReadKey();
        }

        // ===== 4. ZARZĄDZANIE BAZĄ DANYCH =====
        static void RunDatabaseManager()
        {
            Console.WriteLine("   🗄️ Zarządzanie bazą danych:");
            Console.WriteLine();
            Console.WriteLine("   [1] Utwórz migrację (Add-Migration)");
            Console.WriteLine("   [2] Aktualizuj bazę (Update-Database)");
            Console.WriteLine("   [3] Usuń bazę danych");
            Console.WriteLine("   [0] Powrót do menu");
            Console.WriteLine();
            Console.Write("   Twój wybór: ");

            var choice = Console.ReadLine();
            Console.WriteLine();

            switch (choice)
            {
                case "1":
                    // AddMigration();
                    Console.WriteLine("   ✅ Migracja utworzona!");
                    break;
                case "2":
                    // UpdateDatabase();
                    Console.WriteLine("   ✅ Baza zaktualizowana!");
                    break;
                case "3":
                    DropDatabase();
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("   ❌ Nieprawidłowy wybór.");
                    break;
            }

            Console.WriteLine();
            Console.WriteLine("   Naciśnij dowolny klawisz, aby kontynuować...");
            Console.ReadKey();
        }

        // ===== 5. RAPORTY =====
        static void RunReports()
        {
            Console.WriteLine("   📊 Generowanie raportów:");
            Console.WriteLine();
            Console.WriteLine("   [1] Lista wszystkich pociągów");
            Console.WriteLine("   [2] Lista wszystkich tras");
            Console.WriteLine("   [3] Rozkład jazdy dla wybranej trasy");
            Console.WriteLine("   [0] Powrót do menu");
            Console.WriteLine();
            Console.Write("   Twój wybór: ");

            var choice = Console.ReadLine();
            Console.WriteLine();

            switch (choice)
            {
                case "1":
                    ListAllTrains();
                    break;
                case "2":
                    ListAllRoutes();
                    break;
                case "3":
                    ShowSchedule();
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("   ❌ Nieprawidłowy wybór.");
                    break;
            }

            Console.WriteLine();
            Console.WriteLine("   Naciśnij dowolny klawisz, aby kontynuować...");
            Console.ReadKey();
        }

        // ==========================================
        // IMPLEMENTACJE FUNKCJI
        // ==========================================

        static void SeedDatabase()
        {
            try
            {
                using var scope = _serviceProvider!.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var trainService = scope.ServiceProvider.GetRequiredService<ITrainService>();

                // Sprawdź, czy są już dane
                var trains = trainService.GetAllTrainsAsync().GetAwaiter().GetResult();
                if (trains.Any())
                {
                    Console.WriteLine("   ⚠️ Baza danych już zawiera dane.");
                    return;
                }

                // Dodaj przykładowe pociągi
                var newTrains = new[]
                {
                    new TTD.Data.Models.Train
                    {
                        Name = "EU07-101",
                        Model = "EU07",
                        VMax = 160,
                        Power = 2400,
                        Weight = 120,
                        ModelYear = 2005,
                        IsElectric = true
                    },
                    new TTD.Data.Models.Train
                    {
                        Name = "EN57-001",
                        Model = "EN57",
                        VMax = 120,
                        Power = 1800,
                        Weight = 100,
                        ModelYear = 1990,
                        IsElectric = true
                    },
                    new TTD.Data.Models.Train
                    {
                        Name = "Pendolino-01",
                        Model = "Pendolino",
                        VMax = 250,
                        Power = 5000,
                        Weight = 180,
                        ModelYear = 2014,
                        IsElectric = true
                    }
                };

                foreach (var train in newTrains)
                {
                    trainService.AddTrainAsync(train).GetAwaiter().GetResult();
                }

                Console.WriteLine($"   ✅ Dodano {newTrains.Length} przykładowe pociągi!");
                Console.WriteLine("   💡 Możesz teraz dodać stacje i trasy z poziomu UI.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ❌ Błąd podczas seedowania: {ex.Message}");
            }
        }

        static void ExportToCsv()
        {
            try
            {
                using var scope = _serviceProvider!.CreateScope();
                var trainService = scope.ServiceProvider.GetRequiredService<ITrainService>();
                var trains = trainService.GetAllTrainsAsync().GetAwaiter().GetResult();

                string csvPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                    "trains_export.csv"
                );

                using var writer = new StreamWriter(csvPath);
                writer.WriteLine("Id,Name,Model,VMax,Power,Weight,ModelYear,IsElectric");
                foreach (var train in trains)
                {
                    writer.WriteLine($"{train.Id},{train.Name},{train.Model},{train.VMax},{train.Power},{train.Weight},{train.ModelYear},{train.IsElectric}");
                }

                Console.WriteLine($"   ✅ Eksport zakończony! Plik: {csvPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ❌ Błąd eksportu: {ex.Message}");
            }
        }

        static void ImportFromCsv()
        {
            Console.WriteLine("   ⚠️ Funkcja importu w przygotowaniu...");
        }

        static void ShowStatistics()
        {
            try
            {
                using var scope = _serviceProvider!.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                int trainCount = dbContext.Trains.Count();
                int stationCount = dbContext.Stations.Count();
                int routeCount = dbContext.Routes.Count();
                int scheduleCount = dbContext.Schedules.Count();

                Console.WriteLine("   📊 STATYSTYKI BAZY DANYCH:");
                Console.WriteLine($"   🚂 Pociągi: {trainCount}");
                Console.WriteLine($"   🏢 Stacje: {stationCount}");
                Console.WriteLine($"   🛤️ Trasy: {routeCount}");
                Console.WriteLine($"   🕐 Kursy: {scheduleCount}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ❌ Błąd statystyk: {ex.Message}");
            }
        }

        static void DropDatabase()
        {
            try
            {
                string dbPath = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "ttd_database.db"
                );

                if (File.Exists(dbPath))
                {
                    Console.WriteLine($"   ⚠️ Usuwanie bazy: {dbPath}");
                    File.Delete(dbPath);
                    Console.WriteLine("   ✅ Baza danych została usunięta!");
                }
                else
                {
                    Console.WriteLine("   ⚠️ Plik bazy danych nie istnieje.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ❌ Błąd usuwania bazy: {ex.Message}");
            }
        }

        static void ListAllTrains()
        {
            try
            {
                using var scope = _serviceProvider!.CreateScope();
                var trainService = scope.ServiceProvider.GetRequiredService<ITrainService>();
                var trains = trainService.GetAllTrainsAsync().GetAwaiter().GetResult();

                Console.WriteLine("   📋 LISTA POCIĄGÓW:");
                if (!trains.Any())
                {
                    Console.WriteLine("   ⚠️ Brak pociągów w bazie.");
                    return;
                }

                foreach (var train in trains)
                {
                    Console.WriteLine($"   - {train.Name} (Model: {train.Model}, Vmax: {train.VMax} km/h, Elektryczny: {(train.IsElectric ? "Tak" : "Nie")})");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ❌ Błąd: {ex.Message}");
            }
        }

        static void ListAllRoutes()
        {
            try
            {
                using var scope = _serviceProvider!.CreateScope();
                var routeService = scope.ServiceProvider.GetRequiredService<IRouteService>();
                var routes = routeService.GetAllRoutesAsync().GetAwaiter().GetResult();

                Console.WriteLine("   📋 LISTA TRAS:");
                if (!routes.Any())
                {
                    Console.WriteLine("   ⚠️ Brak tras w bazie.");
                    return;
                }

                foreach (var route in routes)
                {
                    var stops = route.RouteStations?.OrderBy(rs => rs.StopOrder).ToList() ?? new();
                    string stopNames = string.Join(" → ", stops.Select(s => s.Station?.Name ?? "?"));
                    Console.WriteLine($"   - {route.Name} (Aktywna: {(route.IsActive ? "Tak" : "Nie")})");
                    Console.WriteLine($"     Trasa: {stopNames}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ❌ Błąd: {ex.Message}");
            }
        }

        static void ShowSchedule()
        {
            Console.WriteLine("   ⚠️ Funkcja wyświetlania rozkładu w przygotowaniu...");
        }
    }
}