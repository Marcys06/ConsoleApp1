using System;
using System.Threading.Tasks;
using TTD.Core.Interfaces;

namespace TTD.Main.ConsoleTools
{
    public static class ImportData
    {
        public static async Task Execute(ITrainService trainService)
        {
            Console.WriteLine("   ⚠️ Funkcja importu w przygotowaniu.");
            Console.WriteLine("   (Funkcja będzie dostępna w kolejnej wersji)");
        }
    }
}