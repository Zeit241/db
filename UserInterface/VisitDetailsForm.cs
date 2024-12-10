using System;
using System.Drawing;
using System.Windows.Forms;
using WindowsFormsApp1;

namespace DatabaseCursovaya.UserInterface
{
    public partial class VisitDetailsForm : Form
    {
        private readonly DatabaseManager _dbManager;
        private readonly int _visitId;
        private ComboBox diagnosisComboBox;
        private ComboBox statusComboBox;
        private Button btnNewDiagnosis;
        private Button saveButton;
        private Button cancelButton;

        public VisitDetailsForm(int visitId)
        {
            InitializeComponent();
            _dbManager = new DatabaseManager();
            _visitId = visitId;

            this.Text = "Редактирование визита";
            this.Size = new Size(400, 400);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;

            InitializeControls();
            LoadVisitData();
        }

        private void InitializeControls()
        {
            // Диагноз
            Label diagnosisLabel = new Label
            {
                Text = "Диагноз:",
                Location = new Point(12, 15),
                AutoSize = true
            };

            diagnosisComboBox = new ComboBox
            {
                Location = new Point(12, 35),
                Width = 250,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            btnNewDiagnosis = new Button
            {
                Text = "Новый",
                Location = new Point(270, 34),
                Width = 100
            };
            btnNewDiagnosis.Click += BtnNewDiagnosis_Click;

            // План лечения
            Label treatmentPlanLabel = new Label
            {
                Text = "План лечения:",
                Location = new Point(12, 65),
                AutoSize = true
            };

            TextBox treatmentPlanTextBox = new TextBox
            {
                Location = new Point(12, 85),
                Width = 358,
                Height = 40,
                Multiline = true
            };

            // Рецепт
            Label prescriptionLabel = new Label
            {
                Text = "Рецепт:",
                Location = new Point(12, 135),
                AutoSize = true
            };

            TextBox prescriptionTextBox = new TextBox
            {
                Location = new Point(12, 155),
                Width = 358,
                Height = 40,
                Multiline = true
            };

            // Заметки
            Label notesLabel = new Label
            {
                Text = "Заметки:",
                Location = new Point(12, 205),
                AutoSize = true
            };

            TextBox notesTextBox = new TextBox
            {
                Location = new Point(12, 225),
                Width = 358,
                Height = 40,
                Multiline = true
            };

            // Кнопки
            saveButton = new Button
            {
                Text = "Сохранить",
                DialogResult = DialogResult.OK,
                Location = new Point(12, 160),
                Width = 100
            };
            saveButton.Click += SaveButton_Click;

            cancelButton = new Button
            {
                Text = "Отмена",
                DialogResult = DialogResult.Cancel,
                Location = new Point(122, 160),
                Width = 100
            };

            this.Controls.AddRange(new Control[] {
        diagnosisLabel, diagnosisComboBox, btnNewDiagnosis,
        treatmentPlanLabel, treatmentPlanTextBox,
        prescriptionLabel, prescriptionTextBox,
        notesLabel, notesTextBox,
        saveButton, cancelButton
    });

            LoadDiagnoses();
        }

        private void LoadDiagnoses()
        {
            var diagnoses = _dbManager.GetAllDiagnoses();
            diagnosisComboBox.DataSource = diagnoses;
            diagnosisComboBox.DisplayMember = "name";
            diagnosisComboBox.ValueMember = "diagnosis_id";
        }

        private void LoadVisitData()
        {
            var visitData = _dbManager.GetVisitById(_visitId);
            if (visitData != null)
            {


                if (visitData["Диагноз"] != DBNull.Value)
                {
                    diagnosisComboBox.SelectedValue = visitData["diagnosis_id"];
                }
            }
        }

        private void BtnNewDiagnosis_Click(object sender, EventArgs e)
        {
            using (var diagnosisForm = new DiagnosisCreateForm())
            {
                if (diagnosisForm.ShowDialog() == DialogResult.OK)
                {
                    LoadDiagnoses();
                }
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            int? diagnosisId = diagnosisComboBox.SelectedValue != null ?
                (int?)Convert.ToInt32(diagnosisComboBox.SelectedValue) : null;

            string status = statusComboBox.SelectedItem.ToString();
            string dbStatus = status == "Завершен" ? "completed" :
                             status == "Не явился" ? "no_show" : "waiting";


        }
    }
}
