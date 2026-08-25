using System.Collections.Generic;

namespace ConsoleApp1.TTD.Data.Models
{
    public class RouteStation
    {
        public int Id { get; set; }  // ← TO MUSI BYĆ!
        
        public int RouteId { get; set; }
        public int StationId { get; set; }
        public int StopOrder { get; set; }
        public int StopDuration { get; set; }
        public int? DistanceFromPrevious { get; set; }
        
        public Route Route { get; set; } = null!;
        public Station Station { get; set; } = null!;
        public ICollection<ScheduleTravelTime> ScheduleTravelTimes { get; set; } = new List<ScheduleTravelTime>();
    }
}