using System;
using System.Linq;
using TTD.Data;
using TTD.Data.Models;

namespace TTD.Main.ConsoleTools
{
    public static class SeedData
    {
        public static void Execute(AppDbContext dbContext)
        {
            if (dbContext.Trains.Any())
            {
                Console.WriteLine("   ⚠️ Baza danych już zawiera dane.");
                return;
            }

            Console.WriteLine("   📝 Dodawanie przykładowych danych...");

            // Pociągi
            var trains = new[]
            {
                new Train { Name = "EU07-101", Model = "EU07", VMax = 160, Power = 2400, Weight = 120, ModelYear = 2005, IsElectric = true },
                new Train { Name = "EN57-001", Model = "EN57", VMax = 120, Power = 1800, Weight = 100, ModelYear = 1990, IsElectric = true },
                new Train { Name = "Pendolino-01", Model = "Pendolino", VMax = 250, Power = 5000, Weight = 180, ModelYear = 2014, IsElectric = true }
            };
            dbContext.Trains.AddRange(trains);
            dbContext.SaveChanges();

            // Stacje
            var stations = new[]
            {
                new Station { Name = "Warszawa Centralna", Latitude = 52.2297, Longitude = 21.0122, IsPassenger = true },
                new Station { Name = "Kraków Główny", Latitude = 50.0614, Longitude = 19.9386, IsPassenger = true },
                new Station { Name = "Gdańsk Główny", Latitude = 54.3520, Longitude = 18.6466, IsPassenger = true, IsCargo = true },
                new Station { Name = "Poznań Główny", Latitude = 52.4064, Longitude = 16.9252, IsPassenger = true, IsCargo = true }
            };
            dbContext.Stations.AddRange(stations);
            dbContext.SaveChanges();

            // Trasa
            var warsaw = dbContext.Stations.First(s => s.Name == "Warszawa Centralna");
            var krakow = dbContext.Stations.First(s => s.Name == "Kraków Główny");

            var route = new Route { Name = "Warszawa – Kraków", IsActive = true, Notes = "Trasa testowa" };
            dbContext.Routes.Add(route);
            dbContext.SaveChanges();

            var routeStations = new[]
            {
                new RouteStation { RouteId = route.Id, StationId = warsaw.Id, StopOrder = 1, StopDuration = 5 },
                new RouteStation { RouteId = route.Id, StationId = krakow.Id, StopOrder = 2, StopDuration = 10 },
                new RouteStation { RouteId = route.Id, StationId = warsaw.Id, StopOrder = 3, StopDuration = 10 }
            };
            dbContext.RouteStations.AddRange(routeStations);
            dbContext.SaveChanges();

            // Kursy
            var train = dbContext.Trains.First(t => t.Name == "EU07-101");
            var schedules = new[]
            {
                new Schedule { RouteId = route.Id, TrainId = train.Id, DepartureTime = new TimeSpan(3, 0, 0), IsActive = true, Notes = "nocny" },
                new Schedule { RouteId = route.Id, TrainId = train.Id, DepartureTime = new TimeSpan(6, 0, 0), IsActive = true, Notes = "poranny" },
                new Schedule { RouteId = route.Id, TrainId = train.Id, DepartureTime = new TimeSpan(12, 0, 0), IsActive = true, Notes = "południowy" },
                new Schedule { RouteId = route.Id, TrainId = train.Id, DepartureTime = new TimeSpan(19, 0, 0), IsActive = true, Notes = "wieczorny" }
            };
            dbContext.Schedules.AddRange(schedules);
            dbContext.SaveChanges();

            Console.WriteLine($"   ✅ Dodano {trains.Length} pociągów, {stations.Length} stacje, 1 trasę i {schedules.Length} kursy!");
        }
    }
}