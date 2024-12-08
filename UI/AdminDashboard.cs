using System;
using System.Windows.Forms;
using System.Drawing;
using DatabaseCursovaya;
using System.Data;
using DatabaseCursovaya.UI;

namespace WindowsFormsApp1
{
    public partial class AdminDashboard : Form
    {
        private readonly DatabaseManager _dbManager;

        private TextBox _searchBox;

        private DataGridView _doctorsGrid;
        private DataGridView _patientsGrid;

        private DataTable _originalPatientsData;
        private DataTable _originalDoctorsData;

        private int _currentTab;

        public AdminDashboard()
        {
            InitializeComponent();
            _dbManager = new DatabaseManager();

            // Настройка формы
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // Проверяем авторизацию и роль
            if (!UserSession.Instance.IsAuthenticated || UserSession.Instance.Role != "admin")
            {
                MessageBox.Show("Доступ запрещен!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }

            InitializeComponents();
            LoadDoctors();
            LoadPatients();

        }

        private void InitializeDoctorsTab(TabPage tab)
        {
            // Создаем панели
            var doctorsButtonPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 40,
                Padding = new Padding(5)
            };

            var searchPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 40
            };

            // Создаем и настраиваем кнопки
            var addButton = new Button { Text = "Добавить" };
            var editButton = new Button { Text = "Редактировать" };
            var deleteButton = new Button { Text = "Удалить" };
            var scheduleButton = new Button { Text = "Расписание" };

            // Привязываем обработчики к существующим методам
            addButton.Click += BtnCreate_Click;
            editButton.Click += BtnEdit_Click;
            deleteButton.Click += BtnDelete_Click;
            scheduleButton.Click += BtnSchedule_Click;

            doctorsButtonPanel.Controls.AddRange(new Control[] { addButton, editButton, deleteButton, scheduleButton });

            // Настройка таблицы докторов
            _doctorsGrid = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AllowUserToAddRows = false,
                ReadOnly = true
            };

            // Добавляем элементы управления на вкладку
            tab.Controls.Add(_doctorsGrid);
            tab.Controls.Add(searchPanel);
            tab.Controls.Add(doctorsButtonPanel);
        }

        private void InitializePatientsTab(TabPage tab)
        {
            // Создаем панели
            var patientsButtonPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 40,
                Padding = new Padding(5)
            };

