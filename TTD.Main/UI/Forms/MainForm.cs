using System;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;
using TTD.Core.Interfaces;

namespace TTD.Main.UI.Forms
{
    public partial class MainForm : Form
    {
        private readonly IServiceProvider _serviceProvider;

        public MainForm(IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _serviceProvider = serviceProvider;
            this.Text = "OpenTTD Manager - Panel sterowania";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new System.Drawing.Size(800, 600);
        }

        private void InitializeComponent()
        {
            // Przyciski do zarządzania
            this.btnTrains = new Button { Text = "🚂 Zarządzaj pociągami", Location = new System.Drawing.Point(250, 100), Size = new System.Drawing.Size(300, 60), Font = new System.Drawing.Font("Segoe UI", 14F) };
            this.btnStations = new Button { Text = "🏢 Zarządzaj stacjami", Location = new System.Drawing.Point(250, 180), Size = new System.Drawing.Size(300, 60), Font = new System.Drawing.Font("Segoe UI", 14F) };
            this.btnRoutes = new Button { Text = "🛤️ Zarządzaj trasami", Location = new System.Drawing.Point(250, 260), Size = new System.Drawing.Size(300, 60), Font = new System.Drawing.Font("Segoe UI", 14F) };
            this.btnSchedules = new Button { Text = "🕐 Zarządzaj rozkładami", Location = new System.Drawing.Point(250, 340), Size = new System.Drawing.Size(300, 60), Font = new System.Drawing.Font("Segoe UI", 14F) };
            this.btnExit = new Button { Text = "Zamknij", Location = new System.Drawing.Point(250, 440), Size = new System.Drawing.Size(300, 40), Font = new System.Drawing.Font("Segoe UI", 12F) };

            // Obsługa kliknięć
            this.btnTrains.Click += (s, e) => new TrainForm(_serviceProvider).ShowDialog();
            this.btnStations.Click += (s, e) => new StationForm(_serviceProvider).ShowDialog();
            this.btnRoutes.Click += (s, e) => new RouteForm(_serviceProvider).ShowDialog();
            this.btnSchedules.Click += (s, e) => new ScheduleForm(_serviceProvider).ShowDialog();
            this.btnExit.Click += (s, e) => this.Close();

            // Tytuł
            this.lblTitle = new Label { Text = "OpenTTD Manager", Font = new System.Drawing.Font("Segoe UI", 20F, System.Drawing.FontStyle.Bold), Location = new System.Drawing.Point(200, 20), Size = new System.Drawing.Size(400, 50), TextAlign = System.Drawing.ContentAlignment.MiddleCenter };

            // Dodanie kontrolek do okna
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.btnTrains);
            this.Controls.Add(this.btnStations);
            this.Controls.Add(this.btnRoutes);
            this.Controls.Add(this.btnSchedules);
            this.Controls.Add(this.btnExit);
        }

        private Button btnTrains, btnStations, btnRoutes, btnSchedules, btnExit;
        private Label lblTitle;
    }
}