namespace WindowsFormsApp1
{
    partial class RegPage
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
            this.label4 = new System.Windows.Forms.Label();
            this.signInLink = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.password = new System.Windows.Forms.TextBox();
            this.username = new System.Windows.Forms.TextBox();
            this.signUpBtn = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.passwordConfirm = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Calibri", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label4.Location = new System.Drawing.Point(12, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(254, 66);
            this.label4.TabIndex = 13;
            this.label4.Text = "MEDLUX\r\nПанель регистрации\r\n";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // signInLink
            // 
            this.signInLink.AutoSize = true;
            this.signInLink.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.signInLink.Location = new System.Drawing.Point(118, 296);
            this.signInLink.Name = "signInLink";
            this.signInLink.Size = new System.Drawing.Size(31, 13);
            this.signInLink.TabIndex = 12;
            this.signInLink.Text = "Вход";
            this.signInLink.Click += new System.EventHandler(this.signInLink_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(43, 131);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(45, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "Пароль";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(43, 88);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "Логин";
            // 
            // password
            // 
            this.password.Location = new System.Drawing.Point(42, 146);
            this.password.Name = "password";
            this.password.Size = new System.Drawing.Size(196, 20);
            this.password.TabIndex = 9;
            // 
            // username
            // 
            this.username.Location = new System.Drawing.Point(42, 104);
            this.username.Name = "username";
            this.username.Size = new System.Drawing.Size(196, 20);
            this.username.TabIndex = 8;
            // 
            // signUpBtn
            // 
            this.signUpBtn.Location = new System.Drawing.Point(85, 232);
            this.signUpBtn.Name = "signUpBtn";
            this.signUpBtn.Size = new System.Drawing.Size(102, 36);
            this.signUpBtn.TabIndex = 7;
            this.signUpBtn.Text = "Регистрация";
            this.signUpBtn.UseVisualStyleBackColor = true;
            this.signUpBtn.Click += new System.EventHandler(this.signUpBtn_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(43, 178);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(83, 13);
            this.label5.TabIndex = 15;
            this.label5.Text = "Повтор пароля";
            // 
            // passwordConfirm
            // 
            this.passwordConfirm.Location = new System.Drawing.Point(42, 193);
            this.passwordConfirm.Name = "passwordConfirm";
            this.passwordConfirm.Size = new System.Drawing.Size(196, 20);
            this.passwordConfirm.TabIndex = 14;
            // 
            // RegPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(282, 318);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.passwordConfirm);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.signInLink);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.password);
            this.Controls.Add(this.username);
            this.Controls.Add(this.signUpBtn);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "RegPage";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Регистрация";
            this.Load += new System.EventHandler(this.Form2_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label signInLink;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox password;
        private System.Windows.Forms.TextBox username;
        private System.Windows.Forms.Button signUpBtn;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox passwordConfirm;
    }
}