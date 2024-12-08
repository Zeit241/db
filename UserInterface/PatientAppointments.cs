using DatabaseCursovaya.UserInterface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using WindowsFormsApp1;

namespace DatabaseCursovaya.UI
{
    public partial class PatientAppointments : Form
    {
        private readonly DatabaseManager _dbManager;
        private readonly int _patientId;
        private readonly string _patientName;
        private TabControl tabControl; // Добавляем поле

        public PatientAppointments(int patientId, string patientName)
        {
            InitializeComponent();
            _dbManager = new DatabaseManager();
            _patientId = patientId;
            _patientName = patientName;

            this.Text = $"Приемы пациента: {_patientName}";
            InitializeTabs();
        }

        private void InitializeTabs()
        {
            tabControl = new TabControl { Dock = DockStyle.Fill };

            var existingAppointmentsTab = new TabPage("Существующие записи");
            var newAppointmentTab = new TabPage("Создание новой записи");

            InitializeExistingAppointmentsTab(existingAppointmentsTab);
            InitializeNewAppointmentTab(newAppointmentTab);

            tabControl.TabPages.Add(existingAppointmentsTab);
            tabControl.TabPages.Add(newAppointmentTab);

            this.Controls.Add(tabControl);
        }

        private void InitializeExistingAppointmentsTab(TabPage tab)
        {
            // Создаем панель для кнопки
            var buttonPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 40,
                Padding = new Padding(5)
            };

            var deleteButton = new Button
            {
                Text = "Удалить запись",
                Width = 120,
                Height = 30,
                Location = new Point(5, 5)
            };
            deleteButton.Click += DeleteAppointment_Click;
            buttonPanel.Controls.Add(deleteButton);

            var appointmentsGrid = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                RowHeadersVisible = false,
                MultiSelect = false
            };

            appointmentsGrid.DataSource = _dbManager.GetPatientAppointments(_patientId);

            // Очищаем существующие контролы
            tab.Controls.Clear();
            tab.Controls.Add(appointmentsGrid);
            tab.Controls.Add(buttonPanel);
        }

        private void InitializeNewAppointmentTab(TabPage tab)
        {
            // Создаем панель для поиска
            var searchPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 50,
                Padding = new Padding(10)
            };

            var searchBox = new TextBox
            {
                Width = 200,
                Location = new Point(10, 15),

            };

            searchPanel.Controls.Add(searchBox);

            // Настраиваем ListView
            var doctorsListView = new ListView
            {
                View = View.LargeIcon,
                Dock = DockStyle.Fill,
                LargeImageList = new ImageList { ImageSize = new Size(64, 64) }
            };


            searchBox.TextChanged += (s, e) => FilterDoctors(doctorsListView, searchBox.Text);

            // Загрузка докторов в ListView
            LoadDoctors(doctorsListView);

            doctorsListView.ItemActivate += (s, e) =>
            {
                if (doctorsListView.SelectedItems.Count > 0)
                {
                    var selectedDoctor = (Doctor)doctorsListView.SelectedItems[0].Tag;
                    ShowDoctorSchedule(selectedDoctor);
                }
            };

            tab.Controls.Add(doctorsListView);
            tab.Controls.Add(searchPanel);
        }

        private void LoadDoctors(ListView listView)
        {
            var dt = _dbManager.GetAllDoctors();
            listView.Items.Clear();

            foreach (DataRow row in dt.Rows)
            {
                var doctor = new Doctor
                {
                    DoctorId = Convert.ToInt32(row["ID"]),
                    FirstName = row["Имя"].ToString(),
                    LastName = row["Фамилия"].ToString(),
                    Patronymic = row["Отчество"].ToString()
                };

                var item = new ListViewItem
                {
                    Text = doctor.FullName,
                    Tag = doctor,
                    ImageKey = "doctor",
                    ImageIndex = 0
                };

                listView.Items.Add(item);
            }
        }

        private void FilterDoctors(ListView listView, string searchText)
        {
            listView.Items.Clear();
            var dt = _dbManager.GetAllDoctors();
            var filteredRows = dt.AsEnumerable()
                .Where(row => row.Field<string>("Фамилия").ToLower().Contains(searchText.ToLower()) ||
                             row.Field<string>("Имя").ToLower().Contains(searchText.ToLower()));

            foreach (var row in filteredRows)
            {
                var doctor = new Doctor
                {
                    DoctorId = row.Field<int>("ID"),
                    FirstName = row.Field<string>("Имя"),
                    LastName = row.Field<string>("Фамилия"),
                    Patronymic = row.Field<string>("Отчество")
                };
                var item = new ListViewItem(doctor.FullName) { Tag = doctor };
                listView.Items.Add(item);
            }
        }

        private void ShowDoctorSchedule(Doctor doctor)
        {
            var scheduleForm = new DoctorScheduleForm(doctor, _patientId)
            {
                Owner = this  // Устанавливаем владельца формы
            };
            scheduleForm.ShowDialog();
        }

        public void RefreshAppointments()
        {
            InitializeExistingAppointmentsTab((TabPage)tabControl.TabPages[0]);
        }

        private void DeleteAppointment_Click(object sender, EventArgs e)
        {
            var grid = tabControl.TabPages[0].Controls.OfType<DataGridView>().FirstOrDefault();
            if (grid?.CurrentRow == null) return;

            if (MessageBox.Show(
                "Вы действительно хотите удалить эту запись?",
                "Подтверждение удаления",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) == DialogResult.Yes)
            {
                var date = Convert.ToDateTime(grid.CurrentRow.Cells["Дата"].Value);
                var time = TimeSpan.Parse(grid.CurrentRow.Cells["Время"].Value.ToString());
                var doctorName = grid.CurrentRow.Cells["Врач"].Value.ToString();

                if (_dbManager.DeleteAppointment(_patientId, date, time))
                {
                    MessageBox.Show("Запись успешно удалена", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    RefreshAppointments();
                }
                else
                {
                    MessageBox.Show("Не удалось удалить запись", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
