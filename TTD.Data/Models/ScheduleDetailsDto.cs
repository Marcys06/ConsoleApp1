using System;
using System.Collections.Generic;

namespace ConsoleApp1.TTD.Data.Models
{
    /// <summary>
    /// DTO do wyświetlania szczegółów kursu z obliczonymi czasami.
    /// </summary>
    public class ScheduleDetailsDto
    {
        public int ScheduleId { get; set; }
        public string TrainName { get; set; } = string.Empty;
        public string RouteName { get; set; } = string.Empty;
        public TimeSpan DepartureTime { get; set; }
        public TimeSpan? ArrivalTime { get; set; } // czas powrotu na stację początkową
        public List<StopDetail> Stops { get; set; } = new List<StopDetail>();
    }

    /// <summary>
    /// Szczegóły pojedynczego przystanku w kursie.
    /// </summary>
    public class StopDetail
    {
        public string StationName { get; set; } = string.Empty;
        public TimeSpan ArrivalTime { get; set; }
        public int StopDuration { get; set; }
        public TimeSpan DepartureTime { get; set; }
        public int? DistanceFromPrevious { get; set; }
    }
}