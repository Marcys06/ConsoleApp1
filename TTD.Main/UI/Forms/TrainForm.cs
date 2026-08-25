using System;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;
using TTD.Core.Interfaces;
using TTD.Data.Models;

namespace TTD.Main.UI.Forms
{
    public partial class TrainForm : Form
    {
        private readonly ITrainService _trainService;
        private Train? _selectedTrain;

        public TrainForm(IServiceProvider serviceProvider)
        {
            _trainService = serviceProvider.GetRequiredService<ITrainService>();
            InitializeComponent();
            LoadTrains();
        }

        private void InitializeComponent()
        {
            this.dgvTrains = new DataGridView();
            this.btnAdd = new Button();
            this.btnEdit = new Button();
            this.btnDelete = new Button();
            this.btnRefresh = new Button();
            this.txtSearch = new TextBox();
            this.lblSearch = new Label();

            // dgvTrains
            this.dgvTrains.Location = new System.Drawing.Point(20, 50);
            this.dgvTrains.Size = new System.Drawing.Size(700, 350);
            this.dgvTrains.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvTrains.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvTrains.MultiSelect = false;
            this.dgvTrains.SelectionChanged += (s, e) => SelectTrain();

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
            this.btnRefresh.Click += (s, e) => LoadTrains();

            // txtSearch
            this.txtSearch.Location = new System.Drawing.Point(150, 15);
            this.txtSearch.Size = new System.Drawing.Size(200, 25);
            this.txtSearch.TextChanged += (s, e) => LoadTrains();

            // lblSearch
            this.lblSearch.Text = "🔍 Szukaj:";
            this.lblSearch.Location = new System.Drawing.Point(90, 18);
            this.lblSearch.Size = new System.Drawing.Size(60, 20);

            // TrainForm
            this.Text = "🚂 Zarządzanie pociągami";
            this.Size = new System.Drawing.Size(750, 500);
            this.StartPosition = FormStartPosition.CenterParent;
            this.Controls.Add(this.dgvTrains);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.btnEdit);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.txtSearch);
            this.Controls.Add(this.lblSearch);
        }

        private async void LoadTrains()
        {
            var trains = await _trainService.SearchTrainsAsync(txtSearch.Text);
            
            dgvTrains.DataSource = null;
            dgvTrains.DataSource = trains.Select(t => new
            {
                t.Id,
                t.Name,
                t.Model,
                Vmax = t.VMax,
                Moc = t.Power,
                Masa = t.Weight,
                Rok = t.ModelYear,
                Elektryczny = t.IsElectric ? "Tak" : "Nie"
            }).ToList();

            if (dgvTrains.Columns.Contains("Id"))
                dgvTrains.Columns["Id"].Visible = false;

            dgvTrains.ClearSelection();
            _selectedTrain = null;
        }

        private void SelectTrain()
        {
            if (dgvTrains.SelectedRows.Count > 0)
            {
                int id = (int)dgvTrains.SelectedRows[0].Cells["Id"].Value;
                _selectedTrain = new Train { Id = id };
            }
        }

        private async void BtnAdd_Click(object sender, EventArgs e)
        {
            var form = new TrainEditForm(null);
            if (form.ShowDialog() == DialogResult.OK)
            {
                await _trainService.AddTrainAsync(form.Train);
                LoadTrains();
            }
        }

        private async void BtnEdit_Click(object sender, EventArgs e)
        {
            if (_selectedTrain == null)
            {
                MessageBox.Show("Wybierz pociąg do edycji.", "Informacja", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var train = await _trainService.GetTrainByIdAsync(_selectedTrain.Id);
            if (train != null)
            {
                var form = new TrainEditForm(train);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    await _trainService.UpdateTrainAsync(form.Train);
                    LoadTrains();
                }
            }
        }

        private async void BtnDelete_Click(object sender, EventArgs e)
        {
            if (_selectedTrain == null)
            {
                MessageBox.Show("Wybierz pociąg do usunięcia.", "Informacja", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var result = MessageBox.Show($"Czy na pewno chcesz usunąć wybrany pociąg?", 
                "Potwierdzenie", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                await _trainService.DeleteTrainAsync(_selectedTrain.Id);
                LoadTrains();
            }
        }

        private DataGridView dgvTrains;
        private Button btnAdd, btnEdit, btnDelete, btnRefresh;
        private TextBox txtSearch;
        private Label lblSearch;
    }
}