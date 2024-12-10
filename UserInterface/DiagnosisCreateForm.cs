using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApp1;

namespace DatabaseCursovaya.UserInterface
{
    public partial class DiagnosisCreateForm : Form
    {
        private TextBox nameTextBox;
        private Button saveButton;
        private Button cancelButton;
        private readonly DatabaseManager _dbManager;

        public DiagnosisCreateForm()
        {
            InitializeComponent();
            _dbManager = new DatabaseManager();
            InitializeControls();
        }

        private void InitializeControls()
        {
            this.Text = "Создание диагноза";
            this.Size = new System.Drawing.Size(300, 150);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;

            Label nameLabel = new Label
            {
                Text = "Название:",
                Location = new System.Drawing.Point(12, 15),
                AutoSize = true
            };

            nameTextBox = new TextBox
            {
                Location = new System.Drawing.Point(12, 35),
                Width = 250
            };

            saveButton = new Button
            {
                Text = "Сохранить",
                DialogResult = DialogResult.OK,
                Location = new System.Drawing.Point(12, 70),
                Width = 100
            };
            saveButton.Click += SaveButton_Click;

            cancelButton = new Button
            {
                Text = "Отмена",
                DialogResult = DialogResult.Cancel,
                Location = new System.Drawing.Point(122, 70),
                Width = 100
            };

            this.Controls.AddRange(new Control[] { nameLabel, nameTextBox, saveButton, cancelButton });
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            string name = nameTextBox.Text.Trim();
            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Введите название диагноза.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (_dbManager.CreateDiagnosis(name))
            {
                MessageBox.Show("Диагноз успешно создан.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Ошибка при создании диагноза.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
