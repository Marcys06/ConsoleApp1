using System;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;
using TTD.Core.Interfaces;
using TTD.Data.Models;

namespace TTD.Main.UI.Forms
{
    public partial class ScheduleForm : Form
    {
        private readonly IScheduleService _scheduleService;
        private readonly IServiceProvider _serviceProvider;
        private Schedule? _selectedSchedule;

        // ===== KONTROLKI =====
        private DataGridView dgvSchedules = null!;
        private Button btnAdd = null!;
        private Button btnEdit = null!;
        private Button btnDelete = null!;
        private Button btnRefresh = null!;
        private Button btnDetails = null!;
        private Button btnGenerateAll = null!;
        private Button btnBatchAdd = null!;
        private ComboBox cbRouteFilter = null!;
        private Label lblRouteFilter = null!;
        private ComboBox cbTrainFilter = null!;
        private Label lblTrainFilter = null!;
        private TextBox txtSearch = null!;
        private Label lblSearch = null!;
        private CheckBox chkShowActiveOnly = null!;

        // ===== KONSTRUKTOR =====
        public ScheduleForm(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _scheduleService = serviceProvider.GetRequiredService<IScheduleService>();
            InitializeComponent();
            _ = LoadSchedulesAsync();
        }

        // ===== INICJALIZACJA KONTROLEK =====
        private void InitializeComponent()
        {
            this.dgvSchedules = new DataGridView();
            this.btnAdd = new Button();
            this.btnEdit = new Button();
            this.btnDelete = new Button();
            this.btnRefresh = new Button();
            this.btnDetails = new Button();
            this.btnGenerateAll = new Button();
            this.btnBatchAdd = new Button();
            this.cbRouteFilter = new ComboBox();
            this.lblRouteFilter = new Label();
            this.cbTrainFilter = new ComboBox();
            this.lblTrainFilter = new Label();
            this.txtSearch = new TextBox();
            this.lblSearch = new Label();
            this.chkShowActiveOnly = new CheckBox();

            // ===== dgvSchedules =====
            this.dgvSchedules.Location = new System.Drawing.Point(20, 100);
            this.dgvSchedules.Size = new System.Drawing.Size(750, 300);
            this.dgvSchedules.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvSchedules.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvSchedules.MultiSelect = false;
            this.dgvSchedules.ReadOnly = true;
            this.dgvSchedules.RowHeadersVisible = false;
            this.dgvSchedules.BackgroundColor = System.Drawing.Color.White;
            this.dgvSchedules.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.LightGray;
            this.dgvSchedules.SelectionChanged += (s, e) => SelectSchedule();

            // ===== btnAdd =====
            this.btnAdd.Text = "➕ Dodaj";
            this.btnAdd.Location = new System.Drawing.Point(20, 420);
            this.btnAdd.Size = new System.Drawing.Size(100, 35);
            this.btnAdd.BackColor = System.Drawing.Color.LightGreen;
            this.btnAdd.Click += BtnAdd_Click!;

            // ===== btnBatchAdd =====
            this.btnBatchAdd.Text = "Dodaj wiele";
            this.btnBatchAdd.Location = new System.Drawing.Point(130, 420);
            this.btnBatchAdd.Size = new System.Drawing.Size(100, 35);
            this.btnBatchAdd.BackColor = System.Drawing.Color.LightBlue;
            this.btnBatchAdd.Click += BtnBatchAdd_Click!;

            // ===== btnEdit =====
            this.btnEdit.Text = "✏️ Edytuj";
            this.btnEdit.Location = new System.Drawing.Point(240, 420);
            this.btnEdit.Size = new System.Drawing.Size(100, 35);
            this.btnEdit.BackColor = System.Drawing.Color.LightYellow;
            this.btnEdit.Click += BtnEdit_Click!;

            // ===== btnDelete =====
            this.btnDelete.Text = "🗑️ Usuń";
            this.btnDelete.Location = new System.Drawing.Point(350, 420);
            this.btnDelete.Size = new System.Drawing.Size(100, 35);
            this.btnDelete.BackColor = System.Drawing.Color.LightCoral;
            this.btnDelete.Click += BtnDelete_Click!;

            // ===== btnRefresh =====
            this.btnRefresh.Text = "🔄 Odśwież";
            this.btnRefresh.Location = new System.Drawing.Point(460, 420);
            this.btnRefresh.Size = new System.Drawing.Size(100, 35);
            this.btnRefresh.Click += (s, e) => _ = LoadSchedulesAsync();

            // ===== btnDetails =====
            this.btnDetails.Text = "📋 Szczegóły";
            this.btnDetails.Location = new System.Drawing.Point(570, 420);
            this.btnDetails.Size = new System.Drawing.Size(100, 35);
            this.btnDetails.BackColor = System.Drawing.Color.LightBlue;
            this.btnDetails.Click += BtnDetails_Click!;

            // ===== btnGenerateAll =====
            this.btnGenerateAll.Text = "⚡ Generuj wszystkie";
            this.btnGenerateAll.Location = new System.Drawing.Point(680, 420);
            this.btnGenerateAll.Size = new System.Drawing.Size(120, 35);
            this.btnGenerateAll.BackColor = System.Drawing.Color.LightGoldenrodYellow;
            this.btnGenerateAll.Click += BtnGenerateAll_Click!;

            // ===== cbRouteFilter =====
            this.cbRouteFilter.Location = new System.Drawing.Point(130, 20);
            this.cbRouteFilter.Size = new System.Drawing.Size(180, 25);
            this.cbRouteFilter.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cbRouteFilter.SelectedIndexChanged += (s, e) => _ = LoadSchedulesAsync();

            // ===== lblRouteFilter =====
            this.lblRouteFilter.Text = "📋 Trasa:";
            this.lblRouteFilter.Location = new System.Drawing.Point(70, 23);
            this.lblRouteFilter.Size = new System.Drawing.Size(60, 20);

            // ===== cbTrainFilter =====
            this.cbTrainFilter.Location = new System.Drawing.Point(380, 20);
            this.cbTrainFilter.Size = new System.Drawing.Size(150, 25);
            this.cbTrainFilter.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cbTrainFilter.SelectedIndexChanged += (s, e) => _ = LoadSchedulesAsync();

            // ===== lblTrainFilter =====
            this.lblTrainFilter.Text = "🚂 Pociąg:";
            this.lblTrainFilter.Location = new System.Drawing.Point(320, 23);
            this.lblTrainFilter.Size = new System.Drawing.Size(60, 20);

            // ===== txtSearch =====
            this.txtSearch.Location = new System.Drawing.Point(620, 20);
            this.txtSearch.Size = new System.Drawing.Size(150, 25);
            this.txtSearch.TextChanged += (s, e) => _ = LoadSchedulesAsync();

            // ===== lblSearch =====
            this.lblSearch.Text = "🔍 Szukaj:";
            this.lblSearch.Location = new System.Drawing.Point(570, 23);
            this.lblSearch.Size = new System.Drawing.Size(50, 20);

            // ===== chkShowActiveOnly =====
            this.chkShowActiveOnly.Text = "✅ Tylko aktywne";
            this.chkShowActiveOnly.Location = new System.Drawing.Point(130, 55);
            this.chkShowActiveOnly.Size = new System.Drawing.Size(120, 25);
            this.chkShowActiveOnly.Checked = true;
            this.chkShowActiveOnly.CheckedChanged += (s, e) => _ = LoadSchedulesAsync();

            // ===== ScheduleForm =====
            this.Text = "🕐 Zarządzanie rozkładami";
            this.Size = new System.Drawing.Size(850, 500);
            this.StartPosition = FormStartPosition.CenterParent;
            this.MinimumSize = new System.Drawing.Size(850, 500);

            // Dodaj kontrolki
            this.Controls.Add(this.dgvSchedules);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.btnBatchAdd);
            this.Controls.Add(this.btnEdit);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.btnDetails);
            this.Controls.Add(this.btnGenerateAll);
            this.Controls.Add(this.cbRouteFilter);
            this.Controls.Add(this.lblRouteFilter);
            this.Controls.Add(this.cbTrainFilter);
            this.Controls.Add(this.lblTrainFilter);
            this.Controls.Add(this.txtSearch);
            this.Controls.Add(this.lblSearch);
            this.Controls.Add(this.chkShowActiveOnly);
        }

        // ===== ŁADOWANIE KURSÓW =====
        private async System.Threading.Tasks.Task LoadSchedulesAsync()
        {
            try
            {
                var schedules = await _scheduleService.GetAllSchedulesAsync();

                // Filtrowanie po aktywności
                if (chkShowActiveOnly.Checked)
                {
                    schedules = schedules.Where(s => s.IsActive);
                }

                // Filtrowanie po trasie
                if (cbRouteFilter.SelectedItem != null && cbRouteFilter.SelectedIndex > 0)
                {
                    int routeId = (int)cbRouteFilter.SelectedValue;
                    schedules = schedules.Where(s => s.RouteId == routeId);
                }

                // Filtrowanie po pociągu
                if (cbTrainFilter.SelectedItem != null && cbTrainFilter.SelectedIndex > 0)
                {
                    int trainId = (int)cbTrainFilter.SelectedValue;
                    schedules = schedules.Where(s => s.TrainId == trainId);
                }

                // Wyszukiwanie
                if (!string.IsNullOrEmpty(txtSearch.Text))
                {
                    var search = txtSearch.Text.ToLower();
                    schedules = schedules.Where(s =>
                        (s.Train?.Name?.ToLower().Contains(search) == true) ||
                        (s.Route?.Name?.ToLower().Contains(search) == true) ||
                        (s.Notes?.ToLower().Contains(search) == true)
                    );
                }

                // Sortowanie
                schedules = schedules.OrderBy(s => s.DepartureTime);

                dgvSchedules.DataSource = null;
                dgvSchedules.DataSource = schedules.Select(s => new
                {
                    s.Id,
                    Trasa = s.Route?.Name ?? "Brak",
                    Pociag = s.Train?.Name ?? "Brak",
                    Odjazd = s.DepartureTime.ToString(@"hh\:mm"),
                    Postoje = s.Route?.RouteStations?.Count ?? 0,
                    Czas = CalculateTotalTime(s),
                    Aktywny = s.IsActive ? "✅" : "❌",
                    Uwagi = s.Notes ?? ""
                }).ToList();

                // Ukryj kolumnę ID
                if (dgvSchedules.Columns.Contains("Id"))
                    dgvSchedules.Columns["Id"].Visible = false;

                // Dostosuj kolumny
                if (dgvSchedules.Columns.Contains("Czas"))
                    dgvSchedules.Columns["Czas"].HeaderText = "Czas (min)";

                dgvSchedules.ClearSelection();
                _selectedSchedule = null;

                // Załaduj filtry
                await LoadFiltersAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd ładowania kursów: {ex.Message}", "Błąd",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ===== OBLICZANIE CAŁKOWITEGO CZASU =====
        private string CalculateTotalTime(Schedule schedule)
        {
            if (schedule.Route == null) return "0";

            int total = 0;
            var stops = schedule.Route.RouteStations.OrderBy(rs => rs.StopOrder).ToList();

            for (int i = 0; i < stops.Count - 1; i++)
            {
                var travelTime = schedule.TravelTimes
                    .FirstOrDefault(tt => tt.RouteStationId == stops[i].Id);

                total += travelTime?.TravelTimeMinutes ?? 30;
                total += stops[i].StopDuration;
            }

            if (stops.Any())
                total += stops.Last().StopDuration;

            return total.ToString();
        }

        // ===== ŁADOWANIE FILTRÓW =====
        private async System.Threading.Tasks.Task LoadFiltersAsync()
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var routeService = scope.ServiceProvider.GetRequiredService<IRouteService>();
                var trainService = scope.ServiceProvider.GetRequiredService<ITrainService>();

                // Filtry tras
                var routes = await routeService.GetAllRoutesAsync();
                var routeItems = routes.Select(r => new { r.Id, r.Name }).ToList();
                routeItems.Insert(0, new { Id = 0, Name = "Wszystkie" });

                cbRouteFilter.DataSource = null;
                cbRouteFilter.DisplayMember = "Name";
                cbRouteFilter.ValueMember = "Id";
                cbRouteFilter.DataSource = routeItems;
                cbRouteFilter.SelectedIndex = 0;

                // Filtry pociągów
                var trains = await trainService.GetAllTrainsAsync();
                var trainItems = trains.Select(t => new { t.Id, t.Name }).ToList();
                trainItems.Insert(0, new { Id = 0, Name = "Wszystkie" });

                cbTrainFilter.DataSource = null;
                cbTrainFilter.DisplayMember = "Name";
                cbTrainFilter.ValueMember = "Id";
                cbTrainFilter.DataSource = trainItems;
                cbTrainFilter.SelectedIndex = 0;
            }
            catch
            {
                // Ignoruj błędy - filtry pozostaną puste
            }
        }

        // ===== WYBÓR KURSU =====
        private void SelectSchedule()
        {
            if (dgvSchedules.SelectedRows.Count > 0)
            {
                int id = (int)dgvSchedules.SelectedRows[0].Cells["Id"].Value;
                _selectedSchedule = new Schedule { Id = id };
            }
        }

        // ===== DODAWANIE KURSU (POJEDYNCZY) =====
        private async void BtnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                var form = new ScheduleEditForm(null, _serviceProvider);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    await _scheduleService.AddScheduleAsync(form.Schedule);
                    await LoadSchedulesAsync();
                    MessageBox.Show("✅ Kurs został dodany!", "Sukces",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd dodawania kursu: {ex.Message}", "Błąd",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ===== DODAWANIE WIELU KURSÓW =====
        private async void BtnBatchAdd_Click(object sender, EventArgs e)
        {
            try
            {
                var form = new BatchScheduleForm(_serviceProvider);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    await LoadSchedulesAsync();
                    MessageBox.Show($"✅ Dodano {form.GeneratedSchedules.Count} kursów!",
                        "Sukces", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd dodawania wielu kursów: {ex.Message}", "Błąd",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ===== EDYCJA KURSU =====
        private async void BtnEdit_Click(object sender, EventArgs e)
        {
            if (_selectedSchedule == null)
            {
                MessageBox.Show("Wybierz kurs do edycji.", "Informacja",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                var schedule = await _scheduleService.GetScheduleByIdAsync(_selectedSchedule.Id);
                if (schedule != null)
                {
                    var form = new ScheduleEditForm(schedule, _serviceProvider);
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        await _scheduleService.UpdateScheduleAsync(form.Schedule);
                        await LoadSchedulesAsync();
                        MessageBox.Show("✅ Kurs został zaktualizowany!", "Sukces",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd edycji kursu: {ex.Message}", "Błąd",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ===== USUWANIE KURSU =====
        private async void BtnDelete_Click(object sender, EventArgs e)
        {
            if (_selectedSchedule == null)
            {
                MessageBox.Show("Wybierz kurs do usunięcia.", "Informacja",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var result = MessageBox.Show("Czy na pewno chcesz usunąć wybrany kurs?",
                "Potwierdzenie", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                try
                {
                    await _scheduleService.DeleteScheduleAsync(_selectedSchedule.Id);
                    await LoadSchedulesAsync();
                    MessageBox.Show("✅ Kurs został usunięty!", "Sukces",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Błąd usuwania kursu: {ex.Message}", "Błąd",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // ===== SZCZEGÓŁY KURSU =====
        private async void BtnDetails_Click(object sender, EventArgs e)
        {
            if (_selectedSchedule == null)
            {
                MessageBox.Show("Wybierz kurs do wyświetlenia.", "Informacja",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                var schedule = await _scheduleService.GetScheduleByIdAsync(_selectedSchedule.Id);
                if (schedule != null)
                {
                    var form = new ScheduleDetailsForm(schedule, _serviceProvider);
                    form.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd wyświetlania szczegółów: {ex.Message}", "Błąd",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ===== GENERUJ WSZYSTKIE CZASY =====
        private async void BtnGenerateAll_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show(
                "Czy na pewno chcesz wygenerować/odświeżyć czasy przejazdu dla wszystkich kursów?\n" +
                "Może to zająć chwilę.",
                "Potwierdzenie",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
                return;

            try
            {
                this.Cursor = Cursors.WaitCursor;
                btnGenerateAll.Enabled = false;
                btnGenerateAll.Text = "⏳ Generowanie...";

                await RegenerateAllTravelTimesAsync();

                MessageBox.Show("✅ Czasy przejazdu zostały odświeżone dla wszystkich kursów!",
                    "Sukces", MessageBoxButtons.OK, MessageBoxIcon.Information);

                await LoadSchedulesAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd generowania czasów: {ex.Message}", "Błąd",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
                btnGenerateAll.Enabled = true;
                btnGenerateAll.Text = "⚡ Generuj wszystkie";
            }
        }

        // ===== REGENERACJA CZASÓW DLA WSZYSTKICH KURSÓW =====
        private async System.Threading.Tasks.Task RegenerateAllTravelTimesAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var trainService = scope.ServiceProvider.GetRequiredService<ITrainService>();
            var routeService = scope.ServiceProvider.GetRequiredService<IRouteService>();

            var schedules = await _scheduleService.GetAllSchedulesAsync();

            foreach (var schedule in schedules)
            {
                var train = await trainService.GetTrainByIdAsync(schedule.TrainId);
                var route = await routeService.GetRouteByIdAsync(schedule.RouteId);

                if (train == null || route == null)
                    continue;

                // Usuń stare czasy
                schedule.TravelTimes.Clear();

                // Wygeneruj nowe czasy
                var stops = route.RouteStations.OrderBy(rs => rs.StopOrder).ToList();
                for (int i = 0; i < stops.Count - 1; i++)
                {
                    var from = stops[i];
                    var to = stops[i + 1];

                    int distance = to.DistanceFromPrevious ?? 50;
                    int travelTime = TTD.Core.Services.TimeCalculator.CalculateTravelTime(distance, train.VMax);

                    schedule.TravelTimes.Add(new ScheduleTravelTime
                    {
                        RouteStationId = from.Id,
                        TravelTimeMinutes = travelTime
                    });
                }

                await _scheduleService.UpdateScheduleAsync(schedule);
            }
        }
    }
}