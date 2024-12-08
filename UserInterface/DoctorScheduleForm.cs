using DatabaseCursovaya.UI;
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
    public partial class DoctorScheduleForm : Form
    {
        private readonly DatabaseManager _dbManager;
        private readonly Doctor _doctor;
        private readonly int _patientId;
        private MonthCalendar _calendar;
        private FlowLayoutPanel _timeSlotPanel;

        public DoctorScheduleForm(Doctor doctor, int patientId)
        {
            InitializeComponent();
            _dbManager = new DatabaseManager();
            _doctor = doctor;
            _patientId = patientId;
            Console.WriteLine($"DoctorScheduleForm создана для пациента {patientId} и врача {doctor.DoctorId}");

            this.Text = $"Расписание врача: {doctor.FullName}";
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            // Создаем ��алендарь
            _calendar = new MonthCalendar
            {
                Location = new Point(20, 20),
                MaxSelectionCount = 1,
                MinDate = DateTime.Today
            };
            _calendar.DateSelected += Calendar_DateSelected;

            // Создаем панель для временных слотов
            _timeSlotPanel = new FlowLayoutPanel
            {
                Location = new Point(300, 20),
                Size = new Size(460, 520),
                AutoScroll = true,
                BorderStyle = BorderStyle.FixedSingle
            };

            this.Controls.Add(_calendar);
            this.Controls.Add(_timeSlotPanel);

            LoadTimeSlots(DateTime.Today);
        }

        private void Calendar_DateSelected(object sender, DateRangeEventArgs e)
        {
            LoadTimeSlots(e.Start);
        }

        private void LoadTimeSlots(DateTime date)
        {
            _timeSlotPanel.Controls.Clear();

            var availableSlots = _dbManager.GetAvailableTimeSlots(_doctor.DoctorId, date);

            if (availableSlots.Count == 0)
            {
                var label = new Label
                {
                    Text = "Нет доступного времени на выбранную дату",
                    AutoSize = true,
                    Location = new Point(10, 10)
                };
                _timeSlotPanel.Controls.Add(label);
                return;
            }

            foreach (var slot in availableSlots)
            {
                var button = new Button
                {
                    Text = $"{slot.Hours:D2}:{slot.Minutes:D2}",
                    Size = new Size(100, 40),
                    Margin = new Padding(5),
                    BackColor = Color.LightGreen
                };

                button.Click += (s, e) => TimeSlot_Click(date, slot);
                _timeSlotPanel.Controls.Add(button);
            }
        }

        private void TimeSlot_Click(DateTime date, TimeSpan time)
        {
            var result = MessageBox.Show(
                $"Записаться на прием {date.ToShortDateString()} в {time.Hours:D2}:{time.Minutes:D2}?",
                "Подтверждение записи",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                CreateAppointment(date, time);
            }
        }

        private void CreateAppointment(DateTime date, TimeSpan time)
        {
            if (_dbManager.CreateAppointment(_doctor.DoctorId, _patientId, date, time))
            {
                MessageBox.Show(
                    "Запись на прием успешно создана",
                    "Успех",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                LoadTimeSlots(date);

                // Обновляем родительскую форму
                if (Owner is PatientAppointments parentForm)
                {
                    parentForm.RefreshAppointments();
                }
            }
            else
            {
                MessageBox.Show(
                    "Ошибка при создании записи",
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }
    }
}
