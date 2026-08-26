using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;
using TTD.Core.Interfaces;
using TTD.Data.Models;

namespace TTD.Main.UI.Forms
{
    public partial class ScheduleEditForm : Form
    {
        public Schedule Schedule { get; private set; }
        private readonly IRouteService _routeService;
        private readonly ITrainService _trainService;
        private List<Route> _routes = new();
        private List<Train> _trains = new();

        // ===== KONTROLKI =====
        private ComboBox cbRoute = null!;
        private ComboBox cbTrain = null!;
        private DateTimePicker dtpDeparture = null!;
        private NumericUpDown numTravelTime = null!;
        private CheckBox chkActive = null!;
        private DateTimePicker dtpValidFrom = null!;
        private DateTimePicker dtpValidTo = null!;
        private TextBox txtNotes = null!;
        private Label lblRoute = null!;
        private Label lblTrain = null!;
        private Label lblDeparture = null!;
        private Label lblTravelTime = null!;
        private Label lblValidFrom = null!;
        private Label lblValidTo = null!;
        private Label lblNotes = null!;
        private Button btnSave = null!;
        private Button btnCancel = null!;
        private Button btnGenerate = null!;
        private DataGridView dgvTravelTimes = null!;

        // ===== KONSTRUKTOR =====
        public ScheduleEditForm(Schedule? schedule, IServiceProvider serviceProvider)
        {
            _routeService = serviceProvider.GetRequiredService<IRouteService>();
            _trainService = serviceProvider.GetRequiredService<ITrainService>();

            InitializeComponent();

            if (schedule != null)
            {
                Schedule = schedule;
                this.Text = "✏️ Edytuj kurs";
                LoadScheduleData();
            }
            else
            {
                Schedule = new Schedule
                {
                    IsActive = true,
                    DepartureTime = new TimeSpan(8, 0, 0),
                    TravelTimes = new List<ScheduleTravelTime>()
                };
                this.Text = "➕ Dodaj nowy kurs";
            }

            _ = LoadDataAsync();
        }

