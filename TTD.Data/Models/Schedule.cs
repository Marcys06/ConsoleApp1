using System;
using System.Collections.Generic;

namespace ConsoleApp1.TTD.Data.Models
{
    /// <summary>
    /// Reprezentuje konkretny kurs/rozklad na danej trasie.
    /// </summary>
    public class Schedule
    {
        public int Id { get; set; }

        // ===== KLUCZE OBCE =====

        /// <summary>
        /// Identyfikator trasy.
        /// </summary>
        public int RouteId { get; set; }

        /// <summary>
        /// Identyfikator pociągu.
        /// </summary>
        public int TrainId { get; set; }

        // ===== DANE KURSU =====

        /// <summary>
        /// Godzina odjazdu z pierwszej stacji.
        /// </summary>
        public TimeSpan DepartureTime { get; set; }

        /// <summary>
        /// Czy kurs jest aktywny.
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Data od której kurs obowiązuje (opcjonalna).
        /// </summary>
        public DateTime? ValidFrom { get; set; }

        /// <summary>
        /// Data do której kurs obowiązuje (opcjonalna).
        /// </summary>
        public DateTime? ValidTo { get; set; }

        /// <summary>
        /// Dodatkowe uwagi o kursie (np. "kursuje tylko w weekendy").
        /// </summary>
        public string? Notes { get; set; }

        // ===== RELACJE =====

        /// <summary>
        /// Trasa, której dotyczy ten kurs.
        /// </summary>
        public Route Route { get; set; } = null!;

        /// <summary>
        /// Pociąg, który obsługuje ten kurs.
        /// </summary>
        public Train Train { get; set; } = null!;

        /// <summary>
        /// Lista czasów przejazdu dla tego kursu (dla każdego odcinka trasy).
        /// </summary>
        public ICollection<ScheduleTravelTime> TravelTimes { get; set; } = new List<ScheduleTravelTime>();
    }
}