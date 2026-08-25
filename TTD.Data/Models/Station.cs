using System.Collections.Generic;

namespace ConsoleApp1.TTD.Data.Models
{
    /// <summary>
    /// Reprezentuje stację kolejową.
    /// </summary>
    public class Station
    {
        public int Id { get; set; }

        /// <summary>
        /// Nazwa stacji (np. "Warszawa Centralna").
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Szerokość geograficzna (lub koordynata X na mapie).
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// Długość geograficzna (lub koordynata Y na mapie).
        /// </summary>
        public double Longitude { get; set; }

        /// <summary>
        /// Koordynata X na mapie OpenTTD (opcjonalna).
        /// </summary>
        public int? MapX { get; set; }

        /// <summary>
        /// Koordynata Y na mapie OpenTTD (opcjonalna).
        /// </summary>
        public int? MapY { get; set; }

        /// <summary>
        /// Czy stacja obsługuje pasażerów.
        /// </summary>
        public bool IsPassenger { get; set; }

        /// <summary>
        /// Czy stacja obsługuje towary.
        /// </summary>
        public bool IsCargo { get; set; }

        // ===== RELACJE =====

        /// <summary>
        /// Lista powiązań tej stacji z trasami (kolejność postojów).
        /// </summary>
        public ICollection<RouteStation> RouteStations { get; set; } = new List<RouteStation>();
    }
}