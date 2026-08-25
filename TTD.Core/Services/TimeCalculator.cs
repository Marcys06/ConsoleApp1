using System;
using System.Collections.Generic;
using System.Linq;
using TTD.Data.Models;

namespace TTD.Core.Services
{
    public static class TimeCalculator
    {
        /// <summary>
        /// Oblicza czas przejazdu między stacjami na podstawie odległości i prędkości
        /// </summary>
        public static int CalculateTravelTime(int distanceKm, int vMaxKmh)
        {
            if (distanceKm <= 0 || vMaxKmh <= 0)
                return 0;

            // Czas w minutach = (odległość / prędkość) * 60
            return (int)Math.Ceiling((double)distanceKm / vMaxKmh * 60);
        }

        /// <summary>
        /// Oblicza całkowity czas trasy (w minutach) na podstawie przystanków i czasów przejazdu
        /// </summary>
        public static int CalculateTotalRouteTime(List<RouteStation> routeStations, List<ScheduleTravelTime> travelTimes)
        {
            if (routeStations == null || routeStations.Count == 0)
                return 0;

            int totalTime = 0;
            var sortedStations = routeStations.OrderBy(rs => rs.StopOrder).ToList();

            for (int i = 0; i < sortedStations.Count; i++)
            {
                // Dodaj czas postoju
                totalTime += sortedStations[i].StopDuration;

                // Dodaj czas przejazdu do następnej stacji (jeśli istnieje)
                if (i < sortedStations.Count - 1)
                {
                    var travelTime = travelTimes?
                        .FirstOrDefault(tt => tt.RouteStationId == sortedStations[i].Id);
                    if (travelTime != null)
                        totalTime += travelTime.TravelTimeMinutes;
                }
            }

            return totalTime;
        }

        /// <summary>
        /// Oblicza czas powrotu na stację początkową
        /// </summary>
        public static TimeSpan CalculateReturnTime(TimeSpan departureTime, int totalMinutes)
        {
            return departureTime.Add(TimeSpan.FromMinutes(totalMinutes));
        }

        /// <summary>
        /// Generuje pełny rozkład dla kursu (czasy przyjazdów i odjazdów)
        /// </summary>
        public static List<(string StationName, TimeSpan Arrival, TimeSpan Departure, int StopDuration)> 
            GenerateScheduleTimeline(Schedule schedule, List<RouteStation> routeStations, List<ScheduleTravelTime> travelTimes)
        {
            var result = new List<(string, TimeSpan, TimeSpan, int)>();
            var currentTime = schedule.DepartureTime;
            var sortedStations = routeStations.OrderBy(rs => rs.StopOrder).ToList();

            for (int i = 0; i < sortedStations.Count; i++)
            {
                var station = sortedStations[i];
                var stationName = station.Station?.Name ?? $"Stacja {station.StationId}";

                // Przyjazd (dla pierwszej stacji = czas odjazdu)
                var arrivalTime = currentTime;

                // Czas postoju
                var stopDuration = station.StopDuration;

                // Odjazd
                var departureTime = arrivalTime.Add(TimeSpan.FromMinutes(stopDuration));

                result.Add((stationName, arrivalTime, departureTime, stopDuration));

                // Przejazd do następnej stacji
                if (i < sortedStations.Count - 1)
                {
                    var travelTime = travelTimes?
                        .FirstOrDefault(tt => tt.RouteStationId == station.Id);
                    if (travelTime != null)
                        currentTime = departureTime.Add(TimeSpan.FromMinutes(travelTime.TravelTimeMinutes));
                    else
                        currentTime = departureTime.Add(TimeSpan.FromMinutes(30)); // domyślnie 30 min
                }
            }

            return result;
        }
    }
}