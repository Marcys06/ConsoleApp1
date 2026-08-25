using System;
using System.Collections.Generic;

namespace ConsoleApp1.TTD.Data.Models
{
    /// <summary>
    /// Reprezentuje trasę (np. Warszawa ↔ Kraków).
    /// </summary>
    public class Route
    {
        public int Id { get; set; }

        /// <summary>
        /// Nazwa trasy (np. "Warszawa – Kraków").
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Czy trasa jest aktywna.
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Dodatkowe uwagi o trasie.
        /// </summary>
        public string? Notes { get; set; }

        // ===== RELACJE =====

        /// <summary>
        /// Lista stacji na tej trasie (z kolejnością i czasem postoju).
        /// </summary>
        public ICollection<RouteStation> RouteStations { get; set; } = new List<RouteStation>();

        /// <summary>
        /// Lista kursów (rozkładów) przypisanych do tej trasy.
        /// </summary>
        public ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
    }
}