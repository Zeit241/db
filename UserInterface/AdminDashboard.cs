using System;
using System.Windows.Forms;
using System.Drawing;
using DatabaseCursovaya;
using System.Data;
using DatabaseCursovaya.UI;
using DatabaseCursovaya.UserInterface;
using System.Linq;

namespace WindowsFormsApp1
{
    public partial class AdminDashboard : Form
    {
        private readonly DatabaseManager _dbManager;

        private TextBox _doctorsSearchBox;
        private TextBox _patientsSearchBox;

        private DataGridView _doctorsGrid;
        private DataGridView _patientsGrid;
        private DataGridView _diagnosesGrid;

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
            _doctorsSearchBox = new TextBox
            {
                Width = 200,
                Location = new Point(580, 5),
            };
            _doctorsSearchBox.TextChanged += (s, e) => SearchBox_TextChanged(s, e, true);

            // Создаем панели
            Panel doctorsButtonPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 40,
                Padding = new Padding(5)
            };

            // Создаем и настраиваем кнопки
            Button addButton = new Button
            {
                Text = "Добавить",
                Width = 100,
                Height = 30,
                Location = new Point(5, 5)
            };

            Button editButton = new Button
            {
                Text = "Редактировать",
                Width = 100,
                Height = 30,
                Location = new Point(110, 5)
            };

            Button deleteButton = new Button
            {
                Text = "Удалить",
                Width = 100,
                Height = 30,
                Location = new Point(215, 5)
            };

            Button scheduleButton = new Button
            {
                Text = "Расписание",
                Width = 100,
                Height = 30,
                Location = new Point(320, 5)
            };

            Button manageAccountButton = new Button
            {
                Text = "Управление аккаунтом",
                Width = 140,
                Height = 30,
                Location = new Point(425, 5)
            };

            manageAccountButton.Click += BtnManageAccount_Click;

            // Привязываем обработчики
            addButton.Click += BtnCreate_Click;
            editButton.Click += BtnEdit_Click;
            deleteButton.Click += BtnDelete_Click;
            scheduleButton.Click += BtnSchedule_Click;

