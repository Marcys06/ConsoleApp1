using System.Collections.Generic;

namespace TTD.Data.Models
{
    /// <summary>
    /// Reprezentuje połączenie między trasą a stacją (tabela pośrednia).
    /// </summary>
    public class RouteStation
    {
        // ===== KLUCZE OBCE (klucz złożony: RouteId + StationId + StopOrder) =====

        /// <summary>
        /// Identyfikator trasy.
        /// </summary>
        public int RouteId { get; set; }

        /// <summary>
        /// Identyfikator stacji.
        /// </summary>
        public int StationId { get; set; }

        /// <summary>
        /// Kolejność postoju na trasie (1, 2, 3...).
        /// </summary>
        public int StopOrder { get; set; }

        // ===== DODATKOWE WŁAŚCIWOŚCI =====

        /// <summary>
        /// Czas postoju na tej stacji (w minutach).
        /// </summary>
        public int StopDuration { get; set; }

        /// <summary>
        /// Odległość od poprzedniej stacji (w km, opcjonalna).
        /// </summary>
        public int? DistanceFromPrevious { get; set; }

        // ===== RELACJE =====

        /// <summary>
        /// Trasa, do której należy to połączenie.
        /// </summary>
        public Route Route { get; set; } = null!;

        /// <summary>
        /// Stacja, której dotyczy to połączenie.
        /// </summary>
        public Station Station { get; set; } = null!;

        /// <summary>
        /// Lista czasów przejazdu dla konkretnych kursów na tym odcinku.
        /// </summary>
        public ICollection<ScheduleTravelTime> ScheduleTravelTimes { get; set; } = new List<ScheduleTravelTime>();
    }
}