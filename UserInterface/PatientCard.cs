using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApp1;

namespace DatabaseCursovaya.UI
{
    public partial class PatientCard : Form
    {
        private readonly DatabaseManager _dbManager;
        private readonly int? _patientId;
        private readonly bool _isEditMode;
        private byte[] _photoBytes;

        public PatientCard(int? patientId = null)
        {
            InitializeComponent();
            _dbManager = new DatabaseManager();
            _patientId = patientId;
            _isEditMode = patientId.HasValue;

            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            InitializeForm();
            if (_isEditMode)
            {
                LoadPatientData();
            }
        }

        private void InitializeForm()
        {
            this.Text = _isEditMode ? "Редактирование пациента" : "Новый пациент";

            // Инициализация значений пола
            genderComboBox.Items.Clear();
            genderComboBox.Items.AddRange(new[] { "M", "F" });
            genderComboBox.SelectedIndex = 0;
        }

        private void LoadPatientData()
        {
            try
            {
                var patient = _dbManager.GetPatientById2(_patientId.Value);
                if (patient != null)
                {
                    lastNameTextBox.Text = patient.LastName;
                    firstNameTextBox.Text = patient.FirstName;
                    patronymicTextBox.Text = patient.Patronymic;
                    birthDatePicker.Value = patient.BirthDate;
                    genderComboBox.SelectedItem = patient.Gender;
                    cityTextBox.Text = patient.AddressCity;
                    streetTextBox.Text = patient.AddressStreet;
                    buildingTextBox.Text = patient.AddressBuilding;
                    phoneTextBox.Text = patient.PhoneNumber;
                    emailTextBox.Text = patient.Email;

                    if (patient.Photo != null)
                    {
                        _photoBytes = patient.Photo;
                        using (var ms = new MemoryStream(patient.Photo))
                        {
                            photo.Image = Image.FromStream(ms);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!ValidateInput()) return;

            try
            {
                var patient = new Patient
                {
                    PatientId = _patientId ?? 0,
                    LastName = lastNameTextBox.Text,
                    FirstName = firstNameTextBox.Text,
                    Patronymic = patronymicTextBox.Text,
                    BirthDate = birthDatePicker.Value,
                    Gender = genderComboBox.SelectedItem.ToString(),
                    AddressCity = cityTextBox.Text,
                    AddressStreet = streetTextBox.Text,
                    AddressBuilding = buildingTextBox.Text,
                    PhoneNumber = phoneTextBox.Text,
                    Email = emailTextBox.Text,
                    Photo = _photoBytes
                };

                bool success = _isEditMode ?
                    _dbManager.UpdatePatient(patient) :
                    _dbManager.CreatePatient(patient);

                if (success)
                {
                    MessageBox.Show(
                        _isEditMode ? "Пациент успешно обновлен" : "Пациент успешно добавлен",
                        "Успех",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                    DialogResult = DialogResult.OK;
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidateInput()
        {
            // Валидация ФИО
            if (string.IsNullOrWhiteSpace(lastNameTextBox.Text) || lastNameTextBox.Text.Length < 2)
            {
                MessageBox.Show("Фамилия должна содержать минимум 2 символа",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                lastNameTextBox.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(firstNameTextBox.Text) || firstNameTextBox.Text.Length < 2)
            {
                MessageBox.Show("Имя должно содержать минимум 2 символа",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                firstNameTextBox.Focus();
                return false;
            }

            if (!string.IsNullOrWhiteSpace(patronymicTextBox.Text) && patronymicTextBox.Text.Length < 2)
            {
                MessageBox.Show("Отчество должно содержать минимум 2 символа",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                patronymicTextBox.Focus();
                return false;
            }

            // Валидация возраста
            var birthDate = birthDatePicker.Value;
            var age = DateTime.Now.Year - birthDate.Year;
            if (birthDate > DateTime.Now)
            {
                MessageBox.Show("Дата рождения не может быть в будущем",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                birthDatePicker.Focus();
                return false;
            }

            if (age > 150)
            {
                MessageBox.Show("Некорректный возраст",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                birthDatePicker.Focus();
                return false;
            }

            // Валидация адреса
            if (string.IsNullOrWhiteSpace(cityTextBox.Text))
            {
                MessageBox.Show("Введите город",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cityTextBox.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(streetTextBox.Text))
            {
                MessageBox.Show("Введите улицу",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                streetTextBox.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(buildingTextBox.Text))
            {
                MessageBox.Show("Введите номер дома",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                buildingTextBox.Focus();
                return false;
            }

            // Валидация контактных данных
            if (!string.IsNullOrWhiteSpace(phoneTextBox.Text))
            {
                string phonePattern = @"^\+?[1-9]\d{10}$";
                if (!System.Text.RegularExpressions.Regex.IsMatch(phoneTextBox.Text, phonePattern))
                {
                    MessageBox.Show("Неверный формат номера телефона. Используйте формат +XXXXXXXXXXX",
                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    phoneTextBox.Focus();
                    return false;
                }
            }

            if (!string.IsNullOrWhiteSpace(emailTextBox.Text))
            {
                string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
                if (!System.Text.RegularExpressions.Regex.IsMatch(emailTextBox.Text, emailPattern))
                {
                    MessageBox.Show("Неверный формат email адреса",
                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    emailTextBox.Focus();
                    return false;
                }
            }

            // Проверка на обязательное наличие хотя бы одного контакта
            if (string.IsNullOrWhiteSpace(phoneTextBox.Text) && string.IsNullOrWhiteSpace(emailTextBox.Text))
            {
                MessageBox.Show("Необходимо указать хотя бы один способ связи (телефон или email)",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                phoneTextBox.Focus();
                return false;
            }

            return true;
        }

        private void LoadPhotoButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        _photoBytes = File.ReadAllBytes(openFileDialog.FileName);
                        photo.Image = Image.FromFile(openFileDialog.FileName);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при загрузке фото: {ex.Message}", "Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}
