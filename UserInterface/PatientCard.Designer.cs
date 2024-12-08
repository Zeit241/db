namespace DatabaseCursovaya.UI
{
    partial class PatientCard
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>

        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.firstNameTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.lastNameTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.patronymicTextBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.birthDatePicker = new System.Windows.Forms.DateTimePicker();
            this.label5 = new System.Windows.Forms.Label();
            this.genderComboBox = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.cityTextBox = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.streetTextBox = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.buildingTextBox = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.phoneTextBox = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.emailTextBox = new System.Windows.Forms.TextBox();
            this.photo = new System.Windows.Forms.PictureBox();
            this.loadPhotoButton = new System.Windows.Forms.Button();
            this.saveButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.photo)).BeginInit();
            this.SuspendLayout();

            // Настройка формы
            this.ClientSize = new System.Drawing.Size(484, 561);
            this.Name = "PatientCard";
            this.Text = "Карточка пациента";

            // label1
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Size = new System.Drawing.Size(32, 13);
            this.label1.Text = "Имя";

            // firstNameTextBox
            this.firstNameTextBox.Location = new System.Drawing.Point(12, 31);
            this.firstNameTextBox.Size = new System.Drawing.Size(200, 20);

            // label2
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 61);
            this.label2.Size = new System.Drawing.Size(56, 13);
            this.label2.Text = "Фамилия";

            // lastNameTextBox
            this.lastNameTextBox.Location = new System.Drawing.Point(12, 77);
            this.lastNameTextBox.Size = new System.Drawing.Size(200, 20);

            // label3
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 107);
            this.label3.Size = new System.Drawing.Size(54, 13);
            this.label3.Text = "Отчество";

            // patronymicTextBox
            this.patronymicTextBox.Location = new System.Drawing.Point(12, 123);
            this.patronymicTextBox.Size = new System.Drawing.Size(200, 20);

            // label4
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 153);
            this.label4.Size = new System.Drawing.Size(86, 13);
            this.label4.Text = "Дата рождения";

            // birthDatePicker
            this.birthDatePicker.Location = new System.Drawing.Point(12, 169);
            this.birthDatePicker.Size = new System.Drawing.Size(200, 20);
            this.birthDatePicker.Format = System.Windows.Forms.DateTimePickerFormat.Short;

            // label5
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 199);
            this.label5.Size = new System.Drawing.Size(27, 13);
            this.label5.Text = "Пол";

            // genderComboBox
            this.genderComboBox.Location = new System.Drawing.Point(12, 215);
            this.genderComboBox.Size = new System.Drawing.Size(200, 21);
            this.genderComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.genderComboBox.Items.AddRange(new object[] { "Мужской", "Женский" });

            // label6
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 245);
            this.label6.Size = new System.Drawing.Size(37, 13);
            this.label6.Text = "Город";

            // cityTextBox
            this.cityTextBox.Location = new System.Drawing.Point(12, 261);
            this.cityTextBox.Size = new System.Drawing.Size(200, 20);

            // label7
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 291);
            this.label7.Size = new System.Drawing.Size(39, 13);
            this.label7.Text = "Улица";

            // streetTextBox
            this.streetTextBox.Location = new System.Drawing.Point(12, 307);
            this.streetTextBox.Size = new System.Drawing.Size(200, 20);

            // label8
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(12, 337);
            this.label8.Size = new System.Drawing.Size(30, 13);
            this.label8.Text = "Дом";

            // buildingTextBox
            this.buildingTextBox.Location = new System.Drawing.Point(12, 353);
            this.buildingTextBox.Size = new System.Drawing.Size(200, 20);

            // label9
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(12, 383);
            this.label9.Size = new System.Drawing.Size(52, 13);
            this.label9.Text = "Телефон";

            // phoneTextBox
            this.phoneTextBox.Location = new System.Drawing.Point(12, 399);
            this.phoneTextBox.Size = new System.Drawing.Size(200, 20);

            // label10
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(12, 429);
            this.label10.Size = new System.Drawing.Size(32, 13);
            this.label10.Text = "Email";

            // emailTextBox
            this.emailTextBox.Location = new System.Drawing.Point(12, 445);
            this.emailTextBox.Size = new System.Drawing.Size(200, 20);

            // photo
            this.photo.Location = new System.Drawing.Point(232, 31);
            this.photo.Size = new System.Drawing.Size(240, 240);
            this.photo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.photo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

            // loadPhotoButton
            this.loadPhotoButton.Location = new System.Drawing.Point(232, 277);
            this.loadPhotoButton.Size = new System.Drawing.Size(240, 23);
            this.loadPhotoButton.Text = "Загрузить фото";
            this.loadPhotoButton.Click += new System.EventHandler(this.LoadPhotoButton_Click);

            // saveButton
            this.saveButton.Location = new System.Drawing.Point(286, 491);
            this.saveButton.Size = new System.Drawing.Size(90, 23);
            this.saveButton.Text = "Сохранить";
            this.saveButton.Click += new System.EventHandler(this.SaveButton_Click);

            // cancelButton
            this.cancelButton.Location = new System.Drawing.Point(382, 491);
            this.cancelButton.Size = new System.Drawing.Size(90, 23);
            this.cancelButton.Text = "Отмена";
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;

            // Добавляем все контролы на форму
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.label1,
                this.firstNameTextBox,
                this.label2,
                this.lastNameTextBox,
                this.label3,
                this.patronymicTextBox,
                this.label4,
                this.birthDatePicker,
                this.label5,
                this.genderComboBox,
                this.label6,
                this.cityTextBox,
                this.label7,
                this.streetTextBox,
                this.label8,
                this.buildingTextBox,
                this.label9,
                this.phoneTextBox,
                this.label10,
                this.emailTextBox,
                this.photo,
                this.loadPhotoButton,
                this.saveButton,
                this.cancelButton
            });

            ((System.ComponentModel.ISupportInitialize)(this.photo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox firstNameTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox lastNameTextBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox patronymicTextBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.DateTimePicker birthDatePicker;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox genderComboBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox cityTextBox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox streetTextBox;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox buildingTextBox;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox phoneTextBox;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox emailTextBox;
        private System.Windows.Forms.PictureBox photo;
        private System.Windows.Forms.Button loadPhotoButton;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Button cancelButton;
    }

}