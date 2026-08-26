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
    public partial class BatchScheduleForm : Form
    {
        private readonly IRouteService _routeService;
        private readonly ITrainService _trainService;
        private readonly IScheduleService _scheduleService;
        private List<Route> _routes = new();
        private List<Train> _trains = new();
        private List<TimeSpan> _departureTimes = new();

        // ===== KONTROLKI =====
        private ComboBox cbRoute = null!;
        private ComboBox cbTrain = null!;
        private DateTimePicker dtpStartTime = null!;
        private DateTimePicker dtpEndTime = null!;
        private NumericUpDown numInterval = null!;
        private CheckBox chkActive = null!;
        private DateTimePicker dtpValidFrom = null!;
        private DateTimePicker dtpValidTo = null!;
        private TextBox txtNotes = null!;
        private Button btnGenerate = null!;
        private Button btnAddTimes = null!;
        private Button btnRemoveTime = null!;
        private Button btnSave = null!;
        private Button btnCancel = null!;
        private ListBox lstDepartureTimes = null!;
        private Label lblRoute = null!;
        private Label lblTrain = null!;
        private Label lblStartTime = null!;
        private Label lblEndTime = null!;
        private Label lblInterval = null!;
        private Label lblTimes = null!;
        private Label lblValidFrom = null!;
        private Label lblValidTo = null!;
        private Label lblNotes = null!;
        private DataGridView dgvPreview = null!;

        public List<Schedule> GeneratedSchedules { get; private set; } = new();

        public BatchScheduleForm(IServiceProvider serviceProvider)
        {
            _routeService = serviceProvider.GetRequiredService<IRouteService>();
            _trainService = serviceProvider.GetRequiredService<ITrainService>();
            _scheduleService = serviceProvider.GetRequiredService<IScheduleService>();

            InitializeComponent();
            _ = LoadDataAsync();
        }

        private void InitializeComponent()
        {
            // ... Inicjalizacja kontrolek ...
            // (podobnie jak w ScheduleEditForm, ale z dodatkowymi polami)
        }

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
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd ładowania danych: {ex.Message}", "Błąd",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ===== GENEROWANIE KURSÓW Z INTERWAŁEM =====
        private void BtnGenerate_Click(object sender, EventArgs e)
        {
            try
            {
                if (cbRoute.SelectedItem == null || cbTrain.SelectedItem == null)
                {
                    MessageBox.Show("Wybierz trasę i pociąg.", "Informacja",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var startTime = dtpStartTime.Value.TimeOfDay;
                var endTime = dtpEndTime.Value.TimeOfDay;
                var interval = (int)numInterval.Value;

                if (startTime >= endTime)
                {
                    MessageBox.Show("Czas końcowy musi być późniejszy niż początkowy.", "Błąd",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                _departureTimes.Clear();

                var currentTime = startTime;
                while (currentTime <= endTime)
                {
                    _departureTimes.Add(currentTime);
                    currentTime = currentTime.Add(TimeSpan.FromMinutes(interval));
                }

                RefreshTimesList();
                UpdatePreview();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd generowania: {ex.Message}", "Błąd",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ===== DODAWANIE NIEREGULARNYCH CZASÓW =====
        private void BtnAddTimes_Click(object sender, EventArgs e)
        {
            try
            {
                var input = Microsoft.VisualBasic.Interaction.InputBox(
                    "Podaj czasy odjazdów (oddzielone spacją lub przecinkiem):\nPrzykład: 8:00, 8:30, 9:15",
                    "Dodaj nieregularne czasy",
                    "",
                    -1, -1
                );

                if (string.IsNullOrEmpty(input))
                    return;

                var parts = input.Split(new[] { ' ', ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
                var added = 0;

                foreach (var part in parts)
                {
                    if (TimeSpan.TryParse(part, out var time))
                    {
                        if (!_departureTimes.Contains(time))
                        {
                            _departureTimes.Add(time);
                            added++;
                        }
                    }
                    else
                    {
                        MessageBox.Show($"Nieprawidłowy format czasu: '{part}'. Użyj formatu HH:MM.", "Błąd",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }

                if (added > 0)
                {
                    _departureTimes.Sort();
                    RefreshTimesList();
                    UpdatePreview();
                    MessageBox.Show($"✅ Dodano {added} czasów odjazdu.", "Sukces",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd dodawania: {ex.Message}", "Błąd",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ===== USUWANIE CZASU =====
        private void BtnRemoveTime_Click(object sender, EventArgs e)
        {
            if (lstDepartureTimes.SelectedIndex < 0)
            {
                MessageBox.Show("Wybierz czas do usunięcia.", "Informacja",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var index = lstDepartureTimes.SelectedIndex;
            _departureTimes.RemoveAt(index);
            RefreshTimesList();
            UpdatePreview();
        }

        // ===== ODŚWIEŻANIE LISTY =====
        private void RefreshTimesList()
        {
            lstDepartureTimes.DataSource = null;
            lstDepartureTimes.DataSource = _departureTimes
                .Select(t => t.ToString(@"hh\:mm"))
                .ToList();
        }

        // ===== PODGLĄD KURSÓW =====
        private void UpdatePreview()
        {
            if (cbRoute.SelectedItem == null || cbTrain.SelectedItem == null)
                return;

            int routeId = (int)cbRoute.SelectedValue;
            int trainId = (int)cbTrain.SelectedValue;

            var route = _routes.FirstOrDefault(r => r.Id == routeId);
            var train = _trains.FirstOrDefault(t => t.Id == trainId);

            if (route == null || train == null)
                return;

            var data = _departureTimes.Select(time => new
            {
                Godzina = time.ToString(@"hh\:mm"),
                Pociag = train.Name,
                Trasa = route.Name,
                Aktywny = chkActive.Checked ? "✅" : "❌",
                Uwagi = txtNotes.Text
            }).ToList();

            dgvPreview.DataSource = null;
            dgvPreview.DataSource = data;
        }

        // ===== ZAPIS KURSÓW =====
        private async void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (cbRoute.SelectedItem == null || cbTrain.SelectedItem == null)
                {
                    MessageBox.Show("Wybierz trasę i pociąg.", "Błąd",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (_departureTimes.Count == 0)
                {
                    MessageBox.Show("Dodaj przynajmniej jeden czas odjazdu.", "Błąd",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int routeId = (int)cbRoute.SelectedValue;
                int trainId = (int)cbTrain.SelectedValue;

                var schedules = new List<Schedule>();
                int added = 0;
                int skipped = 0;

                foreach (var time in _departureTimes)
                {
                    // Sprawdź czy kurs już istnieje
                    var exists = await _scheduleService.IsScheduleUniqueAsync(routeId, time);
                    
                    if (!exists)
                    {
                        skipped++;
                        continue;
                    }

                    var schedule = new Schedule
                    {
                        RouteId = routeId,
                        TrainId = trainId,
                        DepartureTime = time,
                        IsActive = chkActive.Checked,
                        ValidFrom = dtpValidFrom.Checked ? dtpValidFrom.Value : null,
                        ValidTo = dtpValidTo.Checked ? dtpValidTo.Value : null,
                        Notes = txtNotes.Text
                    };

                    schedules.Add(schedule);
                    added++;
                }

                if (schedules.Any())
                {
                    foreach (var schedule in schedules)
                    {
                        await _scheduleService.AddScheduleAsync(schedule);
                    }

                    GeneratedSchedules = schedules;
                    this.DialogResult = DialogResult.OK;

                    MessageBox.Show($"✅ Dodano {added} kursów.\n" +
                                   $"⏭️ Pominięto {skipped} (już istnieją).", 
                        "Sukces", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show($"❌ Nie dodano żadnych kursów.\n" +
                                   $"⏭️ Wszystkie czasy już istnieją ({skipped}).", 
                        "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd zapisu: {ex.Message}", "Błąd",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ===== PRZYCISK ANULUJ =====
        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}