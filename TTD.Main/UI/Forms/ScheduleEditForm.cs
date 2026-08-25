using System;
using System.Windows.Forms;
using TTD.Data.Models;

namespace TTD.Main.UI.Forms
{
    public partial class ScheduleEditForm : Form
    {
        public Schedule Schedule { get; private set; }

        // ===== DEKLARACJE KONTROLEK =====
        private Label lblRouteId;
        private Label lblTrainId;
        private Label lblDeparture;
        private Label lblDuration;
        private Label lblNotes;
        private NumericUpDown numRouteId;
        private NumericUpDown numTrainId;
        private NumericUpDown numDuration;
        private DateTimePicker dtpDeparture;
        private CheckBox chkActive;
        private TextBox txtNotes;
        private Button btnSave;
        private Button btnCancel;

        public ScheduleEditForm(Schedule? schedule)
        {
            InitializeComponent();

            if (schedule != null)
            {
                Schedule = schedule;
                LoadScheduleData();
                this.Text = "✏️ Edytuj kurs";
            }
            else
            {
                Schedule = new Schedule();
                this.Text = "➕ Dodaj nowy kurs";
            }
        }

        private void InitializeComponent()
        {
            this.lblRouteId = new Label();
            this.numRouteId = new NumericUpDown();
            this.lblTrainId = new Label();
            this.numTrainId = new NumericUpDown();
            this.lblDeparture = new Label();
            this.dtpDeparture = new DateTimePicker();
            this.lblDuration = new Label();
            this.numDuration = new NumericUpDown();
            this.chkActive = new CheckBox();
            this.txtNotes = new TextBox();
            this.lblNotes = new Label();
            this.btnSave = new Button();
            this.btnCancel = new Button();

            // Kontrolki
            this.numRouteId.Minimum = 1;
            this.numRouteId.Maximum = 9999;
            this.numTrainId.Minimum = 1;
            this.numTrainId.Maximum = 9999;
            this.numDuration.Minimum = 1;
            this.numDuration.Maximum = 999;
            this.numDuration.Value = 60;

            this.dtpDeparture.Format = DateTimePickerFormat.Time;
            this.dtpDeparture.ShowUpDown = true;
            this.dtpDeparture.Value = DateTime.Now.Date.AddHours(8);

            // btnSave
            this.btnSave.Text = "💾 Zapisz";
            this.btnSave.Location = new System.Drawing.Point(30, 280);
            this.btnSave.Size = new System.Drawing.Size(100, 35);
            this.btnSave.Click += (s, e) => { SaveSchedule(); this.DialogResult = DialogResult.OK; };

            // btnCancel
            this.btnCancel.Text = "❌ Anuluj";
            this.btnCancel.Location = new System.Drawing.Point(140, 280);
            this.btnCancel.Size = new System.Drawing.Size(100, 35);
            this.btnCancel.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; };

            // Układ
            int y = 30;
            int step = 35;

            this.Controls.Add(new Label { Text = "ID Trasy:", Location = new System.Drawing.Point(20, y), Size = new System.Drawing.Size(100, 25) });
            this.Controls.Add(this.numRouteId);
            this.numRouteId.Location = new System.Drawing.Point(130, y);
            y += step;

            this.Controls.Add(new Label { Text = "ID Pociągu:", Location = new System.Drawing.Point(20, y), Size = new System.Drawing.Size(100, 25) });
            this.Controls.Add(this.numTrainId);
            this.numTrainId.Location = new System.Drawing.Point(130, y);
            y += step;

            this.Controls.Add(new Label { Text = "Godzina odjazdu:", Location = new System.Drawing.Point(20, y), Size = new System.Drawing.Size(100, 25) });
            this.Controls.Add(this.dtpDeparture);
            this.dtpDeparture.Location = new System.Drawing.Point(130, y);
            y += step;

            this.Controls.Add(new Label { Text = "Czas przejazdu:", Location = new System.Drawing.Point(20, y), Size = new System.Drawing.Size(100, 25) });
            this.Controls.Add(this.numDuration);
            this.numDuration.Location = new System.Drawing.Point(130, y);
            y += step;

            this.Controls.Add(this.chkActive);
            this.chkActive.Text = "Aktywny";
            this.chkActive.Location = new System.Drawing.Point(130, y);
            y += step;

            this.Controls.Add(new Label { Text = "Uwagi:", Location = new System.Drawing.Point(20, y), Size = new System.Drawing.Size(100, 25) });
            this.Controls.Add(this.txtNotes);
            this.txtNotes.Location = new System.Drawing.Point(130, y);
            this.txtNotes.Size = new System.Drawing.Size(200, 25);
            y += step;

            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnCancel);

            // ScheduleEditForm
            this.Size = new System.Drawing.Size(400, 400);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
        }

        private void LoadScheduleData()
        {
            numRouteId.Value = Schedule.RouteId;
            numTrainId.Value = Schedule.TrainId;
            dtpDeparture.Value = DateTime.Now.Date.Add(Schedule.DepartureTime);
            //numDuration.Value = Schedule.TravelTime;
            chkActive.Checked = Schedule.IsActive;
            txtNotes.Text = Schedule.Notes ?? "";
        }

        private void SaveSchedule()
        {
            Schedule.RouteId = (int)numRouteId.Value;
            Schedule.TrainId = (int)numTrainId.Value;
            Schedule.DepartureTime = dtpDeparture.Value.TimeOfDay;
            //Schedule.TravelTime = (int)numDuration.Value;
            Schedule.IsActive = chkActive.Checked;
            Schedule.Notes = txtNotes.Text;
        }
    }
}