using TTD.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTTDManager.TTD.Main.API;
using OpenTTDManager.TTD.Main.Reports;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using TTD.Core.Extensions;
using TTD.Core.Interfaces;
using TTD.Main.ConsoleTools;
using TTD.Main.Database;
using TTD.Main.UI.Forms;

namespace TTD.Main
{
    class Program
    {
        private static IServiceProvider? _serviceProvider;
        private static IHost? _host;
        private static SimpleApiServer? _apiServer;

        static async Task Main(string[] args)
        {
            Console.Title = "OpenTTD Manager - Launcher";
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // ===== INICJALIZACJA HOSTA (DI) =====
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
                        await RunAPI();
                        break;
                    case "3":
                        RunConsoleTools();
                        break;
                    case "4":
                        RunDatabaseManager();
                        break;
                    case "5":
                        await RunReports();
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
                });

        // ==========================================
        // URUCHAMIANIE POSZCZEGÓLNYCH APLIKACJI
        // ==========================================

        // ===== 1. INTERFEJS GRAFICZNY (Windows Forms) =====
        static void RunUI()
        {
            try
            {
                Console.WriteLine("1. Start UI");
                Console.WriteLine("   🖥️ Uruchamianie interfejsu graficznego...");
                Console.WriteLine("   (Aby wrócić do launcher'a, zamknij okno aplikacji)");
                Console.WriteLine();
                Console.WriteLine("   Naciśnij dowolny klawisz, aby kontynuować...");
                Console.ReadKey();

                // ===== USTAWIENIA WINDOWS FORMS (PRZED UTWORZENIEM OKNA) =====
                Console.WriteLine("2. EnableVisualStyles");
                System.Windows.Forms.Application.EnableVisualStyles();

                Console.WriteLine("3. TextRendering");
                System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);

                // ===== TWORZENIE OKNA (PO USTAWIENIACH) =====
                Console.WriteLine("4. Scope created");
                using var scope = _serviceProvider!.CreateScope();

                Console.WriteLine("5. MainForm created");
                var mainForm = new MainForm(scope.ServiceProvider);

                System.Windows.Forms.Application.Run(mainForm);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Błąd uruchamiania UI: {ex.Message}");
                Console.WriteLine("   Naciśnij dowolny klawisz, aby kontynuować...");
                Console.ReadKey();
            }
        }

        // ===== 2. SERWER API =====
        static async Task RunAPI()
        {
            try
            {
                Console.WriteLine("   🌐 Uruchamianie serwera API...");
                Console.WriteLine("   Serwer będzie dostępny pod adresem: http://localhost:5000");
                Console.WriteLine("   (Aby zatrzymać serwer, naciśnij Ctrl+C)");
                Console.WriteLine();
                Console.WriteLine("   Naciśnij dowolny klawisz, aby kontynuować...");
                Console.ReadKey();

                _apiServer = new SimpleApiServer(_serviceProvider!);
                await _apiServer.StartAsync("http://localhost:5000/");

                Console.WriteLine("   ✅ Serwer API uruchomiony. Naciśnij dowolny klawisz, aby zatrzymać...");
                Console.ReadKey();
                _apiServer.Stop();
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
            using var scope = _serviceProvider!.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var trainService = scope.ServiceProvider.GetRequiredService<ITrainService>();
            var stationService = scope.ServiceProvider.GetRequiredService<IStationService>();
            var routeService = scope.ServiceProvider.GetRequiredService<IRouteService>();

            while (true)
            {
                Console.Clear();
                Console.WriteLine("   ⌨️ Narzędzia konsolowe:");
                Console.WriteLine();
                Console.WriteLine("   [1] Dodaj przykładowe dane (seed)");
                Console.WriteLine("   [2] Eksportuj dane do CSV");
                Console.WriteLine("   [3] Importuj dane z CSV");
                Console.WriteLine("   [4] Wyświetl statystyki");
                Console.WriteLine("   [0] Powrót do menu głównego");
                Console.WriteLine();
                Console.Write("   Twój wybór: ");

                var choice = Console.ReadLine();
                Console.WriteLine();

                switch (choice)
                {
                    case "1":
                        SeedData.Execute(dbContext);
                        break;
                    case "2":
                        ExportData.Execute(trainService, stationService, routeService);
                        break;
                    case "3":
                        ImportData.Execute(trainService);
                        break;
                    case "4":
                        Statistics.Execute(dbContext);
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
        }

        // ===== 4. ZARZĄDZANIE BAZĄ DANYCH =====
        static void RunDatabaseManager()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("   🗄️ Zarządzanie bazą danych:");
                Console.WriteLine();
                Console.WriteLine("   [1] Utwórz migrację (Add-Migration)");
                Console.WriteLine("   [2] Aktualizuj bazę (Update-Database)");
                Console.WriteLine("   [3] Usuń bazę danych");
                Console.WriteLine("   [4] Wykonaj backup bazy");
                Console.WriteLine("   [0] Powrót do menu głównego");
                Console.WriteLine();
                Console.Write("   Twój wybór: ");

                var choice = Console.ReadLine();
                Console.WriteLine();

                switch (choice)
                {
                    case "1":
                        DatabaseManager.AddMigration();
                        break;
                    case "2":
                        DatabaseManager.UpdateDatabase();
                        break;
                    case "3":
                        DatabaseManager.DropDatabase();
                        break;
                    case "4":
                        DatabaseManager.BackupDatabase();
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
        }

        // ===== 5. RAPORTY =====
        static async Task RunReports()
        {
            using var scope = _serviceProvider!.CreateScope();
            var trainService = scope.ServiceProvider.GetRequiredService<ITrainService>();
            var routeService = scope.ServiceProvider.GetRequiredService<IRouteService>();
            var scheduleService = scope.ServiceProvider.GetRequiredService<IScheduleService>();

            while (true)
            {
                Console.Clear();
                Console.WriteLine("   📊 Generowanie raportów:");
                Console.WriteLine();
                Console.WriteLine("   [1] Lista wszystkich pociągów");
                Console.WriteLine("   [2] Lista wszystkich tras");
                Console.WriteLine("   [3] Rozkład jazdy dla wybranej trasy");
                Console.WriteLine("   [4] Pełny raport do pliku tekstowego");
                Console.WriteLine("   [0] Powrót do menu głównego");
                Console.WriteLine();
                Console.Write("   Twój wybór: ");

                var choice = Console.ReadLine();
                Console.WriteLine();

                switch (choice)
                {
                    case "1":
                        await TrainReport.Execute(trainService);
                        break;
                    case "2":
                        await RouteReport.Execute(routeService);
                        break;
                    case "3":
                        await ScheduleReport.Execute(scheduleService);
                        break;
                    case "4":
                        await FullReport.Execute(trainService, routeService, scheduleService);
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
        }
    }
}