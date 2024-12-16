using System;
using System.Drawing;
using System.Windows.Forms;
using WindowsFormsApp1;

namespace DatabaseCursovaya.UserInterface
{
    public partial class VisitDetailsForm : Form
    {
        public event EventHandler VisitUpdated;
        private readonly DatabaseManager _dbManager;
        private readonly int _visitId;
        private ComboBox diagnosisComboBox;
        private ComboBox statusComboBox;
        private Button btnNewDiagnosis;
        private Button saveButton;
        private Button cancelButton;
        private TextBox treatmentPlanTextBox;
        private TextBox prescriptionTextBox;
        private TextBox notesTextBox;
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

            treatmentPlanTextBox = new TextBox
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

            prescriptionTextBox = new TextBox
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

            notesTextBox = new TextBox
            {
                Location = new Point(12, 225),
                Width = 358,
                Height = 40,
                Multiline = true
            };

            // Статус
            Label statusLabel = new Label
            {
                Text = "Статус:",
                Location = new Point(12, 275),
                AutoSize = true
            };

            statusComboBox = new ComboBox
            {
                Location = new Point(12, 295),
                Width = 358,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            // Добавляем значения статусов
            statusComboBox.Items.AddRange(new[] { "Ожидается", "Не явился", "Принят" });
            statusComboBox.SelectedIndex = 0;

            statusComboBox.SelectedIndexChanged += StatusComboBox_SelectedIndexChanged;

            // Кнопки
            saveButton = new Button
            {
                Text = "Сохранить",
                DialogResult = DialogResult.OK,
                Location = new Point(12, 330),
                Width = 100
            };
            saveButton.Click += SaveButton_Click;

            cancelButton = new Button
            {
                Text = "Отмена",
                DialogResult = DialogResult.Cancel,
                Location = new Point(122, 330),
                Width = 100
            };

            this.Controls.AddRange(new Control[] {
        diagnosisLabel, diagnosisComboBox, btnNewDiagnosis,
        treatmentPlanLabel, treatmentPlanTextBox,
        prescriptionLabel, prescriptionTextBox,
        notesLabel, notesTextBox,
        statusLabel, statusComboBox,
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
                string status = visitData["Статус"].ToString();
                statusComboBox.SelectedItem = status;

                if (status != "Не явился")
                {
                    if (visitData["Диагноз"] != DBNull.Value)
                    {
                        diagnosisComboBox.SelectedValue = visitData["diagnosis_id"];
                    }

                    if (visitData["План лечения"] != DBNull.Value)
                    {
                        treatmentPlanTextBox.Text = visitData["План лечения"].ToString();
                    }

                    if (visitData["Рецепт"] != DBNull.Value)
                    {
                        prescriptionTextBox.Text = visitData["Рецепт"].ToString();
                    }
                }

                if (visitData["Заметки"] != DBNull.Value)
                {
                    notesTextBox.Text = visitData["Заметки"].ToString();
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
            string status = statusComboBox.SelectedItem.ToString();
            int? diagnosisId = null;
            string treatmentPlan = "";
            string prescription = "";
            string notes = notesTextBox.Text.Trim();

            if (status != "Не явился")
            {
                diagnosisId = diagnosisComboBox.SelectedValue != null ?
                    (int?)Convert.ToInt32(diagnosisComboBox.SelectedValue) : null;
                treatmentPlan = treatmentPlanTextBox.Text.Trim();
                prescription = prescriptionTextBox.Text.Trim();

                if (status == "Принят" && diagnosisId == null)
                {
                    MessageBox.Show("Для принятого пациента необходимо указать диагноз",
                        "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            if (_dbManager.UpdateVisit(_visitId, diagnosisId, status, notes, treatmentPlan, prescription, null))
            {
                MessageBox.Show("Данные успешно сохранены", "Успех",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                VisitUpdated?.Invoke(this, EventArgs.Empty);
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                MessageBox.Show("Ошибка при сохранении данных", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void StatusComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (statusComboBox.SelectedItem.ToString() == "Не явился")
            {
                diagnosisComboBox.SelectedIndex = -1;
                diagnosisComboBox.Enabled = false;
                btnNewDiagnosis.Enabled = false;
                treatmentPlanTextBox.Clear();
                treatmentPlanTextBox.Enabled = false;
                prescriptionTextBox.Clear();
                prescriptionTextBox.Enabled = false;
            }
            else
            {
                diagnosisComboBox.Enabled = true;
                btnNewDiagnosis.Enabled = true;
                treatmentPlanTextBox.Enabled = true;
                prescriptionTextBox.Enabled = true;
            }
        }
    }
}