            // Добавляем кнопки на панель
            doctorsButtonPanel.Controls.AddRange(new Control[] {
                addButton,
                editButton,
                deleteButton,
                scheduleButton,
                manageAccountButton
            });

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
            tab.Controls.Add(_doctorsSearchBox);
            tab.Controls.Add(doctorsButtonPanel);
        }

        private void InitializePatientsTab(TabPage tab)
        {
            _patientsSearchBox = new TextBox
            {
                Width = 200,
                Location = new Point(580, 5),
            };
            _patientsSearchBox.TextChanged += (s, e) => SearchBox_TextChanged(s, e, false);

            // Создаем панели
            Panel patientsButtonPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 40,
                Padding = new Padding(5)
            };

            // Создаем и ��астраиваем кнопки
            Button addButton = new Button
            {
                Text = "Добавить",
                Width = 100,
                Height = 30,
                Location = new Point(5, 5)
            };

            Button editButton = new Button
            {
                Text = "Редактировать",
                Width = 100,
                Height = 30,
                Location = new Point(110, 5)
            };

            Button deleteButton = new Button
            {
                Text = "Удалить",
                Width = 100,
                Height = 30,
                Location = new Point(215, 5)
            };

            Button appointmentsButton = new Button
            {
                Text = "Приемы",
                Width = 100,
                Height = 30,
                Location = new Point(320, 5)
            };

            Button manageAccountButton = new Button
            {
                Text = "Управление аккаунтом",
                Width = 140,
                Height = 30,
                Location = new Point(425, 5)
            };

            // Привязываем обработчики
            addButton.Click += BtnCreatePatient_Click;
            editButton.Click += BtnEditPatient_Click;
            deleteButton.Click += BtnDeletePatient_Click;
            appointmentsButton.Click += BtnPatientAppointments_Click;
            manageAccountButton.Click += BtnManagePatientAccount_Click;

            // Добавляем кнопки на панель
            patientsButtonPanel.Controls.AddRange(new Control[] {
                addButton,
                editButton,
                deleteButton,
                appointmentsButton,
                manageAccountButton
            });

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
            tab.Controls.Add(_patientsSearchBox);
            tab.Controls.Add(patientsButtonPanel);
        }
        private void InitializeDiagnosesTab(TabPage tab)
        {
            // Создаем панель для кнопок
            Panel diagnosesButtonPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 40,
                Padding = new Padding(5)
            };

            // Создаем и настраиваем кнопки
            Button detailsButton = new Button
            {
                Text = "Подробнее",
                Width = 100,
                Height = 30,
                Location = new Point(5, 5)
            };

            Button editButton = new Button
            {
                Text = "Изменить",
                Width = 100,
                Height = 30,
                Location = new Point(110, 5)
            };

            Button deleteButton = new Button
            {
                Text = "Удалить",
                Width = 100,
                Height = 30,
                Location = new Point(215, 5)
            };

            // Привязываем обработчики
            detailsButton.Click += BtnDiagnosisDetails_Click;
            editButton.Click += BtnEditDiagnosis_Click;
            deleteButton.Click += BtnDeleteDiagnosis_Click;

            // Добавляем кнопки на панель
            diagnosesButtonPanel.Controls.AddRange(new Control[] {
        detailsButton,
        editButton,
        deleteButton
    });

            // Настройка таблицы диагнозов
            _diagnosesGrid = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AllowUserToAddRows = false,
                ReadOnly = true
            };

            // Добавляем элементы управления на вкладку
            tab.Controls.Add(_diagnosesGrid);
            tab.Controls.Add(diagnosesButtonPanel);

            // Загружаем данные
            LoadDiagnoses();
        }

        private void LoadDiagnoses()
        {
            _diagnosesGrid.DataSource = _dbManager.GetAllVisits();
            // Настраиваем отображение столбцов
            if (_patientsGrid.Columns.Count > 0)
                _diagnosesGrid.Columns["diagnosis_id"].HeaderText = "ID";
            {
            }
        }

        private void BtnDiagnosisDetails_Click(object sender, EventArgs e)
        {
            if (_diagnosesGrid.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите визит для просмотра", "Предупреждение",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var row = _diagnosesGrid.SelectedRows[0];
            int visitId = Convert.ToInt32(row.Cells["visit_id"].Value);

            using (var detailsForm = new VisitDetailsForm(visitId))
            {
                detailsForm.ShowDialog();
            }
        }

        private void BtnEditDiagnosis_Click(object sender, EventArgs e)
        {
            if (_diagnosesGrid.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите диагноз для редактирования", "Предупреждение",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var row = _diagnosesGrid.SelectedRows[0];
            int diagnosisId = Convert.ToInt32(row.Cells["diagnosis_id"].Value);

            using (var editForm = new VisitEditForm(diagnosisId))
            {
                if (editForm.ShowDialog() == DialogResult.OK)
                {
                    LoadDiagnoses();
                }
            }
        }

        private void BtnDeleteDiagnosis_Click(object sender, EventArgs e)
        {
            if (_diagnosesGrid.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите диагноз для удаления", "Предупреждение",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var row = _diagnosesGrid.SelectedRows[0];
            int diagnosisId = Convert.ToInt32(row.Cells["diagnosis_id"].Value);
            string diagnosisName = row.Cells["name"].Value.ToString();

            if (MessageBox.Show($"Вы действительно хотите удалить диагноз '{diagnosisName}'?",
                             "Подтверждение",
                             MessageBoxButtons.YesNo,
                             MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (_dbManager.DeleteVisit(diagnosisId))
                {
                    MessageBox.Show("Диагноз успешно удален", "Успех",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadDiagnoses();
                }
                else
                {
                    MessageBox.Show("Ошибка при удалении диагноза", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void InitializeComponents()
        {
            tabControl1.SelectedIndexChanged += (s, args) =>
            {
                this._currentTab = tabControl1.SelectedIndex;
            };

            // Инициализация вкладок
            InitializeDoctorsTab(tabControl1.TabPages[0]);
            InitializePatientsTab(tabControl1.TabPages[1]);
            InitializeDiagnosesTab(tabControl1.TabPages[2]);
        }

        private void LoadDoctors()
        {
            _originalDoctorsData = _dbManager.GetAllDoctors();
            _doctorsGrid.DataSource = _originalDoctorsData;
        }

        private void LoadPatients()
        {
            _originalPatientsData = _dbManager.GetAllPatients();
            _patientsGrid.DataSource = _originalPatientsData;

            // Настраиваем отображение столбцов
            if (_patientsGrid.Columns.Count > 0)
            {
                _patientsGrid.Columns["patient_id"].HeaderText = "ID";
                _patientsGrid.Columns["first_name"].HeaderText = "Имя";
                _patientsGrid.Columns["last_name"].HeaderText = "Фамилия";
                _patientsGrid.Columns["patronymic"].HeaderText = "Отчество";
                _patientsGrid.Columns["birth_date"].HeaderText = "Дата рождения";
                _patientsGrid.Columns["address"].HeaderText = "Адресс";
                _patientsGrid.Columns["phone_number"].HeaderText = "Телефон";
                _patientsGrid.Columns["email"].HeaderText = "Email";
                _patientsGrid.Columns["gender"].HeaderText = "Пол";

                // Скрываем столбец с фото, если он есть
                if (_patientsGrid.Columns.Contains("photo"))
                    _patientsGrid.Columns["photo"].Visible = false;
            }
        }

        private void SearchBox_TextChanged(object sender, EventArgs e, bool isDoctorsTab)
        {
            string searchText = ((TextBox)sender).Text.ToLower();

            if (isDoctorsTab)
            {
                if (string.IsNullOrWhiteSpace(searchText))
                {
                    _doctorsGrid.DataSource = _originalDoctorsData;
                    return;
                }

                DataTable filteredTable = _originalDoctorsData.Clone();
                foreach (DataRow row in _originalDoctorsData.Rows)
                {
                    if (RowContainsText(row, searchText))
                        filteredTable.ImportRow(row);
                }
                _doctorsGrid.DataSource = filteredTable;
            }
            else
            {
                if (string.IsNullOrWhiteSpace(searchText))
                {
                    _patientsGrid.DataSource = _originalPatientsData;
                    return;
                }

                DataTable filteredTable = _originalPatientsData.Clone();
                foreach (DataRow row in _originalPatientsData.Rows)
                {
                    if (RowContainsText(row, searchText))
                        filteredTable.ImportRow(row);
                }
                _patientsGrid.DataSource = filteredTable;
            }
        }

        private bool RowContainsText(DataRow row, string searchText)
        {
            return row.ItemArray.Any(field =>
                field != null && field.ToString().ToLower().Contains(searchText));
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
                    MessageBox.Show("Ошибка при удалении врача", "Ош��бка", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                MessageBox.Show("Выберите пациента для удаления", "Предупреждение",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var row = _patientsGrid.SelectedRows[0];
            int patientId = Convert.ToInt32(row.Cells["patient_id"].Value);
            string patientName = $"{row.Cells["last_name"].Value} {row.Cells["first_name"].Value}";

            if (MessageBox.Show($"Вы действительно хотите удалить пациента {patientName}?",
                             "Подтверждение",
                             MessageBoxButtons.YesNo,
                             MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (_dbManager.DeletePatient(patientId))
                {
                    MessageBox.Show("Пациент успешно удален", "Успех",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadPatients();
                }
                else
                {
                    MessageBox.Show("Ошибка при удалении пациента", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnEditPatient_Click(object sender, EventArgs e)
        {
            if (_patientsGrid.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите пациента для редактирования", "Предупреждение",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var row = _patientsGrid.SelectedRows[0];
            int patientId = Convert.ToInt32(row.Cells["patient_id"].Value);

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
                MessageBox.Show("Выберите пациента для просмотра приемов", "Предупреждение",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var row = _patientsGrid.SelectedRows[0];
            int patientId = Convert.ToInt32(row.Cells["patient_id"].Value);
            string patientName = $"{row.Cells["last_name"].Value} {row.Cells["first_name"].Value}";

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

        private void BtnManageAccount_Click(object sender, EventArgs e)
        {
            if (_doctorsGrid.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите врача для управления аккаунтом", "Предупреждение",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var row = _doctorsGrid.SelectedRows[0];
            int doctorId = Convert.ToInt32(row.Cells["ID"].Value);
            string doctorName = $"{row.Cells["Фамилия"].Value} {row.Cells["Имя"].Value}";

            using (var accountForm = new UserAccountForm(doctorId, "doctor", doctorName))
            {
                accountForm.ShowDialog();
            }
        }

        private void BtnManagePatientAccount_Click(object sender, EventArgs e)
        {
            if (_patientsGrid.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите пациента для управления аккаунтом", "Предупреждени��",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var row = _patientsGrid.SelectedRows[0];
            int patientId = Convert.ToInt32(row.Cells["patient_id"].Value);
            string patientName = $"{row.Cells["last_name"].Value} {row.Cells["first_name"].Value}";

            using (var accountForm = new UserAccountForm(patientId, "patient", patientName))
            {
                accountForm.ShowDialog();
            }
        }
    }
}