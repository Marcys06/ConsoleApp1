using System.Collections.Generic;

namespace TTD.Data.Models
{
    /// <summary>
    /// Reprezentuje pociąg w systemie.
    /// </summary>
    public class Train
    {
        public int Id { get; set; }

        /// <summary>
        /// Nazwa pociągu (np. "EU07-101").
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Model pociągu (np. "EU07", "EN57").
        /// </summary>
        public string Model { get; set; } = string.Empty;

        /// <summary>
        /// Prędkość maksymalna w km/h.
        /// </summary>
        public int VMax { get; set; }

        /// <summary>
        /// Moc silnika w kW.
        /// </summary>
        public int Power { get; set; }

        /// <summary>
        /// Masa pociągu w tonach.
        /// </summary>
        public int Weight { get; set; }

        /// <summary>
        /// Siła pociągowa (opcjonalna).
        /// </summary>
        public int? TractiveEffort { get; set; }

        /// <summary>
        /// Rok produkcji.
        /// </summary>
        public int ModelYear { get; set; }

        /// <summary>
        /// Czy pociąg jest elektryczny.
        /// </summary>
        public bool IsElectric { get; set; }

        /// <summary>
        /// Ścieżka do ikony lub obrazka pociągu.
        /// </summary>
        public string? ImagePath { get; set; }

        // ===== RELACJE =====

        /// <summary>
        /// Lista kursów (rozkładów) przypisanych do tego pociągu.
        /// </summary>
        public ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
    }
}