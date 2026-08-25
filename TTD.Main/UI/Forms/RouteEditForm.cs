using System;
using System.Collections.Generic;
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
        private readonly IStationService _stationService;
        private List<Station> _allStations = new();

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
                LoadRouteData();
                this.Text = "✏️ Edytuj trasę";
            }
            else
            {
                Route = new Route { RouteStations = new List<RouteStation>() };
                this.Text = "➕ Dodaj nową trasę";
            }

            LoadStations();
        }

        private void InitializeComponent()
        {
            this.lblName = new Label();
            this.txtName = new TextBox();
            this.lblStations = new Label();
            this.lstAvailableStations = new ListBox();
            this.lstSelectedStations = new ListBox();
            this.btnAddStation = new Button();
            this.btnRemoveStation = new Button();
            this.btnUp = new Button();
            this.btnDown = new Button();
            this.numStopDuration = new NumericUpDown();
            this.lblStopDuration = new Label();
            this.chkActive = new CheckBox();
            this.btnSave = new Button();
            this.btnCancel = new Button();

            // txtName
            this.txtName.Location = new System.Drawing.Point(130, 30);
            this.txtName.Size = new System.Drawing.Size(200, 25);

            // lblName
            this.lblName.Text = "Nazwa trasy:";
            this.lblName.Location = new System.Drawing.Point(20, 30);
            this.lblName.Size = new System.Drawing.Size(100, 25);

            // lblStations
            this.lblStations.Text = "Dostępne stacje:";
            this.lblStations.Location = new System.Drawing.Point(20, 70);
            this.lblStations.Size = new System.Drawing.Size(120, 25);

            // lstAvailableStations
            this.lstAvailableStations.Location = new System.Drawing.Point(20, 100);
            this.lstAvailableStations.Size = new System.Drawing.Size(200, 200);
            this.lstAvailableStations.SelectionMode = SelectionMode.MultiExtended;

            // btnAddStation
            this.btnAddStation.Text = "→";
            this.btnAddStation.Location = new System.Drawing.Point(230, 170);
            this.btnAddStation.Size = new System.Drawing.Size(40, 30);
            this.btnAddStation.Click += BtnAddStation_Click;

            // btnRemoveStation
            this.btnRemoveStation.Text = "←";
            this.btnRemoveStation.Location = new System.Drawing.Point(230, 210);
            this.btnRemoveStation.Size = new System.Drawing.Size(40, 30);
            this.btnRemoveStation.Click += BtnRemoveStation_Click;

            // lstSelectedStations
            this.lstSelectedStations.Location = new System.Drawing.Point(280, 100);
            this.lstSelectedStations.Size = new System.Drawing.Size(200, 200);

            // btnUp
            this.btnUp.Text = "↑";
            this.btnUp.Location = new System.Drawing.Point(490, 130);
            this.btnUp.Size = new System.Drawing.Size(40, 30);
            this.btnUp.Click += BtnUp_Click;

            // btnDown
            this.btnDown.Text = "↓";
            this.btnDown.Location = new System.Drawing.Point(490, 170);
            this.btnDown.Size = new System.Drawing.Size(40, 30);
            this.btnDown.Click += BtnDown_Click;

            // numStopDuration
            this.numStopDuration.Location = new System.Drawing.Point(130, 320);
            this.numStopDuration.Size = new System.Drawing.Size(100, 25);
            this.numStopDuration.Minimum = 1;
            this.numStopDuration.Maximum = 120;
            this.numStopDuration.Value = 5;

            // lblStopDuration
            this.lblStopDuration.Text = "Postój (min):";
            this.lblStopDuration.Location = new System.Drawing.Point(20, 320);
            this.lblStopDuration.Size = new System.Drawing.Size(100, 25);

            // chkActive
            this.chkActive.Text = "Aktywna";
            this.chkActive.Location = new System.Drawing.Point(280, 320);
            this.chkActive.Size = new System.Drawing.Size(100, 25);
            this.chkActive.Checked = true;

            // btnSave
            this.btnSave.Text = "💾 Zapisz";
            this.btnSave.Location = new System.Drawing.Point(130, 370);
            this.btnSave.Size = new System.Drawing.Size(100, 35);
            this.btnSave.Click += (s, e) => { SaveRoute(); this.DialogResult = DialogResult.OK; };

            // btnCancel
            this.btnCancel.Text = "❌ Anuluj";
            this.btnCancel.Location = new System.Drawing.Point(240, 370);
            this.btnCancel.Size = new System.Drawing.Size(100, 35);
            this.btnCancel.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; };

            // RouteEditForm
            this.Size = new System.Drawing.Size(600, 500);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            this.Controls.Add(this.lblName);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.lblStations);
            this.Controls.Add(this.lstAvailableStations);
            this.Controls.Add(this.btnAddStation);
            this.Controls.Add(this.btnRemoveStation);
            this.Controls.Add(this.lstSelectedStations);
            this.Controls.Add(this.btnUp);
            this.Controls.Add(this.btnDown);
            this.Controls.Add(this.numStopDuration);
            this.Controls.Add(this.lblStopDuration);
            this.Controls.Add(this.chkActive);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnCancel);
        }

        private async void LoadStations()
        {
            if (_stationService != null)
            {
                _allStations = (await _stationService.GetAllStationsAsync()).ToList();
                RefreshStationLists();
            }
        }

        private void RefreshStationLists()
        {
            var selectedIds = Route.RouteStations?.Select(rs => rs.StationId).ToList() ?? new List<int>();
            
            lstAvailableStations.DataSource = null;
            lstAvailableStations.DataSource = _allStations
                .Where(s => !selectedIds.Contains(s.Id))
                .Select(s => s.Name)
                .ToList();

            lstSelectedStations.DataSource = null;
            lstSelectedStations.DataSource = Route.RouteStations?
                .OrderBy(rs => rs.StopOrder)
                .Select(rs => $"{rs.StopOrder}. {rs.Station?.Name ?? rs.StationId.ToString()}")
                .ToList() ?? new List<string>();
        }

        private void LoadRouteData()
        {
            txtName.Text = Route.Name;
            chkActive.Checked = Route.IsActive;
            // RouteStations są ładowane przez LoadStations
        }

        private void SaveRoute()
        {
            Route.Name = txtName.Text;
            Route.IsActive = chkActive.Checked;
            // RouteStations są zapisywane przez serwis
        }

        private void BtnAddStation_Click(object sender, EventArgs e)
        {
            // Implementacja dodawania stacji
        }

        private void BtnRemoveStation_Click(object sender, EventArgs e)
        {
            // Implementacja usuwania stacji
        }

        private void BtnUp_Click(object sender, EventArgs e)
        {
            // Implementacja przesuwania w górę
        }

        private void BtnDown_Click(object sender, EventArgs e)
        {
            // Implementacja przesuwania w dół
        }

        private Label lblName, lblStations, lblStopDuration;
        private TextBox txtName;
        private ListBox lstAvailableStations, lstSelectedStations;
        private Button btnAddStation, btnRemoveStation, btnUp, btnDown;
        private NumericUpDown numStopDuration;
        private CheckBox chkActive;
        private Button btnSave, btnCancel;
    }
}