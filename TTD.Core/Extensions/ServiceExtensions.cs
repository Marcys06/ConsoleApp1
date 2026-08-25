using Microsoft.Extensions.DependencyInjection;
using TTD.Core.Interfaces;
using TTD.Core.Services;

namespace TTD.Core.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddCoreServices(this IServiceCollection services)
        {
            services.AddScoped<ITrainService, TrainService>();
            services.AddScoped<IStationService, StationService>();
            services.AddScoped<IRouteService, RouteService>();
            services.AddScoped<IScheduleService, ScheduleService>();

            return services;
        }
    }
}