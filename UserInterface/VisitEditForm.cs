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
    public partial class VisitEditForm : Form
    {
        private TextBox nameTextBox;
        private TextBox descriptionTextBox;
        private TextBox severityTextBox;
        private TextBox recommendationsTextBox;
        private Button saveButton;
        private Button cancelButton;
        private readonly DatabaseManager _dbManager;
        private readonly int _diagnosisId;

        public VisitEditForm(int diagnosisId)
        {
            _diagnosisId = diagnosisId;
            _dbManager = new DatabaseManager();
            InitializeComponents();
            LoadDiagnosisData();
        }

        private void InitializeComponents()
        {
            // Базовая настройка формы
            Text = "Редактирование диагноза";
            Width = 400;
            Height = 500;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            MaximizeBox = false;

            // Добавьте элементы управления и их обработчики
        }

        private void LoadDiagnosisData()
        {
            // Загрузка данных диагноза
        }
    }
}
