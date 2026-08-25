using System;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;
using TTD.Core.Interfaces;
using TTD.Data.Models;

namespace TTD.Main.UI.Forms
{
    public partial class StationForm : Form
    {
        private readonly IStationService _stationService;
        private Station? _selectedStation;

        public StationForm(IServiceProvider serviceProvider)
        {
            _stationService = serviceProvider.GetRequiredService<IStationService>();
            InitializeComponent();
            LoadStations();
        }

        private void InitializeComponent()
        {
            this.dgvStations = new DataGridView();
            this.btnAdd = new Button();
            this.btnEdit = new Button();
            this.btnDelete = new Button();
            this.btnRefresh = new Button();
            this.txtSearch = new TextBox();
            this.lblSearch = new Label();

            // dgvStations
            this.dgvStations.Location = new System.Drawing.Point(20, 50);
            this.dgvStations.Size = new System.Drawing.Size(700, 350);
            this.dgvStations.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvStations.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvStations.MultiSelect = false;
            this.dgvStations.SelectionChanged += (s, e) => SelectStation();

            // btnAdd
            this.btnAdd.Text = "➕ Dodaj";
            this.btnAdd.Location = new System.Drawing.Point(20, 420);
            this.btnAdd.Size = new System.Drawing.Size(100, 35);
            this.btnAdd.Click += BtnAdd_Click;

            // btnEdit
            this.btnEdit.Text = "✏️ Edytuj";
            this.btnEdit.Location = new System.Drawing.Point(130, 420);
            this.btnEdit.Size = new System.Drawing.Size(100, 35);
            this.btnEdit.Click += BtnEdit_Click;

            // btnDelete
            this.btnDelete.Text = "🗑️ Usuń";
            this.btnDelete.Location = new System.Drawing.Point(240, 420);
            this.btnDelete.Size = new System.Drawing.Size(100, 35);
            this.btnDelete.Click += BtnDelete_Click;

            // btnRefresh
            this.btnRefresh.Text = "🔄 Odśwież";
            this.btnRefresh.Location = new System.Drawing.Point(350, 420);
            this.btnRefresh.Size = new System.Drawing.Size(100, 35);
            this.btnRefresh.Click += (s, e) => LoadStations();

            // txtSearch
            this.txtSearch.Location = new System.Drawing.Point(150, 15);
            this.txtSearch.Size = new System.Drawing.Size(200, 25);
            this.txtSearch.TextChanged += (s, e) => LoadStations();

            // lblSearch
            this.lblSearch.Text = "🔍 Szukaj:";
            this.lblSearch.Location = new System.Drawing.Point(90, 18);
            this.lblSearch.Size = new System.Drawing.Size(60, 20);

            // StationForm
            this.Text = "🏢 Zarządzanie stacjami";
            this.Size = new System.Drawing.Size(750, 500);
            this.StartPosition = FormStartPosition.CenterParent;
            this.Controls.Add(this.dgvStations);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.btnEdit);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.txtSearch);
            this.Controls.Add(this.lblSearch);
        }

        private async void LoadStations()
        {
            var stations = await _stationService.SearchStationsAsync(txtSearch.Text);
            
            dgvStations.DataSource = null;
            dgvStations.DataSource = stations.Select(s => new
            {
                s.Id,
                s.Name,
                Szerokosc = s.Latitude,
                Dlugosc = s.Longitude,
                Pasażerska = s.IsPassenger ? "Tak" : "Nie",
                Towarowa = s.IsCargo ? "Tak" : "Nie"
            }).ToList();

            if (dgvStations.Columns.Contains("Id"))
                dgvStations.Columns["Id"].Visible = false;

            dgvStations.ClearSelection();
            _selectedStation = null;
        }

        private void SelectStation()
        {
            if (dgvStations.SelectedRows.Count > 0)
            {
                int id = (int)dgvStations.SelectedRows[0].Cells["Id"].Value;
                _selectedStation = new Station { Id = id };
            }
        }

        private async void BtnAdd_Click(object sender, EventArgs e)
        {
            var form = new StationEditForm(null);
            if (form.ShowDialog() == DialogResult.OK)
            {
                await _stationService.AddStationAsync(form.Station);
                LoadStations();
            }
        }

        private async void BtnEdit_Click(object sender, EventArgs e)
        {
            if (_selectedStation == null)
            {
                MessageBox.Show("Wybierz stację do edycji.", "Informacja", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var station = await _stationService.GetStationByIdAsync(_selectedStation.Id);
            if (station != null)
            {
                var form = new StationEditForm(station);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    await _stationService.UpdateStationAsync(form.Station);
                    LoadStations();
                }
            }
        }

        private async void BtnDelete_Click(object sender, EventArgs e)
        {
            if (_selectedStation == null)
            {
                MessageBox.Show("Wybierz stację do usunięcia.", "Informacja", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var result = MessageBox.Show($"Czy na pewno chcesz usunąć wybraną stację?", 
                "Potwierdzenie", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                await _stationService.DeleteStationAsync(_selectedStation.Id);
                LoadStations();
            }
        }

        private DataGridView dgvStations;
        private Button btnAdd, btnEdit, btnDelete, btnRefresh;
        private TextBox txtSearch;
        private Label lblSearch;
    }
}