using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;
using TTD.Core.Interfaces;
using TTD.Data.Models;

namespace TTD.Main.UI.Forms
{
    public partial class ScheduleDetailsForm : Form
    {
        private readonly Schedule _schedule;
        private readonly IRouteService _routeService;
        private readonly ITrainService _trainService;

        // ===== KONTROLKI =====
        private Label lblTitle = null!;
        private Label lblInfo = null!;
        private DataGridView dgvStops = null!;
        private Label lblSummary = null!;
        private Button btnClose = null!;

        // ===== KONSTRUKTOR =====
        public ScheduleDetailsForm(Schedule schedule, IServiceProvider serviceProvider)
        {
            _schedule = schedule;
            _routeService = serviceProvider.GetRequiredService<IRouteService>();
            _trainService = serviceProvider.GetRequiredService<ITrainService>();
            
            InitializeComponent();
            _ = LoadDetailsAsync();
        }

        // ===== INICJALIZACJA KONTROLEK =====
        private void InitializeComponent()
        {
            this.lblTitle = new Label();
            this.lblInfo = new Label();
            this.dgvStops = new DataGridView();
            this.btnClose = new Button();
            this.lblSummary = new Label();

            // ===== lblTitle =====
            this.lblTitle.Text = "📋 Rozkład jazdy";
            this.lblTitle.Location = new Point(20, 20);
            this.lblTitle.Size = new Size(400, 35);
            this.lblTitle.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            this.lblTitle.ForeColor = Color.DarkBlue;

            // ===== lblInfo =====
            this.lblInfo.Text = "";
            this.lblInfo.Location = new Point(20, 65);
            this.lblInfo.Size = new Size(550, 60);
            this.lblInfo.Font = new Font("Segoe UI", 10F);
            this.lblInfo.ForeColor = Color.DarkGray;

            // ===== dgvStops =====
            this.dgvStops.Location = new Point(20, 140);
            this.dgvStops.Size = new Size(550, 250);
            this.dgvStops.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvStops.ReadOnly = true;
            this.dgvStops.RowHeadersVisible = false;
            this.dgvStops.BackgroundColor = Color.White;
            this.dgvStops.AlternatingRowsDefaultCellStyle.BackColor = Color.LightGray;

            // ===== lblSummary =====
            this.lblSummary.Text = "";
            this.lblSummary.Location = new Point(20, 405);
            this.lblSummary.Size = new Size(550, 40);
            this.lblSummary.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            this.lblSummary.ForeColor = Color.DarkGreen;

            // ===== btnClose =====
            this.btnClose.Text = "Zamknij";
            this.btnClose.Location = new Point(240, 460);
            this.btnClose.Size = new Size(100, 35);
            this.btnClose.BackColor = Color.LightGray;
            this.btnClose.Click += (s, e) => this.Close();

            // ===== Form =====
            this.Text = "📋 Rozkład jazdy - szczegóły";
            this.Size = new Size(610, 540);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Dodaj kontrolki
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.lblInfo);
            this.Controls.Add(this.dgvStops);
            this.Controls.Add(this.lblSummary);
            this.Controls.Add(this.btnClose);
        }