        // ===== INICJALIZACJA KONTROLEK =====
        private void InitializeComponent()
        {
            this.cbRoute = new ComboBox();
            this.cbTrain = new ComboBox();
            this.dtpDeparture = new DateTimePicker();
            this.numTravelTime = new NumericUpDown();
            this.chkActive = new CheckBox();
            this.dtpValidFrom = new DateTimePicker();
            this.dtpValidTo = new DateTimePicker();
            this.txtNotes = new TextBox();
            this.lblRoute = new Label();
            this.lblTrain = new Label();
            this.lblDeparture = new Label();
            this.lblTravelTime = new Label();
            this.lblValidFrom = new Label();
            this.lblValidTo = new Label();
            this.lblNotes = new Label();
            this.btnSave = new Button();
            this.btnCancel = new Button();
            this.btnGenerate = new Button();
            this.dgvTravelTimes = new DataGridView();

            int y = 30;
            int step = 35;

            // ===== Trasa =====
            this.lblRoute.Text = "Trasa:";
            this.lblRoute.Location = new Point(20, y);
            this.lblRoute.Size = new Size(100, 25);

            this.cbRoute.Location = new Point(130, y);
            this.cbRoute.Size = new Size(250, 25);
            this.cbRoute.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cbRoute.SelectedIndexChanged += async (s, e) => await LoadTravelTimesAsync();
            y += step;

            // ===== Pociąg =====
            this.lblTrain.Text = "Pociąg:";
            this.lblTrain.Location = new Point(20, y);
            this.lblTrain.Size = new Size(100, 25);

            this.cbTrain.Location = new Point(130, y);
            this.cbTrain.Size = new Size(250, 25);
            this.cbTrain.DropDownStyle = ComboBoxStyle.DropDownList;
            // ⭐ Nie przeliczamy automatycznie - użytkownik ręcznie wpisuje czasy
            y += step;

            // ===== Godzina odjazdu =====
            this.lblDeparture.Text = "Odjazd:";
            this.lblDeparture.Location = new Point(20, y);
            this.lblDeparture.Size = new Size(100, 25);

            this.dtpDeparture.Format = DateTimePickerFormat.Time;
            this.dtpDeparture.ShowUpDown = true;
            this.dtpDeparture.Location = new Point(130, y);
            this.dtpDeparture.Size = new Size(120, 25);
            this.dtpDeparture.Value = DateTime.Today.AddHours(8);
            y += step;

            // ===== Czas przejazdu (⭐ RĘCZNIE WPISYWANY) =====
            this.lblTravelTime.Text = "Czas (min):";
            this.lblTravelTime.Location = new Point(20, y);
            this.lblTravelTime.Size = new Size(100, 25);

            this.numTravelTime.Location = new Point(130, y);
            this.numTravelTime.Size = new Size(80, 25);
            this.numTravelTime.Minimum = 1;
            this.numTravelTime.Maximum = 999;
            this.numTravelTime.Value = 60;
            this.numTravelTime.Enabled = true;  // ⭐ UMOŻLIWIONA EDYCJA
            y += step;

            // ===== Aktywny =====
            this.chkActive.Text = "Aktywny";
            this.chkActive.Location = new Point(130, y);
            this.chkActive.Size = new Size(100, 25);
            this.chkActive.Checked = true;
            y += step;

            // ===== Okres ważności =====
            this.lblValidFrom.Text = "Od:";
            this.lblValidFrom.Location = new Point(20, y);
            this.lblValidFrom.Size = new Size(100, 25);

            this.dtpValidFrom.Format = DateTimePickerFormat.Short;
            this.dtpValidFrom.Location = new Point(130, y);
            this.dtpValidFrom.Size = new Size(120, 25);
            this.dtpValidFrom.Value = DateTime.Today;
            this.dtpValidFrom.Checked = false;
            this.dtpValidFrom.ShowCheckBox = true;
            y += step;

            this.lblValidTo.Text = "Do:";
            this.lblValidTo.Location = new Point(20, y);
            this.lblValidTo.Size = new Size(100, 25);

            this.dtpValidTo.Format = DateTimePickerFormat.Short;
            this.dtpValidTo.Location = new Point(130, y);
            this.dtpValidTo.Size = new Size(120, 25);
            this.dtpValidTo.Value = DateTime.Today.AddMonths(1);
            this.dtpValidTo.Checked = false;
            this.dtpValidTo.ShowCheckBox = true;
            y += step;

            // ===== Uwagi =====
            this.lblNotes.Text = "Uwagi:";
            this.lblNotes.Location = new Point(20, y);
            this.lblNotes.Size = new Size(100, 25);

            this.txtNotes.Location = new Point(130, y);
            this.txtNotes.Size = new Size(250, 25);
            y += step;

            // ===== Przycisk Generuj (⭐ TERAZ USTAWIENIE CZASÓW DLA WSZYSTKICH ODCINKÓW) =====
            this.btnGenerate.Text = "⚡ Ustaw czasy";
            this.btnGenerate.Location = new Point(130, y);
            this.btnGenerate.Size = new Size(120, 30);
            this.btnGenerate.BackColor = Color.LightYellow;
            this.btnGenerate.Click += BtnGenerate_Click!;
            y += step + 10;

            // ===== DataGridView =====
            this.dgvTravelTimes.Location = new Point(20, y);
            this.dgvTravelTimes.Size = new Size(540, 150);
            this.dgvTravelTimes.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvTravelTimes.ReadOnly = true;
            this.dgvTravelTimes.RowHeadersVisible = false;
            this.dgvTravelTimes.BackgroundColor = Color.White;
            y += 160;

            // ===== Przyciski =====
            this.btnSave.Text = "💾 Zapisz";
            this.btnSave.Location = new Point(130, y);
            this.btnSave.Size = new Size(100, 35);
            this.btnSave.BackColor = Color.LightGreen;
            this.btnSave.Click += (s, e) => { SaveSchedule(); this.DialogResult = DialogResult.OK; };

            this.btnCancel.Text = "❌ Anuluj";
            this.btnCancel.Location = new Point(240, y);
            this.btnCancel.Size = new Size(100, 35);
            this.btnCancel.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; };

