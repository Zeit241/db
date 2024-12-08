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
    public partial class DoctorCard : Form
    {
        private readonly DatabaseManager _dbManager;
        private readonly int? _doctorId;
        private readonly bool _isEditMode;
        private byte[] _currentPhoto;
        public DoctorCard(int? doctorId = null)
        {
            InitializeComponent();
            _dbManager = new DatabaseManager();
            _doctorId = doctorId;
            _isEditMode = doctorId.HasValue;

            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            InitializeForm();
            SetupPhotoHandling();
        }


        private void InitializeForm()
        {
            // Заполняем комбобокс специальностей
            var specialties = _dbManager.GetAllSpecialties();
            specialtyComboBox.DataSource = specialties;
            specialtyComboBox.DisplayMember = "name";
            specialtyComboBox.ValueMember = "specialty_id";

            // Заполняем комбобокс пола
            genderComboBox.Items.Add("Мужской");
            genderComboBox.Items.Add("Женский");

            if (_isEditMode)
            {
                this.Text = "Редактирование врача";
                LoadDoctorData();
            }
            else
            {
                this.Text = "Новый врач";
                genderComboBox.SelectedIndex = 0;
                dateOfBirthPicker.Value = DateTime.Now.AddYears(-30);
            }
        }
        private void LoadDoctorData()
        {
            var doctorData = _dbManager.GetDoctorById(_doctorId.Value);
            if (doctorData != null)
            {
                lastNameTextBox.Text = doctorData["last_name"].ToString();
                firstNameTextBox.Text = doctorData["first_name"].ToString();
                patronymicTextBox.Text = doctorData["patronymic"].ToString();
                specialtyComboBox.SelectedValue = doctorData["specialty_id"];
                experienceNumeric.Value = Convert.ToInt32(doctorData["experience_years"]);
                phoneTextBox.Text = doctorData["phone_number"].ToString();
                emailTextBox.Text = doctorData["email"].ToString();
                cityTextBox.Text = doctorData["address_city"].ToString();
                streetTextBox.Text = doctorData["address_street"].ToString();
                buildingTextBox.Text = doctorData["address_building"].ToString();
                dateOfBirthPicker.Value = Convert.ToDateTime(doctorData["date_of_birth"]);
                genderComboBox.SelectedIndex = doctorData["gender"].ToString() == "M" ? 0 : 1;

                Console.WriteLine("Загрузка данных врача");
                if (doctorData["photo"] != DBNull.Value)
                {
                    _currentPhoto = (byte[])doctorData["photo"];
                    Console.WriteLine($"Загружено фото из БД, размер: {_currentPhoto.Length} байт");
                    using (var ms = new MemoryStream(_currentPhoto))
                    {
                        if (photo.Image != null)
                        {
                            photo.Image.Dispose();
                        }
                        photo.Image = Image.FromStream(ms);
                        Console.WriteLine("Фото установлено в PictureBox");
                    }
                }
                else
                {
                    Console.WriteLine("Фото в БД отсутствует");
                    _currentPhoto = null;
                    photo.Image = null;
                }
            }
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            if (!ValidateInput())
                return;

            try
            {
                Console.WriteLine($"Сохранение формы. Режим редактирования: {_isEditMode}");
                if (_currentPhoto != null)
                {
                    Console.WriteLine($"Есть фото для сохранения, размер: {_currentPhoto.Length} байт");
                }
                else
                {
                    Console.WriteLine("Фото для сохранения отсутствует");
                }

                bool success;
                if (_isEditMode)
                {
                    success = _dbManager.UpdateDoctor(
                        _doctorId.Value,
                        firstNameTextBox.Text.Trim(),
                        lastNameTextBox.Text.Trim(),
                        patronymicTextBox.Text.Trim(),
                        (int)specialtyComboBox.SelectedValue,
                        (int)experienceNumeric.Value,
                        phoneTextBox.Text.Trim(),
                        emailTextBox.Text.Trim(),
                        cityTextBox.Text.Trim(),
                        streetTextBox.Text.Trim(),
                        buildingTextBox.Text.Trim(),
                        dateOfBirthPicker.Value,
                        genderComboBox.SelectedIndex == 0 ? 'M' : 'F',
                        _currentPhoto
                    );
                }
                else
                {
                    success = _dbManager.CreateDoctor(
                        firstNameTextBox.Text.Trim(),
                        lastNameTextBox.Text.Trim(),
                        patronymicTextBox.Text.Trim(),
                        (int)specialtyComboBox.SelectedValue,
                        (int)experienceNumeric.Value,
                        phoneTextBox.Text.Trim(),
                        emailTextBox.Text.Trim(),
                        cityTextBox.Text.Trim(),
                        streetTextBox.Text.Trim(),
                        buildingTextBox.Text.Trim(),
                        dateOfBirthPicker.Value,
                        genderComboBox.SelectedIndex == 0 ? 'M' : 'F',
                        _currentPhoto
                    );
                }

                Console.WriteLine($"Результат сохранения: {success}");

                if (success)
                {
                    MessageBox.Show(
                        _isEditMode ? "Врач успешно обновлен" : "Врач успешно добавлен",
                        "Успех",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show(
                        "Произошла ошибка при сохранении данных",
                        "Ошибка",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при сохранении: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                MessageBox.Show(
                    $"Произошла ошибка: {ex.Message}",
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(lastNameTextBox.Text))
            {
                MessageBox.Show("Введите фамилию", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(firstNameTextBox.Text))
            {
                MessageBox.Show("Введите имя", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (experienceNumeric.Value < 0)
            {
                MessageBox.Show("Стаж не может быть отрицательным", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (dateOfBirthPicker.Value > DateTime.Now.AddYears(-18))
            {
                MessageBox.Show("Врач должен быть старше 18 лет", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (specialtyComboBox.SelectedValue == null)
            {
                MessageBox.Show("Выберите специальность", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void SetupPhotoHandling()
        { 
            // Настройка PictureBox
            photo.SizeMode = PictureBoxSizeMode.StretchImage;
            photo.BorderStyle = BorderStyle.FixedSingle;

            // Настройка кнопки загрузки
            loadPhotoButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            loadPhotoButton.Location = new Point(photo.Left, photo.Bottom + 10);

            // Добавляем контекстное меню для PictureBox
            ContextMenuStrip photoMenu = new ContextMenuStrip();
            photoMenu.Items.Add("Загрузить фото", null, (s, e) => LoadPhotoButton_Click(s, e));
            photoMenu.Items.Add("Удалить фото", null, (s, e) => ClearPhoto());
            photo.ContextMenuStrip = photoMenu;
        }

        private void ClearPhoto()
        {
            photo.Image = null;
            _currentPhoto = null;
        }

        private void LoadPhotoButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Изображения|*.jpg;*.jpeg;*.png;*.gif;*.bmp";
                openFileDialog.Title = "Выберите фотографию врача";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        Console.WriteLine($"Выбран файл: {openFileDialog.FileName}");
                        using (var originalImage = Image.FromFile(openFileDialog.FileName))
                        {
                            int size = 200;
                            using (var resizedImage = new Bitmap(size, size))
                            {
                                using (var graphics = Graphics.FromImage(resizedImage))
                                {
                                    graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                                    graphics.DrawImage(originalImage, 0, 0, size, size);
                                }

                                using (var ms = new MemoryStream())
                                {
                                    resizedImage.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                                    _currentPhoto = ms.ToArray();
                                    Console.WriteLine($"Фото преобразовано, размер: {_currentPhoto.Length} байт");
                                }

                                if (photo.Image != null)
                                {
                                    photo.Image.Dispose();
                                }
                                photo.Image = new Bitmap(resizedImage);
                                Console.WriteLine("Фото установлено в PictureBox");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Ошибка при загрузке фото: {ex.Message}");
                        MessageBox.Show($"Ошибка при загрузке изображения: {ex.Message}", 
                                      "Ошибка", 
                                      MessageBoxButtons.OK, 
                                      MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}