        // ===== ŁADOWANIE SZCZEGÓŁÓW =====
        private async System.Threading.Tasks.Task LoadDetailsAsync()
        {
            try
            {
                var route = await _routeService.GetRouteByIdAsync(_schedule.RouteId);
                var train = await _trainService.GetTrainByIdAsync(_schedule.TrainId);

                if (route == null || train == null)
                {
                    lblInfo.Text = "❌ Nie można załadować danych trasy lub pociągu.";
                    lblInfo.ForeColor = Color.Red;
                    return;
                }

                // Informacje o kursie
                lblInfo.Text = $"🚂 Pociąg: {train.Name} ({train.Model}, Vmax: {train.VMax} km/h)\n" +
                              $"🛤️ Trasa: {route.Name}\n" +
                              $"🕐 Odjazd: {_schedule.DepartureTime:hh\\:mm} | " +
                              $"{(route.IsActive ? "✅ Aktywna" : "❌ Nieaktywna")}";

                // Generuj rozkład
                var stops = route.RouteStations.OrderBy(rs => rs.StopOrder).ToList();
                var currentTime = _schedule.DepartureTime;
                var data = new List<object>();
                int totalTravelTime = 0;
                int totalStopTime = 0;

                for (int i = 0; i < stops.Count; i++)
                {
                    var stop = stops[i];
                    var stationName = stop.Station?.Name ?? $"Stacja {stop.StationId}";
                    
                    // Czas przyjazdu (dla pierwszej stacji = czas odjazdu)
                    var arrivalTime = currentTime;
                    
                    // Czas odjazdu (po postoju)
                    var departureTime = currentTime.Add(TimeSpan.FromMinutes(stop.StopDuration));
                    
                    // Czas przejazdu z poprzedniej stacji
                    var travelTime = _schedule.TravelTimes
                        .FirstOrDefault(tt => tt.RouteStationId == stop.Id);
                    
                    // Określ typ przystanku
                    string stopType;
                    if (i == 0)
                        stopType = "🚀 Start";
                    else if (i == stops.Count - 1)
                        stopType = "🏁 Koniec";
                    else if (travelTime != null && travelTime.TravelTimeMinutes > 0)
                        stopType = "📍 Przystanek";
                    else
                        stopType = "📍 Przystanek";

                    // Czy to powrót do stacji początkowej?
                    bool isReturn = (i > 0 && i == stops.Count - 1 && 
                                    stops.First().StationId == stop.StationId);

                    if (isReturn)
                        stopType = "🔄 Powrót";

                    data.Add(new
                    {
                        Lp = i + 1,
                        Stacja = stationName,
                        Typ = stopType,
                        Przyjazd = arrivalTime.ToString(@"hh\:mm"),
                        Odjazd = departureTime.ToString(@"hh\:mm"),
                        Postój = $"{stop.StopDuration} min",
                        Przejazd = travelTime?.TravelTimeMinutes ?? 0
                    });

                    // Dodaj czasy
                    totalStopTime += stop.StopDuration;
                    
                    if (travelTime != null)
                    {
                        totalTravelTime += travelTime.TravelTimeMinutes;
                        currentTime = departureTime.Add(TimeSpan.FromMinutes(travelTime.TravelTimeMinutes));
                    }
                    else
                    {
                        currentTime = departureTime;
                    }
                }

                // Wyświetl dane
                dgvStops.DataSource = null;
                dgvStops.DataSource = data;

                // Dostosuj kolumny
                if (dgvStops.Columns.Contains("Lp"))
                    dgvStops.Columns["Lp"].Width = 40;
                if (dgvStops.Columns.Contains("Stacja"))
                    dgvStops.Columns["Stacja"].Width = 120;
                if (dgvStops.Columns.Contains("Typ"))
                    dgvStops.Columns["Typ"].Width = 80;
                if (dgvStops.Columns.Contains("Przyjazd"))
                    dgvStops.Columns["Przyjazd"].Width = 60;
                if (dgvStops.Columns.Contains("Odjazd"))
                    dgvStops.Columns["Odjazd"].Width = 60;
                if (dgvStops.Columns.Contains("Postój"))
                    dgvStops.Columns["Postój"].Width = 60;
                if (dgvStops.Columns.Contains("Przejazd"))
                    dgvStops.Columns["Przejazd"].Width = 60;

                // Podsumowanie
                int totalMinutes = totalTravelTime + totalStopTime;
                int hours = totalMinutes / 60;
                int minutes = totalMinutes % 60;

                lblSummary.Text = $"⏱️ Całkowity czas: {hours}h {minutes}min " +
                                 $"| 🚆 Czas jazdy: {totalTravelTime}min " +
                                 $"| 🚉 Postoje: {totalStopTime}min " +
                                 $"| {(route.IsActive ? "✅ Kurs aktywny" : "❌ Kurs nieaktywny")}";
                
                // Kolor w zależności od statusu
                lblSummary.ForeColor = route.IsActive ? Color.DarkGreen : Color.Red;
            }
            catch (Exception ex)
            {
                lblInfo.Text = $"❌ Błąd ładowania szczegółów: {ex.Message}";
                lblInfo.ForeColor = Color.Red;
            }
        }
    }
}