            // ===== Form =====
            this.Size = new Size(620, 540);
            this.MinimumSize = new Size(620, 480);  // ⭐ DODAJ - minimalny rozmiar
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.Sizable;  // ⭐ ZMIEŃ NA Sizable
            this.MaximizeBox = true;  // ⭐ ZMIEŃ NA true
            this.MinimizeBox = true;  // ⭐ ZMIEŃ NA true

            // Dodaj kontrolki
            this.Controls.Add(this.lblRoute);
            this.Controls.Add(this.cbRoute);
            this.Controls.Add(this.lblTrain);
            this.Controls.Add(this.cbTrain);
            this.Controls.Add(this.lblDeparture);
            this.Controls.Add(this.dtpDeparture);
            this.Controls.Add(this.lblTravelTime);
            this.Controls.Add(this.numTravelTime);
            this.Controls.Add(this.chkActive);
            this.Controls.Add(this.lblValidFrom);
            this.Controls.Add(this.dtpValidFrom);
            this.Controls.Add(this.lblValidTo);
            this.Controls.Add(this.dtpValidTo);
            this.Controls.Add(this.lblNotes);
            this.Controls.Add(this.txtNotes);
            this.Controls.Add(this.btnGenerate);
            this.Controls.Add(this.dgvTravelTimes);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnCancel);
        }

        // ===== ŁADOWANIE DANYCH =====
        private async System.Threading.Tasks.Task LoadDataAsync()
        {
            try
            {
                _routes = (await _routeService.GetAllRoutesAsync()).ToList();
                _trains = (await _trainService.GetAllTrainsAsync()).ToList();

                cbRoute.DataSource = null;
                cbRoute.DisplayMember = "Name";
                cbRoute.ValueMember = "Id";
                cbRoute.DataSource = _routes;

                cbTrain.DataSource = null;
                cbTrain.DisplayMember = "Name";
                cbTrain.ValueMember = "Id";
                cbTrain.DataSource = _trains;

                if (Schedule.RouteId > 0)
                    cbRoute.SelectedValue = Schedule.RouteId;
                if (Schedule.TrainId > 0)
                    cbTrain.SelectedValue = Schedule.TrainId;

                await LoadTravelTimesAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd ładowania danych: {ex.Message}", "Błąd",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ===== ŁADOWANIE CZASÓW PRZEJAZDU =====
        private async System.Threading.Tasks.Task LoadTravelTimesAsync()
        {
            try
            {
                if (cbRoute.SelectedItem == null)
                    return;

                int routeId = (int)cbRoute.SelectedValue;
                var route = _routes.FirstOrDefault(r => r.Id == routeId);

                if (route == null)
                    return;

                // Wyświetl odcinki z czasami (jeśli istnieją)
                DisplayTravelTimes(route);
            }
            catch (Exception ex)
            {
                // Ignoruj
            }
        }

        private void DisplayTravelTimes(Route route)
        {
            var stops = route.RouteStations.OrderBy(rs => rs.StopOrder).ToList();

            var data = new List<object>();
            for (int i = 0; i < stops.Count - 1; i++)
            {
                var from = stops[i];
                var to = stops[i + 1];
                var tt = Schedule.TravelTimes.FirstOrDefault(t => t.RouteStationId == from.Id);

                string fromName = from.Station?.Name ?? $"Stacja {from.StationId}";
                string toName = to.Station?.Name ?? $"Stacja {to.StationId}";

                data.Add(new
                {
                    Odcinek = $"{fromName} → {toName}",
                    Czas = tt?.TravelTimeMinutes ?? 0,
                    Postoj = from.StopDuration
                });
            }

            dgvTravelTimes.DataSource = null;
            dgvTravelTimes.DataSource = data;
        }

        // ===== WCZYTANIE DANYCH KURSU =====
        private void LoadScheduleData()
        {
            if (Schedule.RouteId > 0)
                cbRoute.SelectedValue = Schedule.RouteId;
            if (Schedule.TrainId > 0)
                cbTrain.SelectedValue = Schedule.TrainId;

            dtpDeparture.Value = DateTime.Today.Add(Schedule.DepartureTime);
            chkActive.Checked = Schedule.IsActive;

            if (Schedule.ValidFrom.HasValue)
            {
                dtpValidFrom.Checked = true;
                dtpValidFrom.Value = Schedule.ValidFrom.Value;
            }
            if (Schedule.ValidTo.HasValue)
            {
                dtpValidTo.Checked = true;
                dtpValidTo.Value = Schedule.ValidTo.Value;
            }

            txtNotes.Text = Schedule.Notes ?? "";
        }

        // ===== PRZYCISK USTAW CZASY =====
        private void BtnGenerate_Click(object sender, EventArgs e)
        {
            try
            {
                if (cbRoute.SelectedItem == null)
                {
                    MessageBox.Show("Wybierz trasę.", "Informacja",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                int routeId = (int)cbRoute.SelectedValue;
                var route = _routes.FirstOrDefault(r => r.Id == routeId);

                if (route == null)
                    return;

                var stops = route.RouteStations.OrderBy(rs => rs.StopOrder).ToList();
                int totalTime = 0;

                // ⭐ UŻYTKOWNIK RĘCZNIE WPISUJE CZAS DLA KAŻDEGO ODCINKA
                Schedule.TravelTimes = new List<ScheduleTravelTime>();

                for (int i = 0; i < stops.Count - 1; i++)
                {
                    var from = stops[i];

                    // Zapytaj użytkownika o czas dla każdego odcinka
                    string input = Microsoft.VisualBasic.Interaction.InputBox(
                        $"Podaj czas przejazdu dla odcinka:\n{from.Station?.Name ?? $"Stacja {from.StationId}"} → {stops[i + 1].Station?.Name ?? $"Stacja {stops[i + 1].StationId}"}",
                        "Czas przejazdu",
                        "30",
                        -1, -1
                    );

                    if (!int.TryParse(input, out int travelTime) || travelTime <= 0)
                    {
                        MessageBox.Show("Podaj poprawny czas (liczba minut > 0).", "Błąd",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    Schedule.TravelTimes.Add(new ScheduleTravelTime
                    {
                        RouteStationId = from.Id,
                        TravelTimeMinutes = travelTime
                    });

                    totalTime += travelTime;
                    totalTime += from.StopDuration;
                }

                if (stops.Any())
                    totalTime += stops.Last().StopDuration;

                numTravelTime.Value = totalTime;
                DisplayTravelTimes(route);

                MessageBox.Show($"✅ Czasy przejazdu zostały ustawione!\nCałkowity czas: {totalTime} minut",
                    "Sukces", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd: {ex.Message}", "Błąd",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ===== ZAPIS KURSU =====
        private void SaveSchedule()
        {
            if (cbRoute.SelectedItem == null || cbTrain.SelectedItem == null)
            {
                MessageBox.Show("Wybierz trasę i pociąg.", "Błąd",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // ⭐ SPRAWDŹ CZY SĄ CZASY PRZEJAZDU
            if (Schedule.TravelTimes == null || !Schedule.TravelTimes.Any())
            {
                var result = MessageBox.Show(
                    "Brak czasów przejazdu. Czy chcesz zapisać kurs bez czasów?",
                    "Ostrzeżenie",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (result == DialogResult.No)
                    return;
            }

            Schedule.RouteId = (int)cbRoute.SelectedValue;
            Schedule.TrainId = (int)cbTrain.SelectedValue;
            Schedule.DepartureTime = dtpDeparture.Value.TimeOfDay;
            Schedule.IsActive = chkActive.Checked;
            Schedule.ValidFrom = dtpValidFrom.Checked ? dtpValidFrom.Value : null;
            Schedule.ValidTo = dtpValidTo.Checked ? dtpValidTo.Value : null;
            Schedule.Notes = txtNotes.Text;
        }
    }
}