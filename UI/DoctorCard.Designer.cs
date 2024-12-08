namespace DatabaseCursovaya.UI
{
    partial class DoctorCard
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.firstNameTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.lastNameTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.patronymicTextBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.specialtyComboBox = new System.Windows.Forms.ComboBox();
            this.experienceNumeric = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.photo = new System.Windows.Forms.PictureBox();
            this.loadPhotoButton = new System.Windows.Forms.Button();
            this.dateOfBirthPicker = new System.Windows.Forms.DateTimePicker();
            this.label6 = new System.Windows.Forms.Label();
            this.emailTextBox = new System.Windows.Forms.TextBox();
            this.cityTextBox = new System.Windows.Forms.TextBox();
            this.streetTextBox = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.phoneTextBox = new System.Windows.Forms.MaskedTextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.buildingTextBox = new System.Windows.Forms.TextBox();
            this.genderComboBox = new System.Windows.Forms.ComboBox();
            this.label12 = new System.Windows.Forms.Label();
            this.saveButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.experienceNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.photo)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Имя";
            // 
            // firstNameTextBox
            // 
            this.firstNameTextBox.Location = new System.Drawing.Point(15, 25);
            this.firstNameTextBox.Name = "firstNameTextBox";
            this.firstNameTextBox.Size = new System.Drawing.Size(144, 20);
            this.firstNameTextBox.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 52);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Фамилия";
            // 
            // lastNameTextBox
            // 
            this.lastNameTextBox.Location = new System.Drawing.Point(18, 68);
            this.lastNameTextBox.Name = "lastNameTextBox";
            this.lastNameTextBox.Size = new System.Drawing.Size(141, 20);
            this.lastNameTextBox.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 100);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(54, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Отчество";
            // 
            // patronymicTextBox
            // 
            this.patronymicTextBox.Location = new System.Drawing.Point(18, 116);
            this.patronymicTextBox.Name = "patronymicTextBox";
            this.patronymicTextBox.Size = new System.Drawing.Size(141, 20);
            this.patronymicTextBox.TabIndex = 5;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(18, 197);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(85, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Специальность";
            // 
            // specialtyComboBox
            // 
            this.specialtyComboBox.FormattingEnabled = true;
            this.specialtyComboBox.Location = new System.Drawing.Point(15, 213);
            this.specialtyComboBox.Name = "specialtyComboBox";
            this.specialtyComboBox.Size = new System.Drawing.Size(141, 21);
            this.specialtyComboBox.TabIndex = 7;
            // 
            // experienceNumeric
            // 
            this.experienceNumeric.Location = new System.Drawing.Point(15, 257);
            this.experienceNumeric.Name = "experienceNumeric";
            this.experienceNumeric.Size = new System.Drawing.Size(141, 20);
            this.experienceNumeric.TabIndex = 8;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(15, 241);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(33, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "Стаж";
            // 
            // photo
            // 
            this.photo.Location = new System.Drawing.Point(178, 25);
            this.photo.Name = "photo";
            this.photo.Size = new System.Drawing.Size(147, 141);
            this.photo.TabIndex = 10;
            this.photo.TabStop = false;
            // 
            // loadPhotoButton
            // 
            this.loadPhotoButton.Location = new System.Drawing.Point(195, 172);
            this.loadPhotoButton.Name = "loadPhotoButton";
            this.loadPhotoButton.Size = new System.Drawing.Size(111, 23);
            this.loadPhotoButton.TabIndex = 11;
            this.loadPhotoButton.Text = "Выбрать фото";
            this.loadPhotoButton.UseVisualStyleBackColor = true;
            this.loadPhotoButton.Click += new System.EventHandler(this.LoadPhotoButton_Click);
            // 
            // dateOfBirthPicker
            // 
            this.dateOfBirthPicker.Location = new System.Drawing.Point(15, 302);
            this.dateOfBirthPicker.Name = "dateOfBirthPicker";
            this.dateOfBirthPicker.Size = new System.Drawing.Size(144, 20);
            this.dateOfBirthPicker.TabIndex = 12;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(15, 284);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(86, 13);
            this.label6.TabIndex = 13;
            this.label6.Text = "Дата рождения";
            // 
            // emailTextBox
            // 
            this.emailTextBox.Location = new System.Drawing.Point(15, 382);
            this.emailTextBox.Name = "emailTextBox";
            this.emailTextBox.Size = new System.Drawing.Size(141, 20);
            this.emailTextBox.TabIndex = 15;
            // 
            // cityTextBox
            // 
            this.cityTextBox.Location = new System.Drawing.Point(174, 226);
            this.cityTextBox.Name = "cityTextBox";
            this.cityTextBox.Size = new System.Drawing.Size(141, 20);
            this.cityTextBox.TabIndex = 16;
            // 
            // streetTextBox
            // 
            this.streetTextBox.Location = new System.Drawing.Point(171, 270);
            this.streetTextBox.Name = "streetTextBox";
            this.streetTextBox.Size = new System.Drawing.Size(141, 20);
            this.streetTextBox.TabIndex = 17;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(15, 327);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(93, 13);
            this.label7.TabIndex = 18;
            this.label7.Text = "Номер телефона";
            // 
            // phoneTextBox
            // 
            this.phoneTextBox.Location = new System.Drawing.Point(15, 344);
            this.phoneTextBox.Name = "phoneTextBox";
            this.phoneTextBox.Size = new System.Drawing.Size(141, 20);
            this.phoneTextBox.TabIndex = 19;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(15, 367);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(32, 13);
            this.label8.TabIndex = 20;
            this.label8.Text = "Email";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(175, 213);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(37, 13);
            this.label9.TabIndex = 21;
            this.label9.Text = "Город";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(171, 256);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(39, 13);
            this.label10.TabIndex = 22;
            this.label10.Text = "Улица";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(171, 299);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(44, 13);
            this.label11.TabIndex = 24;
            this.label11.Text = "Здание";
            // 
            // buildingTextBox
            // 
            this.buildingTextBox.Location = new System.Drawing.Point(171, 313);
            this.buildingTextBox.Name = "buildingTextBox";
            this.buildingTextBox.Size = new System.Drawing.Size(141, 20);
            this.buildingTextBox.TabIndex = 23;
            // 
            // genderComboBox
            // 
            this.genderComboBox.FormattingEnabled = true;
            this.genderComboBox.Location = new System.Drawing.Point(18, 159);
            this.genderComboBox.Name = "genderComboBox";
            this.genderComboBox.Size = new System.Drawing.Size(138, 21);
            this.genderComboBox.TabIndex = 25;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(18, 143);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(27, 13);
            this.label12.TabIndex = 26;
            this.label12.Text = "Пол";
            // 
            // saveButton
            // 
            this.saveButton.Location = new System.Drawing.Point(82, 426);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(166, 37);
            this.saveButton.TabIndex = 27;
            this.saveButton.Text = "Сохранить";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // DoctorCard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(338, 469);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.genderComboBox);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.buildingTextBox);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.phoneTextBox);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.streetTextBox);
            this.Controls.Add(this.cityTextBox);
            this.Controls.Add(this.emailTextBox);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.dateOfBirthPicker);
            this.Controls.Add(this.loadPhotoButton);
            this.Controls.Add(this.photo);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.experienceNumeric);
            this.Controls.Add(this.specialtyComboBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.patronymicTextBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lastNameTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.firstNameTextBox);
            this.Controls.Add(this.label1);
            this.Name = "DoctorCard";
            this.Text = "Карточка доктора";
            ((System.ComponentModel.ISupportInitialize)(this.experienceNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.photo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox firstNameTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox lastNameTextBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox patronymicTextBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox specialtyComboBox;
        private System.Windows.Forms.NumericUpDown experienceNumeric;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.PictureBox photo;
        private System.Windows.Forms.Button loadPhotoButton;
        private System.Windows.Forms.DateTimePicker dateOfBirthPicker;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox emailTextBox;
        private System.Windows.Forms.TextBox cityTextBox;
        private System.Windows.Forms.TextBox streetTextBox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.MaskedTextBox phoneTextBox;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox buildingTextBox;
        private System.Windows.Forms.ComboBox genderComboBox;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Button saveButton;
    }
}