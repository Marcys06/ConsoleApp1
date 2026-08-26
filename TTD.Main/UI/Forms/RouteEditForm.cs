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
    public partial class RouteEditForm : Form
    {
        public Route Route { get; private set; }
        private readonly IStationService? _stationService;
        private List<Station> _allStations = new();
        private List<RouteStation> _routeStations = new();

        // ===== KONTROLKI =====
        private Label lblName = null!;
        private TextBox txtName = null!;
        private Label lblStations = null!;
        private ListBox lstAvailableStations = null!;
        private ListBox lstSelectedStations = null!;
        private Button btnAddStation = null!;
        private Button btnRemoveStation = null!;
        private Button btnUp = null!;
        private Button btnDown = null!;
        private Button btnCloseLoop = null!;
        private NumericUpDown numStopDuration = null!;
        private Label lblStopDuration = null!;
        private CheckBox chkActive = null!;
        private Label lblInfo = null!;
        private Label lblRouteType = null!;
        private Button btnSave = null!;
        private Button btnCancel = null!;

        // ===== KONSTRUKTOR =====
        public RouteEditForm(Route? route, IServiceProvider? serviceProvider = null)
        {
            InitializeComponent();

            if (serviceProvider != null)
            {
                _stationService = serviceProvider.GetRequiredService<IStationService>();
            }

            if (route != null)
            {
                Route = route;
                _routeStations = route.RouteStations?.ToList() ?? new List<RouteStation>();
                this.Text = "✏️ Edytuj trasę";
                LoadRouteData();
            }
            else
            {
                Route = new Route { RouteStations = new List<RouteStation>() };
                _routeStations = new List<RouteStation>();
                this.Text = "➕ Dodaj nową trasę";
            }

            _ = LoadStationsAsync();
        }

        // ===== INICJALIZACJA KONTROLEK =====
        private void InitializeComponent()
        {
            // Inicjalizacja kontrolek
            this.lblName = new Label();
            this.txtName = new TextBox();
            this.lblStations = new Label();
            this.lstAvailableStations = new ListBox();
            this.lstSelectedStations = new ListBox();
            this.btnAddStation = new Button();
            this.btnRemoveStation = new Button();
            this.btnUp = new Button();
            this.btnDown = new Button();
            this.btnCloseLoop = new Button();
            this.numStopDuration = new NumericUpDown();
            this.lblStopDuration = new Label();
            this.chkActive = new CheckBox();
            this.lblInfo = new Label();
            this.lblRouteType = new Label();
            this.btnSave = new Button();
            this.btnCancel = new Button();

            // ===== txtName =====
            this.txtName.Location = new Point(130, 30);
            this.txtName.Size = new Size(200, 25);

            // ===== lblName =====
            this.lblName.Text = "Nazwa trasy:";
            this.lblName.Location = new Point(20, 30);
            this.lblName.Size = new Size(100, 25);

            // ===== lblStations =====
            this.lblStations.Text = "Dostępne stacje:";
            this.lblStations.Location = new Point(20, 70);
            this.lblStations.Size = new Size(120, 25);

            // ===== lstAvailableStations =====
            this.lstAvailableStations.Location = new Point(20, 100);
            this.lstAvailableStations.Size = new Size(200, 200);
            this.lstAvailableStations.SelectionMode = SelectionMode.MultiExtended;
            this.lstAvailableStations.Font = new Font("Segoe UI", 10F);

            // ===== btnAddStation =====
            this.btnAddStation.Text = "→";
            this.btnAddStation.Location = new Point(230, 160);
            this.btnAddStation.Size = new Size(40, 30);
            this.btnAddStation.Font = new Font("Segoe UI", 12F);
            this.btnAddStation.Click += BtnAddStation_Click!;

            // ===== btnRemoveStation =====
            this.btnRemoveStation.Text = "←";
            this.btnRemoveStation.Location = new Point(230, 200);
            this.btnRemoveStation.Size = new Size(40, 30);
            this.btnRemoveStation.Font = new Font("Segoe UI", 12F);
            this.btnRemoveStation.Click += BtnRemoveStation_Click!;

            // ===== lstSelectedStations =====
            this.lstSelectedStations.Location = new Point(280, 100);
            this.lstSelectedStations.Size = new Size(200, 200);
            this.lstSelectedStations.Font = new Font("Segoe UI", 10F);

            // ===== btnUp =====
            this.btnUp.Text = "↑";
            this.btnUp.Location = new Point(490, 130);
            this.btnUp.Size = new Size(40, 30);
            this.btnUp.Font = new Font("Segoe UI", 12F);
            this.btnUp.Click += BtnUp_Click!;

            // ===== btnDown =====
            this.btnDown.Text = "↓";
            this.btnDown.Location = new Point(490, 170);
            this.btnDown.Size = new Size(40, 30);
            this.btnDown.Font = new Font("Segoe UI", 12F);
            this.btnDown.Click += BtnDown_Click!;

            // ===== btnCloseLoop =====
            this.btnCloseLoop.Text = "🔄";
            this.btnCloseLoop.Location = new Point(230, 240);
            this.btnCloseLoop.Size = new Size(40, 30);
            this.btnCloseLoop.Font = new Font("Segoe UI", 12F);
            this.btnCloseLoop.Click += BtnCloseLoop_Click!;
            this.btnCloseLoop.Enabled = false;
            this.btnCloseLoop.BackColor = Color.LightGray;

            // ===== numStopDuration =====
            this.numStopDuration.Location = new Point(130, 320);
            this.numStopDuration.Size = new Size(100, 25);
            this.numStopDuration.Minimum = 0;
            this.numStopDuration.Maximum = 120;
            this.numStopDuration.Value = 5;

            // ===== lblStopDuration =====
            this.lblStopDuration.Text = "Postój (min):";
            this.lblStopDuration.Location = new Point(20, 320);
            this.lblStopDuration.Size = new Size(100, 25);

            // ===== chkActive =====
            this.chkActive.Text = "Aktywna";
            this.chkActive.Location = new Point(280, 320);
            this.chkActive.Size = new Size(100, 25);
            this.chkActive.Checked = true;

            // ===== lblInfo =====
            this.lblInfo.Text = "Wybierz stacje z lewej i kliknij → aby dodać";
            this.lblInfo.Location = new Point(20, 350);
            this.lblInfo.Size = new Size(450, 25);
            this.lblInfo.ForeColor = Color.Gray;
            this.lblInfo.Font = new Font("Segoe UI", 9F);

            // ===== lblRouteType =====
            this.lblRouteType.Text = "";
            this.lblRouteType.Location = new Point(20, 375);
            this.lblRouteType.Size = new Size(450, 25);
            this.lblRouteType.Font = new Font("Segoe UI", 10F, FontStyle.Bold);

            // ===== btnSave =====
            this.btnSave.Text = "💾 Zapisz";
            this.btnSave.Location = new Point(130, 410);
            this.btnSave.Size = new Size(100, 35);
            this.btnSave.BackColor = Color.LightGreen;
            this.btnSave.Click += (s, e) => { SaveRoute(); this.DialogResult = DialogResult.OK; };

            // ===== btnCancel =====
            this.btnCancel.Text = "❌ Anuluj";
            this.btnCancel.Location = new Point(240, 410);
            this.btnCancel.Size = new Size(100, 35);
            this.btnCancel.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; };

            // ===== RouteEditForm =====
            this.Size = new Size(600, 520);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Dodaj kontrolki
            this.Controls.Add(this.lblName);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.lblStations);
            this.Controls.Add(this.lstAvailableStations);
            this.Controls.Add(this.btnAddStation);
            this.Controls.Add(this.btnRemoveStation);
            this.Controls.Add(this.lstSelectedStations);
            this.Controls.Add(this.btnUp);
            this.Controls.Add(this.btnDown);
            this.Controls.Add(this.btnCloseLoop);
            this.Controls.Add(this.numStopDuration);
            this.Controls.Add(this.lblStopDuration);
            this.Controls.Add(this.chkActive);
            this.Controls.Add(this.lblInfo);
            this.Controls.Add(this.lblRouteType);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnCancel);
        }

        // ===== ŁADOWANIE STACJI =====
        private async System.Threading.Tasks.Task LoadStationsAsync()
        {
            try
            {
                if (_stationService == null)
                {
                    MessageBox.Show("Usługa stacji nie jest dostępna.", "Błąd",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var stations = await _stationService.GetAllStationsAsync();
                _allStations = stations.ToList();

                if (_allStations.Count == 0)
                {
                    lblInfo.Text = "⚠️ Brak stacji w bazie. Dodaj najpierw stacje!";
                    lblInfo.ForeColor = Color.Orange;
                }
                else
                {
                    lblInfo.Text = $"✅ Załadowano {_allStations.Count} stacji";
                    lblInfo.ForeColor = Color.Green;
                }

                RefreshStationLists();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd ładowania stacji: {ex.Message}", "Błąd",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblInfo.Text = $"❌ Błąd: {ex.Message}";
                lblInfo.ForeColor = Color.Red;
            }
        }

        // ===== ODŚWIEŻANIE LIST =====
        private void RefreshStationLists()
        {
            try
            {
                var selectedIds = _routeStations.Select(rs => rs.StationId).ToList();

                // ===== LISTA DOSTĘPNYCH STACJI =====
                var availableStations = _allStations
                    .Where(s =>
                    {
                        var lastStation = _routeStations.OrderBy(rs => rs.StopOrder).LastOrDefault();
                        if (lastStation != null && lastStation.StationId == s.Id)
                            return false;
                        return true;
                    })
                    .OrderBy(s => s.Name)
                    .ToList();

                lstAvailableStations.DataSource = null;
                lstAvailableStations.DisplayMember = "Name";
                lstAvailableStations.ValueMember = "Id";
                lstAvailableStations.DataSource = availableStations;

                // ============================================================
                // ⭐ TU WŁAŚNIE - LISTA WYBRANYCH STACJI
                // ============================================================
                var selectedStations = _routeStations
                    .OrderBy(rs => rs.StopOrder)
                    .Select(rs =>
                    {
                        var stationName = GetStationName(rs.StationId);
                        var isFirst = rs.StopOrder == 1;
                        var isLast = rs.StopOrder == _routeStations.Count;
                        var suffix = "";

                        var previousOccurrences = _routeStations
                            .OrderBy(r => r.StopOrder)
                            .Where(r => r.StationId == rs.StationId && r.StopOrder < rs.StopOrder)
                            .Count();

                        if (isFirst)
                            suffix = " (start)";
                        else if (isLast && IsLoopRoute())
                            suffix = " (powrót)";
                        else if (previousOccurrences > 0)
                            suffix = $" (powrót #{previousOccurrences + 1})";

                        return new StationDisplayItem
                        {
                            Id = rs.Id,
                            DisplayText = $"{rs.StopOrder}. {stationName}{suffix} (postój {rs.StopDuration} min)"
                        };
                    })
                    .ToList();
                // ============================================================

                lstSelectedStations.DataSource = null;
                lstSelectedStations.DisplayMember = "DisplayText";
                lstSelectedStations.ValueMember = "Id";
                lstSelectedStations.DataSource = selectedStations;

                UpdateCloseLoopButton();
                UpdateInfo();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd odświeżania list: {ex.Message}", "Błąd",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private string GetStationName(int stationId)
        {
            return _allStations.FirstOrDefault(s => s.Id == stationId)?.Name ?? $"Stacja {stationId}";
        }

        // ===== KLASA POMOCNICZA =====
        private class StationDisplayItem
        {
            public int Id { get; set; }
            public string DisplayText { get; set; } = string.Empty;

            public override string ToString() => DisplayText;
        }

        // ===== SPRAWDZANIE CZY TRASA JEST PĘTLĄ =====
        private bool IsLoopRoute()
        {
            if (_routeStations.Count < 2) return false;

            var firstStationId = _routeStations.OrderBy(rs => rs.StopOrder).First().StationId;
            var lastStationId = _routeStations.OrderBy(rs => rs.StopOrder).Last().StationId;

            return firstStationId == lastStationId;
        }

        // ===== AKTUALIZACJA PRZYCISKU "ZAMKNIJ PĘTLĘ" =====
        private void UpdateCloseLoopButton()
        {
            if (_routeStations.Count > 1)
            {
                var first = _routeStations.OrderBy(rs => rs.StopOrder).First();
                var last = _routeStations.OrderBy(rs => rs.StopOrder).Last();

                // Jeśli pierwsza i ostatnia to ta sama stacja - pętla jest zamknięta
                if (first.StationId == last.StationId)
                {
                    btnCloseLoop.Enabled = false;
                    btnCloseLoop.BackColor = Color.LightGray;
                    btnCloseLoop.Text = "✅";
                    btnCloseLoop.ForeColor = Color.Green;
                }
                else
                {
                    btnCloseLoop.Enabled = true;
                    btnCloseLoop.BackColor = Color.LightYellow;
                    btnCloseLoop.Text = "🔄";
                    btnCloseLoop.ForeColor = Color.DarkBlue;
                }
            }
            else
            {
                btnCloseLoop.Enabled = false;
                btnCloseLoop.BackColor = Color.LightGray;
                btnCloseLoop.Text = "🔄";
            }
        }

        // ===== ZAMKNIJ PĘTLĘ =====
        private void BtnCloseLoop_Click(object sender, EventArgs e)
        {
            if (_routeStations.Count < 2)
            {
                MessageBox.Show("Dodaj co najmniej 2 stacje przed zamknięciem pętli.", "Informacja",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var firstStation = _routeStations.OrderBy(rs => rs.StopOrder).First();
            var lastStation = _routeStations.OrderBy(rs => rs.StopOrder).Last();

            if (firstStation.StationId == lastStation.StationId)
            {
                MessageBox.Show("Pętla jest już zamknięta.", "Informacja",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Dodaj pierwszą stację na koniec
            var routeStation = new RouteStation
            {
                StationId = firstStation.StationId,
                StopOrder = _routeStations.Count + 1,
                StopDuration = (int)numStopDuration.Value,
            };

            _routeStations.Add(routeStation);
            RefreshStationLists();

            lblInfo.Text = $"🔄 Pętla zamknięta: {GetStationName(firstStation.StationId)}";
            lblInfo.ForeColor = Color.Green;
        }

        // ===== AKTUALIZACJA INFO =====
        // ===== AKTUALIZACJA INFO =====
        private void UpdateInfo()
        {
            if (_routeStations.Count > 0)
            {
                var sorted = _routeStations.OrderBy(rs => rs.StopOrder).ToList();

                var displayParts = new List<string>();
                var stationCounts = new Dictionary<int, int>();

                foreach (var rs in sorted)
                {
                    var stationName = GetStationName(rs.StationId);

                    if (!stationCounts.ContainsKey(rs.StationId))
                        stationCounts[rs.StationId] = 0;
                    stationCounts[rs.StationId]++;

                    var occurrence = stationCounts[rs.StationId];
                    var suffix = "";

                    if (occurrence == 1 && rs.StopOrder == 1)
                        suffix = " (start)";
                    else if (occurrence > 1)
                        suffix = $" (#{occurrence})";

                    displayParts.Add($"{stationName}{suffix}");
                }

                lblInfo.Text = $"🛤️ Trasa: {string.Join(" → ", displayParts)}";
                lblInfo.ForeColor = Color.DarkBlue;

                var firstId = sorted.First().StationId;
                var lastId = sorted.Last().StationId;

                if (firstId == lastId)
                {
                    lblRouteType.Text = "🔄 Trasa pętlowa (powrót do stacji początkowej)";
                    lblRouteType.ForeColor = Color.Purple;
                }
                else if (sorted.Select(rs => rs.StationId).Distinct().Count() < sorted.Count)
                {
                    lblRouteType.Text = "🔁 Trasa z powrotami (stacje powtarzają się)";
                    lblRouteType.ForeColor = Color.DarkOrange;
                }
                else
                {
                    lblRouteType.Text = "➡️ Trasa jednokierunkowa";
                    lblRouteType.ForeColor = Color.DarkGreen;
                }
            }
            else
            {
                lblInfo.Text = "Wybierz stacje z lewej i kliknij → aby dodać";
                lblInfo.ForeColor = Color.Gray;
                lblRouteType.Text = "";
            }
        }

        // ===== WCZYTANIE DANYCH TRASY =====
        private void LoadRouteData()
        {
            txtName.Text = Route.Name;
            chkActive.Checked = Route.IsActive;

            if (Route.RouteStations != null)
            {
                _routeStations = Route.RouteStations.ToList();
            }
        }

        // ===== ZAPIS TRASY =====
        private void SaveRoute()
        {
            Route.Name = txtName.Text.Trim();
            Route.IsActive = chkActive.Checked;
            Route.RouteStations = _routeStations;

            foreach (var rs in Route.RouteStations)
            {
                rs.RouteId = Route.Id;
            }
        }

        // ===== DODAWANIE STACJI =====
        private void BtnAddStation_Click(object sender, EventArgs e)
        {
            try
            {
                if (lstAvailableStations.SelectedItem == null)
                {
                    MessageBox.Show("Wybierz stację do dodania.", "Informacja",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var selectedStations = lstAvailableStations.SelectedItems
                    .Cast<Station>()
                    .ToList();

                foreach (var station in selectedStations)
                {
                    // ⭐ NOWA LOGIKA: Sprawdź czy to nie jest duplikat bezpośredni
                    var lastStation = _routeStations.OrderBy(rs => rs.StopOrder).LastOrDefault();

                    // Nie pozwól dodać tej samej stacji dwa razy pod rząd
                    if (lastStation != null && lastStation.StationId == station.Id)
                    {
                        MessageBox.Show($"Nie można dodać stacji '{station.Name}' dwa razy pod rząd.",
                            "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        continue;
                    }

                    var routeStation = new RouteStation
                    {
                        StationId = station.Id,
                        StopOrder = _routeStations.Count + 1,
                        StopDuration = (int)numStopDuration.Value,
                    };

                    _routeStations.Add(routeStation);
                }

                RefreshStationLists();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd dodawania stacji: {ex.Message}", "Błąd",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ===== USUWANIE STACJI =====
        private void BtnRemoveStation_Click(object sender, EventArgs e)
        {
            try
            {
                if (lstSelectedStations.SelectedItem == null)
                {
                    MessageBox.Show("Wybierz stację do usunięcia.", "Informacja",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var selectedItem = (StationDisplayItem)lstSelectedStations.SelectedItem;
                var toRemove = _routeStations.FirstOrDefault(rs => rs.Id == selectedItem.Id);

                if (toRemove != null)
                {
                    // Nie pozwól usunąć jedynej stacji
                    if (_routeStations.Count <= 1)
                    {
                        MessageBox.Show("Nie można usunąć ostatniej stacji z trasy.", "Informacja",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    _routeStations.Remove(toRemove);

                    int i = 1;
                    foreach (var rs in _routeStations.OrderBy(rs => rs.StopOrder))
                    {
                        rs.StopOrder = i++;
                    }

                    RefreshStationLists();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd usuwania stacji: {ex.Message}", "Błąd",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ===== PRZESUWANIE W GÓRĘ =====
        private void BtnUp_Click(object sender, EventArgs e)
        {
            try
            {
                if (lstSelectedStations.SelectedItem == null)
                {
                    MessageBox.Show("Wybierz stację do przesunięcia.", "Informacja",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var selectedItem = (StationDisplayItem)lstSelectedStations.SelectedItem;
                var selected = _routeStations.FirstOrDefault(rs => rs.Id == selectedItem.Id);

                if (selected != null && selected.StopOrder > 1)
                {
                    var previous = _routeStations.First(rs => rs.StopOrder == selected.StopOrder - 1);

                    int temp = selected.StopOrder;
                    selected.StopOrder = previous.StopOrder;
                    previous.StopOrder = temp;

                    RefreshStationLists();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd przesuwania: {ex.Message}", "Błąd",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ===== PRZESUWANIE W DÓŁ =====
        private void BtnDown_Click(object sender, EventArgs e)
        {
            try
            {
                if (lstSelectedStations.SelectedItem == null)
                {
                    MessageBox.Show("Wybierz stację do przesunięcia.", "Informacja",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var selectedItem = (StationDisplayItem)lstSelectedStations.SelectedItem;
                var selected = _routeStations.FirstOrDefault(rs => rs.Id == selectedItem.Id);

                if (selected != null && selected.StopOrder < _routeStations.Count)
                {
                    var next = _routeStations.First(rs => rs.StopOrder == selected.StopOrder + 1);

                    int temp = selected.StopOrder;
                    selected.StopOrder = next.StopOrder;
                    next.StopOrder = temp;

                    RefreshStationLists();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd przesuwania: {ex.Message}", "Błąd",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}