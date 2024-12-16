using DatabaseCursovaya.UserInterface;
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

namespace DatabaseCursovaya.UI
{
    public partial class DoctorPage : Form
    {
        private readonly DatabaseManager _dbManager;
        private readonly int _doctorId;
        private DataGridView _appointmentsGrid;
        private TextBox _searchBox;
        private Button _editVisitButton;

        public DoctorPage()
        {
            InitializeComponent();
            _dbManager = new DatabaseManager();
            // Настройка формы
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            if (!UserSession.Instance.IsAuthenticated || UserSession.Instance.Role != "doctor")
            {
                MessageBox.Show("Доступ запрещен!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }
            else
            {
                Console.WriteLine(UserSession.Instance.RoleId);
                _doctorId = UserSession.Instance.RoleId;
            }

            InitializeControls();
            LoadAppointments();
        }


        private void InitializeControls()
        {
            this.Text = "Приемы пациентов";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Создаем панель для верхней части с текстом и кнопкой выхода
            Panel headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 40,
                Padding = new Padding(5)
            };

            // Добавляем текст
            Label roleLabel = new Label
            {
                Text = "Режим: Врач",
                AutoSize = true,
                Location = new Point(12, 12)
            };

            // Добавляем кнопку выхода
            Button logoutButton = new Button
            {
                Text = "Выйти",
                Width = 100,
                Height = 30,
                Location = new Point(680, 5)
            };
            logoutButton.Click += (s, e) =>
            {
                UserSession.Instance.Clear();
                LoginPage loginPage = new LoginPage();
                loginPage.Show();
                loginPage.FormClosed += (sender, args) => this.Close();
                this.Hide();
            };

            headerPanel.Controls.Add(roleLabel);
            headerPanel.Controls.Add(logoutButton);

            // Создаем основную панель
            Panel mainPanel = new Panel
            {
                Dock = DockStyle.Fill
            };

            // Панель для поиска и кнопок
            Panel topPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 40,
                Padding = new Padding(5)
            };

            // Поиск
            _searchBox = new TextBox
            {
                Width = 200,
                Location = new Point(5, 10),
            };
            _searchBox.TextChanged += SearchBox_TextChanged;

            // Кнопка редактирования
            _editVisitButton = new Button
            {
                Text = "Редактировать визит",
                Width = 150,
                Location = new Point(215, 8)
            };
            _editVisitButton.Click += EditVisitButton_Click;

            topPanel.Controls.AddRange(new Control[] { _searchBox, _editVisitButton });

            // Таблица
            _appointmentsGrid = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AllowUserToAddRows = false,
                ReadOnly = true
            };

            // Важно: сначала добавляем контролы на форму в правильном порядке
            this.Controls.Add(mainPanel);
            mainPanel.Controls.Add(_appointmentsGrid);
            mainPanel.Controls.Add(topPanel);
            this.Controls.Add(headerPanel);
        }

        private void SearchBox_TextChanged(object sender, EventArgs e)
        {
            string searchText = _searchBox.Text.ToLower();
            foreach (DataGridViewRow row in _appointmentsGrid.Rows)
            {
                bool visible = false;
                foreach (DataGridViewCell cell in row.Cells)
                {
                    if (cell.Value != null && cell.Value.ToString().ToLower().Contains(searchText))
                    {
                        visible = true;
                        break;
                    }
                }
                row.Visible = visible;
            }
        }

        private void EditVisitButton_Click(object sender, EventArgs e)
        {
            if (_appointmentsGrid.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите визит для редактирования", "Предупреждение",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var row = _appointmentsGrid.SelectedRows[0];
            int visitId = Convert.ToInt32(row.Cells["visit_id"].Value);

            using (var detailsForm = new VisitDetailsForm(visitId))
            {
                if (detailsForm.ShowDialog() == DialogResult.OK)
                {
                    LoadAppointments();
                }
            }
        }
        private void LoadAppointments()
        {
            _appointmentsGrid.DataSource = _dbManager.GetDoctorAppointments(_doctorId);
        }
    }
}
