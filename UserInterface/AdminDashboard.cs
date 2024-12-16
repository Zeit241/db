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
            deleteButton.Click += BtnDeleteDoctor_Click;
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

            Button editButton = new Button
            {
                Text = "Изменить",
                Width = 100,
                Height = 30,
                Location = new Point(5, 5)
            };

            Button deleteButton = new Button
            {
                Text = "Удалить",
                Width = 100,
                Height = 30,
                Location = new Point(110, 5)
            };

            // Привязываем обработчики
            editButton.Click += BtnDiagnosisDetails_Click;
            deleteButton.Click += BtnDeleteDiagnosis_Click;

            // Добавляем кнопки на панель
            diagnosesButtonPanel.Controls.AddRange(new Control[] {
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

        public void LoadDiagnoses()
        {
            _diagnosesGrid.DataSource = _dbManager.GetAllVisits();

            if (_diagnosesGrid.Columns.Count > 0)
            {
                // Настраиваем отображение столбцов
                _diagnosesGrid.Columns["visit_id"].HeaderText = "ID";
                _diagnosesGrid.Columns["Пациент"].HeaderText = "Пациент";
                _diagnosesGrid.Columns["Врач"].HeaderText = "Врач";
                _diagnosesGrid.Columns["Дата приема"].HeaderText = "Дата приема";
                _diagnosesGrid.Columns["Время приема"].HeaderText = "Время приема";
                _diagnosesGrid.Columns["Диагноз"].HeaderText = "Диагноз";
                _diagnosesGrid.Columns["Статус"].HeaderText = "Статус";
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
                detailsForm.VisitUpdated += (s, args) => LoadDiagnoses();
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
                MessageBox.Show("Выберите визит для удаления", "Предупреждение",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var row = _diagnosesGrid.SelectedRows[0];
            string status = row.Cells["Статус"].Value.ToString();

            if (status != "Ожидается")
            {
                MessageBox.Show("Можно удалять только ожидающиеся приемы", "Предупреждение",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("Вы действительно хотите удалить этот прием?",
                             "Подтверждение",
                             MessageBoxButtons.YesNo,
                             MessageBoxIcon.Question) == DialogResult.Yes)
            {
                int visitId = Convert.ToInt32(row.Cells["visit_id"].Value);
                if (_dbManager.DeleteVisit(visitId))
                {
                    MessageBox.Show("Прием успешно удален", "Успех",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadDiagnoses();
                }
                else
                {
                    MessageBox.Show("Ошибка при удалении приема", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void InitializeComponents()
        {

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

        private void SearchBox_TextChanged(object sender, EventArgs e, bool isDoctors = false)
        {
            var grid = isDoctors ? _doctorsGrid : _patientsGrid;
            var searchBox = isDoctors ? _doctorsSearchBox : _patientsSearchBox;
            string searchText = searchBox.Text.ToLower();

            foreach (DataGridViewRow row in grid.Rows)
            {
                bool visible = false;
                foreach (DataGridViewCell cell in row.Cells)
                {
                    if (cell.Value != null &&
                        (cell.Value.ToString().ToLower().Contains(searchText) ||
                        (isDoctors && row.Cells["Специальности"].Value?.ToString().ToLower().Contains(searchText) == true)))
                    {
                        visible = true;
                        break;
                    }
                }
                row.Visible = visible;
            }
        }

        private bool RowContainsText(DataRow row, string searchText)
        {
            return row.ItemArray.Any(field =>
                field != null && field.ToString().ToLower().Contains(searchText));
        }

        private void BtnDeleteDoctor_Click(object sender, EventArgs e)
        {
            if (_doctorsGrid.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите врача для удаления",
                    "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var row = _doctorsGrid.SelectedRows[0];
            int doctorId = Convert.ToInt32(row.Cells["ID"].Value);
            string doctorName = $"{row.Cells["Фамилия"].Value} {row.Cells["Имя"].Value}";

            // Проверяем наличие расписания и приемов
            bool hasSchedule = _dbManager.HasDoctorSchedule(doctorId);
            bool hasVisits = _dbManager.HasDoctorVisits(doctorId);

            if (hasSchedule || hasVisits)
            {
                string message = "Внимание!\n\n";
                if (hasSchedule)
                    message += "- У врача есть расписание на будущие даты\n";
                if (hasVisits)
                    message += "- У врача есть записанные пациенты\n\n";
                message += "При удалении врача:\n";
                message += "- Будет удалено всё его расписание\n";
                message += "- Информация о прошедших приемах сохранится\n\n";
                message += "Вы действительно хотите удалить врача?";

                var result = MessageBox.Show(
                    message,
                    "Подтверждение удаления",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                if (result == DialogResult.No)
                    return;
            }
            else
            {
                var result = MessageBox.Show(
                    $"Вы действительно хотите удалить врача {doctorName}?",
                    "Подтверждение удаления",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (result == DialogResult.No)
                    return;
            }

            try
            {
                bool success = _dbManager.DeleteDoctor(doctorId);
                if (success)
                {
                    MessageBox.Show(
                        "Врач успешно удален",
                        "Успех",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                    LoadDoctors(); // Обновляем таблицу
                }
                else
                {
                    MessageBox.Show(
                        "Не удалось удалить врача",
                        "Ошибка",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Ошибка при удалении врача: {ex.Message}",
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
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
                MessageBox.Show("Выберите пациента для управления аккаунтом", "Предупреждение",
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