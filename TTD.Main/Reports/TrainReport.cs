using System;
using System.Linq;
using System.Threading.Tasks;
using TTD.Core.Interfaces;

namespace OpenTTDManager.TTD.Main.Reports
{
    public static class TrainReport
    {
        public static async Task Execute(ITrainService trainService)
        {
            var trains = await trainService.GetAllTrainsAsync();
            
            Console.WriteLine("   📋 LISTA POCIĄGÓW:");
            if (!trains.Any())
            {
                Console.WriteLine("   ⚠️ Brak pociągów w bazie.");
                return;
            }

            foreach (var train in trains)
            {
                Console.WriteLine($"   - {train.Name} (Model: {train.Model}, Vmax: {train.VMax} km/h, " +
                                  $"Moc: {train.Power} kW, Elektryczny: {(train.IsElectric ? "Tak" : "Nie")})");
            }
        }
    }
}