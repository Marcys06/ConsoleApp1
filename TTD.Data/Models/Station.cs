using System.Collections.Generic;

namespace TTD.Data.Models
{
    /// <summary>
    /// Reprezentuje stację kolejową w OpenTTD.
    /// </summary>
    public class Station
    {
        public int Id { get; set; }

        /// <summary>
        /// Nazwa stacji (np. "Warszawa Centralna").
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Współrzędna X na mapie OpenTTD (w tile'ach).
        /// Zakres: 0-16383 (dla mapy 16k x 16k).
        /// </summary>
        public int TileX { get; set; }

        /// <summary>
        /// Współrzędna Y na mapie OpenTTD (w tile'ach).
        /// Zakres: 0-16383 (dla mapy 16k x 16k).
        /// </summary>
        public int TileY { get; set; }

        /// <summary>
        /// Czy stacja obsługuje pasażerów.
        /// </summary>
        public bool IsPassenger { get; set; }

        /// <summary>
        /// Czy stacja obsługuje towary.
        /// </summary>
        public bool IsCargo { get; set; }

        /// <summary>
        /// Liczba peronów na stacji.
        /// Domyślnie: 2.
        /// </summary>
        public int PlatformCount { get; set; } = 2;

        // ===== RELACJE =====

        /// <summary>
        /// Lista powiązań tej stacji z trasami (kolejność postojów).
        /// </summary>
        public ICollection<RouteStation> RouteStations { get; set; } = new List<RouteStation>();
    }
}