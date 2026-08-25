namespace TTD.Data.Models
{
    /// <summary>
    /// Reprezentuje czas przejazdu dla konkretnego kursu na danym odcinku trasy.
    /// </summary>
    public class ScheduleTravelTime
    {
        public int Id { get; set; }

        // ===== KLUCZE OBCE =====

        /// <summary>
        /// Identyfikator kursu.
        /// </summary>
        public int ScheduleId { get; set; }

        /// <summary>
        /// Identyfikator połączenia trasa-stacja (odcinek trasy).
        /// </summary>
        public int RouteStationId { get; set; }

        // ===== DANE =====

        /// <summary>
        /// Czas przejazdu z poprzedniej stacji (w minutach).
        /// </summary>
        public int TravelTimeMinutes { get; set; }

        // ===== RELACJE =====

        /// <summary>
        /// Kurs, którego dotyczy ten czas przejazdu.
        /// </summary>
        public Schedule Schedule { get; set; } = null!;

        /// <summary>
        /// Odcinek trasy (połączenie trasa-stacja), którego dotyczy ten czas.
        /// </summary>
        public RouteStation RouteStation { get; set; } = null!;
    }
}