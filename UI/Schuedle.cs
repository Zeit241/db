using System;
using System.Data;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Schedule : Form
    {
        private readonly DatabaseManager _dbManager;
        private readonly int _doctorId;
        private readonly string _doctorName;

        public Schedule(int doctorId, string doctorName)
        {
            InitializeComponent();
            _dbManager = new DatabaseManager();
            _doctorId = doctorId;
            _doctorName = doctorName;

            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            InitializeScheduleForm();
            LoadSchedule();
        }

        private void InitializeScheduleForm()
        {
            this.Text = $"Расписание врача: {_doctorName}";

            // Настройка DateTimePicker для выбора даты
            datePicker.Format = DateTimePickerFormat.Short;
            datePicker.MinDate = DateTime.Today;
            datePicker.Value = DateTime.Today;

            // Настр��йка DateTimePicker для времени начала
            startTimePicker.Format = DateTimePickerFormat.Time;
            startTimePicker.ShowUpDown = true;

            // Настройка DateTimePicker для времени окончания
            endTimePicker.Format = DateTimePickerFormat.Time;
            endTimePicker.ShowUpDown = true;

            // Настройка DataGridView
            scheduleGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            scheduleGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            scheduleGridView.MultiSelect = false;
            scheduleGridView.ReadOnly = true;
            scheduleGridView.AllowUserToAddRows = false;
            scheduleGridView.AllowUserToDeleteRows = false;

            // Добавляем обработчики событий
            addButton.Click += AddButton_Click;
            deleteButton.Click += DeleteButton_Click;
        }

        private void LoadSchedule()
        {
            var scheduleData = _dbManager.GetDoctorSchedule(_doctorId);
            scheduleGridView.DataSource = scheduleData;
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            if (!ValidateInput())
                return;

            var workDate = datePicker.Value.Date;
            var startTime = startTimePicker.Value.TimeOfDay;
            var endTime = endTimePicker.Value.TimeOfDay;

            // Проверяем пересечение времени
            if (_dbManager.HasTimeOverlap(_doctorId, workDate, startTime, endTime))
            {
                MessageBox.Show(
                    "На выбранную дату и время уже назначен рабочий день.\n" +
                    "Пожалуйста, выберите другое время.",
                    "Пересечение времени",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            if (_dbManager.AddDoctorWorkDay(_doctorId, workDate, startTime, endTime))
            {
                MessageBox.Show(
                    "Рабочий день успешно добавлен",
                    "Успех",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                LoadSchedule();
            }
            else
            {
                MessageBox.Show(
                    "Ошибка при добавлении рабочего дня",
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if (scheduleGridView.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите рабочий день для удаления", 
                              "Предупреждение", 
                              MessageBoxButtons.OK, 
                              MessageBoxIcon.Warning);
                return;
            }

            var row = scheduleGridView.SelectedRows[0];
            int scheduleId = Convert.ToInt32(row.Cells["ID"].Value);
            string workDate = row.Cells["Дата"].Value.ToString();

            if (MessageBox.Show($"Вы действительно хотите удалить рабочий день {workDate}?",
                             "Подтверждение",
                             MessageBoxButtons.YesNo,
                             MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (_dbManager.DeleteWorkDay(scheduleId))
                {
                    MessageBox.Show("Рабочий день успешно удален", 
                                  "Успех", 
                                  MessageBoxButtons.OK, 
                                  MessageBoxIcon.Information);
                    LoadSchedule();
                }
                else
                {
                    MessageBox.Show("Ошибка при удалении рабочего дня", 
                                  "Ошибка", 
                                  MessageBoxButtons.OK, 
                                  MessageBoxIcon.Error);
                }
            }
        }

        private bool ValidateInput()
        {
            // Проверяем, что выбрана будущая дата
            if (datePicker.Value.Date < DateTime.Today)
            {
                MessageBox.Show(
                    "Нельзя создать рабочий день на прошедшую дату",
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return false;
            }

            // Проверяем время
            if (endTimePicker.Value.TimeOfDay <= startTimePicker.Value.TimeOfDay)
            {
                MessageBox.Show(
                    "Время окончания должно быть позже времени начала",
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return false;
            }

            // Проверяем минимальную длительность рабочего дня (например, 1 час)
            TimeSpan duration = endTimePicker.Value.TimeOfDay - startTimePicker.Value.TimeOfDay;
            if (duration.TotalHours < 1)
            {
                MessageBox.Show(
                    "Рабочий день должен быть не менее 1 часа",
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return false;
            }

            // Проверяем максимальную длительность рабочего дня (например, 12 часов)
            if (duration.TotalHours > 12)
            {
                MessageBox.Show(
                    "Рабочий день не может быть более 12 часов",
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return false;
            }

            return true;
        }
    }
}
