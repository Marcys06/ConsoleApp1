using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using TTD.Core.Interfaces;

namespace OpenTTDManager.TTD.Main.API
{
    public class SimpleApiServer
    {
        private readonly IServiceProvider _serviceProvider;
        private HttpListener? _listener;
        private bool _isRunning;

        public SimpleApiServer(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(string prefix = "http://localhost:5000/")
        {
            _listener = new HttpListener();
            _listener.Prefixes.Add(prefix);
            _listener.Start();
            _isRunning = true;

            Console.WriteLine($"   ✅ Serwer API uruchomiony na {prefix}");
            Console.WriteLine("   📋 Dostępne endpointy:");
            Console.WriteLine("      GET /api/trains");
            Console.WriteLine("      GET /api/stations");
            Console.WriteLine("      GET /api/routes");
            Console.WriteLine("      GET /api/schedules");

            while (_isRunning)
            {
                try
                {
                    var context = await _listener.GetContextAsync();
                    _ = Task.Run(() => ProcessRequest(context));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"   ⚠️ Błąd serwera: {ex.Message}");
                }
            }
        }

        private async Task ProcessRequest(HttpListenerContext context)
        {
            var request = context.Request;
            var response = context.Response;
            response.ContentType = "application/json";

            string responseText = "{}";

            try
            {
                using var scope = _serviceProvider.CreateScope();
                var trainService = scope.ServiceProvider.GetRequiredService<ITrainService>();
                var stationService = scope.ServiceProvider.GetRequiredService<IStationService>();
                var routeService = scope.ServiceProvider.GetRequiredService<IRouteService>();
                var scheduleService = scope.ServiceProvider.GetRequiredService<IScheduleService>();

                if (request.HttpMethod == "GET")
                {
                    if (request.Url?.AbsolutePath == "/api/trains")
                    {
                        var data = await trainService.GetAllTrainsAsync();
                        responseText = JsonSerializer.Serialize(data);
                    }
                    else if (request.Url?.AbsolutePath == "/api/stations")
                    {
                        var data = await stationService.GetAllStationsAsync();
                        responseText = JsonSerializer.Serialize(data);
                    }
                    else if (request.Url?.AbsolutePath == "/api/routes")
                    {
                        var data = await routeService.GetAllRoutesAsync();
                        responseText = JsonSerializer.Serialize(data);
                    }
                    else if (request.Url?.AbsolutePath == "/api/schedules")
                    {
                        var data = await scheduleService.GetAllSchedulesAsync();
                        responseText = JsonSerializer.Serialize(data);
                    }
                }
            }
            catch (Exception ex)
            {
                responseText = $"{{\"error\": \"{ex.Message}\"}}";
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }

            var buffer = Encoding.UTF8.GetBytes(responseText);
            response.ContentLength64 = buffer.Length;
            await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
            response.OutputStream.Close();
        }

        public void Stop()
        {
            _isRunning = false;
            _listener?.Stop();
            _listener?.Close();
            Console.WriteLine("   ⏹️ Serwer API zatrzymany.");
        }
    }
}