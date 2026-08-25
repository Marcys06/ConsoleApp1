using TTD.Data.Models;
using System;
using System.Windows.Forms;
using TTD.Data.Models;

namespace TTD.Main.UI.Forms
{
    public partial class StationEditForm : Form
    {
        public Station Station { get; private set; }

        public StationEditForm(Station? station)
        {
            InitializeComponent();

            if (station != null)
            {
                Station = station;
                LoadStationData();
                this.Text = "✏️ Edytuj stację";
            }
            else
            {
                Station = new Station();
                this.Text = "➕ Dodaj nową stację";
            }
        }

        private void InitializeComponent()
        {
            // Etykiety i kontrolki
            this.lblName = new Label { Text = "Nazwa:", Location = new System.Drawing.Point(20, 30), Size = new System.Drawing.Size(100, 25) };
            this.txtName = new TextBox { Location = new System.Drawing.Point(130, 30), Size = new System.Drawing.Size(200, 25) };

            this.lblLat = new Label { Text = "Szerokość:", Location = new System.Drawing.Point(20, 70), Size = new System.Drawing.Size(100, 25) };
            this.numLat = new NumericUpDown { Location = new System.Drawing.Point(130, 70), Size = new System.Drawing.Size(150, 25), Minimum = -90, Maximum = 90, DecimalPlaces = 4 };

            this.lblLng = new Label { Text = "Długość:", Location = new System.Drawing.Point(20, 110), Size = new System.Drawing.Size(100, 25) };
            this.numLng = new NumericUpDown { Location = new System.Drawing.Point(130, 110), Size = new System.Drawing.Size(150, 25), Minimum = -180, Maximum = 180, DecimalPlaces = 4 };

            this.chkPassenger = new CheckBox { Text = "Pasażerska", Location = new System.Drawing.Point(130, 150), Size = new System.Drawing.Size(100, 25) };
            this.chkCargo = new CheckBox { Text = "Towarowa", Location = new System.Drawing.Point(240, 150), Size = new System.Drawing.Size(100, 25) };

            this.btnSave = new Button { Text = "💾 Zapisz", Location = new System.Drawing.Point(30, 200), Size = new System.Drawing.Size(100, 35) };
            this.btnSave.Click += (s, e) => { SaveStation(); this.DialogResult = DialogResult.OK; };

            this.btnCancel = new Button { Text = "❌ Anuluj", Location = new System.Drawing.Point(140, 200), Size = new System.Drawing.Size(100, 35) };
            this.btnCancel.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; };

            // Dodanie kontrolek
            this.Controls.Add(this.lblName);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.lblLat);
            this.Controls.Add(this.numLat);
            this.Controls.Add(this.lblLng);
            this.Controls.Add(this.numLng);
            this.Controls.Add(this.chkPassenger);
            this.Controls.Add(this.chkCargo);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnCancel);

            // StationEditForm
            this.Size = new System.Drawing.Size(400, 300);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
        }

        private void LoadStationData()
        {
            txtName.Text = Station.Name;
            numLat.Value = (decimal)Station.Latitude;
            numLng.Value = (decimal)Station.Longitude;
            chkPassenger.Checked = Station.IsPassenger;
            chkCargo.Checked = Station.IsCargo;
        }

        private void SaveStation()
        {
            Station.Name = txtName.Text;
            Station.Latitude = (double)numLat.Value;
            Station.Longitude = (double)numLng.Value;
            Station.IsPassenger = chkPassenger.Checked;
            Station.IsCargo = chkCargo.Checked;
        }

        private Label lblName, lblLat, lblLng;
        private TextBox txtName;
        private NumericUpDown numLat, numLng;
        private CheckBox chkPassenger, chkCargo;
        private Button btnSave, btnCancel;
    }
}