using System;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;
using TTD.Core.Interfaces;
using TTD.Data.Models;

namespace TTD.Main.UI.Forms
{
    public partial class RouteForm : Form
    {
        private readonly IRouteService _routeService;
        private Route? _selectedRoute;

        public RouteForm(IServiceProvider serviceProvider)
        {
            _routeService = serviceProvider.GetRequiredService<IRouteService>();
            InitializeComponent();
            LoadRoutes();
        }

        private void InitializeComponent()
        {
            this.dgvRoutes = new DataGridView();
            this.btnAdd = new Button();
            this.btnEdit = new Button();
            this.btnDelete = new Button();
            this.btnRefresh = new Button();
            this.txtSearch = new TextBox();
            this.lblSearch = new Label();

            // dgvRoutes
            this.dgvRoutes.Location = new System.Drawing.Point(20, 50);
            this.dgvRoutes.Size = new System.Drawing.Size(700, 350);
            this.dgvRoutes.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvRoutes.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvRoutes.MultiSelect = false;
            this.dgvRoutes.SelectionChanged += (s, e) => SelectRoute();

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
            this.btnRefresh.Click += (s, e) => LoadRoutes();

            // txtSearch
            this.txtSearch.Location = new System.Drawing.Point(150, 15);
            this.txtSearch.Size = new System.Drawing.Size(200, 25);
            this.txtSearch.TextChanged += (s, e) => LoadRoutes();

            // lblSearch
            this.lblSearch.Text = "🔍 Szukaj:";
            this.lblSearch.Location = new System.Drawing.Point(90, 18);
            this.lblSearch.Size = new System.Drawing.Size(60, 20);

            // RouteForm
            this.Text = "🛤️ Zarządzanie trasami";
            this.Size = new System.Drawing.Size(750, 500);
            this.StartPosition = FormStartPosition.CenterParent;
            this.Controls.Add(this.dgvRoutes);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.btnEdit);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.txtSearch);
            this.Controls.Add(this.lblSearch);
        }

        private async void LoadRoutes()
        {
            var routes = await _routeService.GetAllRoutesAsync();
            
            if (!string.IsNullOrEmpty(txtSearch.Text))
            {
                routes = routes.Where(r => r.Name.Contains(txtSearch.Text, StringComparison.OrdinalIgnoreCase));
            }

            dgvRoutes.DataSource = null;
            dgvRoutes.DataSource = routes.Select(r => new
            {
                r.Id,
                r.Name,
                Aktywna = r.IsActive ? "Tak" : "Nie",
                LiczbaStacji = r.RouteStations?.Count ?? 0
            }).ToList();

            if (dgvRoutes.Columns.Contains("Id"))
                dgvRoutes.Columns["Id"].Visible = false;

            dgvRoutes.ClearSelection();
            _selectedRoute = null;
        }

        private void SelectRoute()
        {
            if (dgvRoutes.SelectedRows.Count > 0)
            {
                int id = (int)dgvRoutes.SelectedRows[0].Cells["Id"].Value;
                _selectedRoute = new Route { Id = id };
            }
        }

        private async void BtnAdd_Click(object sender, EventArgs e)
        {
            var form = new RouteEditForm(null);
            if (form.ShowDialog() == DialogResult.OK)
            {
                await _routeService.AddRouteAsync(form.Route);
                LoadRoutes();
            }
        }

        private async void BtnEdit_Click(object sender, EventArgs e)
        {
            if (_selectedRoute == null)
            {
                MessageBox.Show("Wybierz trasę do edycji.", "Informacja", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var route = await _routeService.GetRouteByIdAsync(_selectedRoute.Id);
            if (route != null)
            {
                var form = new RouteEditForm(route);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    await _routeService.UpdateRouteAsync(form.Route);
                    LoadRoutes();
                }
            }
        }

        private async void BtnDelete_Click(object sender, EventArgs e)
        {
            if (_selectedRoute == null)
            {
                MessageBox.Show("Wybierz trasę do usunięcia.", "Informacja", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var result = MessageBox.Show($"Czy na pewno chcesz usunąć wybraną trasę?", 
                "Potwierdzenie", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                await _routeService.DeleteRouteAsync(_selectedRoute.Id);
                LoadRoutes();
            }
        }

        private DataGridView dgvRoutes;
        private Button btnAdd, btnEdit, btnDelete, btnRefresh;
        private TextBox txtSearch;
        private Label lblSearch;
    }
}