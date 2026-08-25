using System;
using System.Windows.Forms;
using TTD.Data.Models;

namespace TTD.Main.UI.Forms
{
    public partial class TrainEditForm : Form
    {
        public Train Train { get; private set; }

        public TrainEditForm(Train? train)
        {
            InitializeComponent();
            
            if (train != null)
            {
                Train = train;
                LoadTrainData();
                this.Text = "✏️ Edytuj pociąg";
            }
            else
            {
                Train = new Train();
                this.Text = "➕ Dodaj nowy pociąg";
            }
        }

        private void InitializeComponent()
        {
            this.lblName = new Label();
            this.txtName = new TextBox();
            this.lblModel = new Label();
            this.txtModel = new TextBox();
            this.lblVMax = new Label();
            this.numVMax = new NumericUpDown();
            this.lblPower = new Label();
            this.numPower = new NumericUpDown();
            this.lblWeight = new Label();
            this.numWeight = new NumericUpDown();
            this.lblYear = new Label();
            this.numYear = new NumericUpDown();
            this.chkElectric = new CheckBox();
            this.btnSave = new Button();
            this.btnCancel = new Button();

            // Kontrolki numeryczne
            this.numVMax.Minimum = 10;
            this.numVMax.Maximum = 600;
            this.numVMax.Value = 120;

            this.numPower.Minimum = 100;
            this.numPower.Maximum = 15000;
            this.numPower.Value = 2000;

            this.numWeight.Minimum = 10;
            this.numWeight.Maximum = 500;
            this.numWeight.Value = 100;

            this.numYear.Minimum = 1900;
            this.numYear.Maximum = 2100;
            this.numYear.Value = 2000;

            // btnSave
            this.btnSave.Text = "💾 Zapisz";
            this.btnSave.Location = new System.Drawing.Point(30, 280);
            this.btnSave.Size = new System.Drawing.Size(100, 35);
            this.btnSave.Click += (s, e) => { SaveTrain(); this.DialogResult = DialogResult.OK; };

            // btnCancel
            this.btnCancel.Text = "❌ Anuluj";
            this.btnCancel.Location = new System.Drawing.Point(140, 280);
            this.btnCancel.Size = new System.Drawing.Size(100, 35);
            this.btnCancel.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; };

            // Układ
            int y = 30;
            int step = 35;
            
            this.Controls.Add(new Label { Text = "Nazwa:", Location = new System.Drawing.Point(20, y), Size = new System.Drawing.Size(100, 25) });
            this.Controls.Add(this.txtName);
            this.txtName.Location = new System.Drawing.Point(130, y);
            this.txtName.Size = new System.Drawing.Size(200, 25);
            y += step;

            this.Controls.Add(new Label { Text = "Model:", Location = new System.Drawing.Point(20, y), Size = new System.Drawing.Size(100, 25) });
            this.Controls.Add(this.txtModel);
            this.txtModel.Location = new System.Drawing.Point(130, y);
            this.txtModel.Size = new System.Drawing.Size(200, 25);
            y += step;

            this.Controls.Add(new Label { Text = "VMax (km/h):", Location = new System.Drawing.Point(20, y), Size = new System.Drawing.Size(100, 25) });
            this.Controls.Add(this.numVMax);
            this.numVMax.Location = new System.Drawing.Point(130, y);
            this.numVMax.Size = new System.Drawing.Size(150, 25);
            y += step;

            this.Controls.Add(new Label { Text = "Moc (kW):", Location = new System.Drawing.Point(20, y), Size = new System.Drawing.Size(100, 25) });
            this.Controls.Add(this.numPower);
            this.numPower.Location = new System.Drawing.Point(130, y);
            this.numPower.Size = new System.Drawing.Size(150, 25);
            y += step;

            this.Controls.Add(new Label { Text = "Masa (t):", Location = new System.Drawing.Point(20, y), Size = new System.Drawing.Size(100, 25) });
            this.Controls.Add(this.numWeight);
            this.numWeight.Location = new System.Drawing.Point(130, y);
            this.numWeight.Size = new System.Drawing.Size(150, 25);
            y += step;

            this.Controls.Add(new Label { Text = "Rok produkcji:", Location = new System.Drawing.Point(20, y), Size = new System.Drawing.Size(100, 25) });
            this.Controls.Add(this.numYear);
            this.numYear.Location = new System.Drawing.Point(130, y);
            this.numYear.Size = new System.Drawing.Size(150, 25);
            y += step;

            this.Controls.Add(this.chkElectric);
            this.chkElectric.Text = "Elektryczny";
            this.chkElectric.Location = new System.Drawing.Point(130, y);
            this.chkElectric.Size = new System.Drawing.Size(150, 25);
            y += step;

            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnCancel);

            // TrainEditForm
            this.Size = new System.Drawing.Size(400, 400);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
        }

        private void LoadTrainData()
        {
            txtName.Text = Train.Name;
            txtModel.Text = Train.Model;
            numVMax.Value = Train.VMax;
            numPower.Value = Train.Power;
            numWeight.Value = Train.Weight;
            numYear.Value = Train.ModelYear;
            chkElectric.Checked = Train.IsElectric;
        }

        private void SaveTrain()
        {
            Train.Name = txtName.Text;
            Train.Model = txtModel.Text;
            Train.VMax = (int)numVMax.Value;
            Train.Power = (int)numPower.Value;
            Train.Weight = (int)numWeight.Value;
            Train.ModelYear = (int)numYear.Value;
            Train.IsElectric = chkElectric.Checked;
        }

        private Label lblName, lblModel, lblVMax, lblPower, lblWeight, lblYear;
        private TextBox txtName, txtModel;
        private NumericUpDown numVMax, numPower, numWeight, numYear;
        private CheckBox chkElectric;
        private Button btnSave, btnCancel;
    }
}