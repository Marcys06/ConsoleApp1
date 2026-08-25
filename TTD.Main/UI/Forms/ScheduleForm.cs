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
        private Schedule? _selectedSchedule;

        public ScheduleForm(IServiceProvider serviceProvider)
        {
            _scheduleService = serviceProvider.GetRequiredService<IScheduleService>();
            InitializeComponent();
            LoadSchedules();
        }

        private void InitializeComponent()
        {
            this.dgvSchedules = new DataGridView();
            this.btnAdd = new Button();
            this.btnEdit = new Button();
            this.btnDelete = new Button();
            this.btnRefresh = new Button();
            this.txtSearch = new TextBox();
            this.lblSearch = new Label();

            // dgvSchedules
            this.dgvSchedules.Location = new System.Drawing.Point(20, 50);
            this.dgvSchedules.Size = new System.Drawing.Size(700, 350);
            this.dgvSchedules.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvSchedules.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvSchedules.MultiSelect = false;
            this.dgvSchedules.SelectionChanged += (s, e) => SelectSchedule();

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
            this.btnRefresh.Click += (s, e) => LoadSchedules();

            // txtSearch
            this.txtSearch.Location = new System.Drawing.Point(150, 15);
            this.txtSearch.Size = new System.Drawing.Size(200, 25);
            this.txtSearch.TextChanged += (s, e) => LoadSchedules();

            // lblSearch
            this.lblSearch.Text = "🔍 Szukaj:";
            this.lblSearch.Location = new System.Drawing.Point(90, 18);
            this.lblSearch.Size = new System.Drawing.Size(60, 20);

            // ScheduleForm
            this.Text = "🕐 Zarządzanie rozkładami";
            this.Size = new System.Drawing.Size(750, 500);
            this.StartPosition = FormStartPosition.CenterParent;
            this.Controls.Add(this.dgvSchedules);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.btnEdit);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.txtSearch);
            this.Controls.Add(this.lblSearch);
        }

        private async void LoadSchedules()
        {
            var schedules = await _scheduleService.GetAllSchedulesAsync();

            dgvSchedules.DataSource = null;
            dgvSchedules.DataSource = schedules.Select(s => new
            {
                s.Id,
                Trasa = s.Route?.Name ?? "Brak",
                Pociag = s.Train?.Name ?? "Brak",
                Odjazd = s.DepartureTime.ToString(@"hh\:mm"),
                Aktywny = s.IsActive ? "Tak" : "Nie",
                Uwagi = s.Notes
            }).ToList();

            if (dgvSchedules.Columns.Contains("Id"))
                dgvSchedules.Columns["Id"].Visible = false;

            dgvSchedules.ClearSelection();
            _selectedSchedule = null;
        }

        private void SelectSchedule()
        {
            if (dgvSchedules.SelectedRows.Count > 0)
            {
                int id = (int)dgvSchedules.SelectedRows[0].Cells["Id"].Value;
                _selectedSchedule = new Schedule { Id = id };
            }
        }

        private async void BtnAdd_Click(object sender, EventArgs e)
        {
            // Implementacja dodawania
            MessageBox.Show("Funkcja w przygotowaniu.", "Informacja", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private async void BtnEdit_Click(object sender, EventArgs e)
        {
            if (_selectedSchedule == null)
            {
                MessageBox.Show("Wybierz kurs do edycji.", "Informacja", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            MessageBox.Show("Funkcja w przygotowaniu.", "Informacja", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private async void BtnDelete_Click(object sender, EventArgs e)
        {
            if (_selectedSchedule == null)
            {
                MessageBox.Show("Wybierz kurs do usunięcia.", "Informacja", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var result = MessageBox.Show($"Czy na pewno chcesz usunąć wybrany kurs?", 
                "Potwierdzenie", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                await _scheduleService.DeleteScheduleAsync(_selectedSchedule.Id);
                LoadSchedules();
            }
        }

        private DataGridView dgvSchedules;
        private Button btnAdd, btnEdit, btnDelete, btnRefresh;
        private TextBox txtSearch;
        private Label lblSearch;
    }
}