            var searchPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 40
            };

            // Создаем и настраиваем кнопки
            var addButton = new Button { Text = "Добавить" };
            var editButton = new Button { Text = "Редактировать" };
            var deleteButton = new Button { Text = "Удалить" };
            var appointmentsButton = new Button { Text = "Приемы" };

            // Привязываем обработчики
            addButton.Click += BtnCreatePatient_Click;
            editButton.Click += BtnEditPatient_Click;
            deleteButton.Click += BtnDeletePatient_Click;
            appointmentsButton.Click += BtnPatientAppointments_Click;

            patientsButtonPanel.Controls.AddRange(new Control[] { addButton, editButton, deleteButton, appointmentsButton });

            // Настройка таблицы пациентов
            _patientsGrid = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AllowUserToAddRows = false,
                ReadOnly = true
            };

            // Добавляем элементы управления на вкладку
            tab.Controls.Add(_patientsGrid);
            tab.Controls.Add(searchPanel);
            tab.Controls.Add(patientsButtonPanel);
        }

        private void InitializeComponents()
        {
            tabControl1.TabIndexChanged += (s, args) =>
            {
                this._currentTab = tabControl1.TabIndex;
            };

            // Создаем вкладку для докторов
            TabPage doctorsTab = tabControl1.TabPages[0];
            TabPage patientsTab = tabControl1.TabPages[1];
            // Создаем панель для поиска

            // Создаем поле поиска
            _searchBox = new TextBox
            {
                Width = 200,
                Location = new Point(580, 5),

            };
            _searchBox.TextChanged += SearchBox_TextChanged;


            // Инициализация вкладок
            InitializeDoctorsTab(tabControl1.TabPages[0]);
            InitializePatientsTab(tabControl1.TabPages[1]);
        }

        private void LoadDoctors()
        {
            _originalDoctorsData = _dbManager.GetAllDoctors();
            _doctorsGrid.DataSource = _originalDoctorsData;
        }

        private void LoadPatients()
        {
            _originalPatientsData = _dbManager.GetAllPatients();
            _patientsGrid.DataSource = _originalDoctorsData;
        }

        private void SearchBox_TextChanged(object sender, EventArgs e)
        {
            string searchText = _searchBox.Text.ToLower();

            if (_currentTab == 0)
            {
                if (string.IsNullOrWhiteSpace(searchText))
                {
                    _doctorsGrid.DataSource = _originalDoctorsData;
                    return;
                }

                // Создаем отфильтрованную таблицу
                DataTable filteredTable = _originalDoctorsData.Clone();
                foreach (DataRow row in _originalDoctorsData.Rows)
                {
                    bool found = false;
                    foreach (DataColumn col in _originalDoctorsData.Columns)
                    {
                        if (row[col] != null && row[col].ToString().ToLower().Contains(searchText))
                        {
                            found = true;
                            break;
                        }
                    }
                    if (found)
                    {
                        filteredTable.ImportRow(row);
                    }
                }

                _doctorsGrid.DataSource = filteredTable;
            }

            if (_currentTab == 1)
            {
                if (string.IsNullOrWhiteSpace(searchText))
                {
                    _patientsGrid.DataSource = _originalPatientsData;
                    return;
                }

                // Создаем отфильтрованную таблицу
                DataTable filteredTable = _originalPatientsData.Clone();
                foreach (DataRow row in _originalPatientsData.Rows)
                {
                    bool found = false;
                    foreach (DataColumn col in _originalPatientsData.Columns)
                    {
                        if (row[col] != null && row[col].ToString().ToLower().Contains(searchText))
                        {
                            found = true;
                            break;
                        }
                    }
                    if (found)
                    {
                        filteredTable.ImportRow(row);
                    }
                }

                _patientsGrid.DataSource = filteredTable;
            }



        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (_doctorsGrid.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите врача для удаления", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var row = _doctorsGrid.SelectedRows[0];
            int doctorId = Convert.ToInt32(row.Cells["ID"].Value);
            string doctorName = $"{row.Cells["Фамилия"].Value} {row.Cells["Имя"].Value}";

            if (MessageBox.Show($"Вы действительно хотите удалить врача {doctorName}?",
                             "Подтверждение",
                             MessageBoxButtons.YesNo,
                             MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (_dbManager.DeleteDoctor(doctorId))
                {
                    MessageBox.Show("Врач успешно удален", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadDoctors();
                }
                else
                {
                    MessageBox.Show("Ошибка при удалении врача", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (_doctorsGrid.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите врача для редактирования", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var row = _doctorsGrid.SelectedRows[0];
            int doctorId = Convert.ToInt32(row.Cells["ID"].Value);

            using (var doctorCard = new DoctorCard(doctorId))
            {
                if (doctorCard.ShowDialog() == DialogResult.OK)
                {
                    LoadDoctors();
                }
            }
        }

        private void BtnCreate_Click(object sender, EventArgs e)
        {
            using (var doctorCard = new DoctorCard())
            {
                if (doctorCard.ShowDialog() == DialogResult.OK)
                {
                    LoadDoctors();
                }
            }
        }

        private void BtnSchedule_Click(object sender, EventArgs e)
        {
            if (_doctorsGrid.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите врача для просмотра расписания", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var row = _doctorsGrid.SelectedRows[0];
            int doctorId = Convert.ToInt32(row.Cells["ID"].Value);
            string doctorName = $"{row.Cells["Фамилия"].Value} {row.Cells["Имя"].Value}";

            using (var scheduleForm = new Schedule(doctorId, doctorName))
            {
                scheduleForm.ShowDialog();
            }
        }

        private void BtnDeletePatient_Click(object sender, EventArgs e)
        {
            if (_patientsGrid.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите пациента для удаления", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var row = _patientsGrid.SelectedRows[0];
            int patientId = Convert.ToInt32(row.Cells["ID"].Value);
            string patientName = $"{row.Cells["Фамилия"].Value} {row.Cells["Имя"].Value}";

            if (MessageBox.Show($"Вы действительно хотите удалить пациента {patientName}?",
                             "Подтверждение",
                             MessageBoxButtons.YesNo,
                             MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (_dbManager.DeletePatient(patientId))
                {
                    MessageBox.Show("Пациент успешно удален", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadPatients();
                }
                else
                {
                    MessageBox.Show("Ошибка при удалении пациента", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnEditPatient_Click(object sender, EventArgs e)
        {
            if (_patientsGrid.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите пациента для редактирования", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var row = _patientsGrid.SelectedRows[0];
            int patientId = Convert.ToInt32(row.Cells["ID"].Value);

            using (var patientCard = new PatientCard(patientId))
            {
                if (patientCard.ShowDialog() == DialogResult.OK)
                {
                    LoadPatients();
                }
            }
        }

        private void BtnCreatePatient_Click(object sender, EventArgs e)
        {
            using (var patientCard = new PatientCard())
            {
                if (patientCard.ShowDialog() == DialogResult.OK)
                {
                    LoadPatients();
                }
            }
        }

        private void BtnPatientAppointments_Click(object sender, EventArgs e)
        {
            if (_patientsGrid.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите пациента для просмотра приемов", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var row = _patientsGrid.SelectedRows[0];
            int patientId = Convert.ToInt32(row.Cells["ID"].Value);
            string patientName = $"{row.Cells["Фамилия"].Value} {row.Cells["Имя"].Value}";

            using (var appointmentsForm = new PatientAppointments(patientId, patientName))
            {
                appointmentsForm.ShowDialog();
            }
        }

        private void logoutBtn_Click(object sender, EventArgs e)
        {
            UserSession.Instance.Clear();
            LoginPage loginPage = new LoginPage();
            loginPage.Show();
            loginPage.FormClosed += (s, args) => this.Close();
            this.Hide();
        }
    }
}