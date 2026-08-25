namespace TTD.Core.Interfaces
{
    public interface IReportService
    {
        void GenerateTrainsReport(string outputPath);
        void GenerateRoutesReport(string outputPath);
        void GenerateScheduleReport(int routeId, string outputPath);
    }
}