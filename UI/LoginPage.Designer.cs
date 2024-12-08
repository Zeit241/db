namespace WindowsFormsApp1
{
    partial class LoginPage
    {
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
            this.label4 = new System.Windows.Forms.Label();
            this.signUpLink = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.password = new System.Windows.Forms.TextBox();
            this.username = new System.Windows.Forms.TextBox();
            this.signIn = new System.Windows.Forms.Button();
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
            this.label4.Size = new System.Drawing.Size(258, 66);
            this.label4.TabIndex = 20;
            this.label4.Text = "MEDLUX\r\nПанель авторизации\r\n";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // signUpLink
            // 
            this.signUpLink.AutoSize = true;
            this.signUpLink.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.signUpLink.Location = new System.Drawing.Point(99, 240);
            this.signUpLink.Name = "signUpLink";
            this.signUpLink.Size = new System.Drawing.Size(72, 13);
            this.signUpLink.TabIndex = 19;
            this.signUpLink.Text = "Регистрация";
            this.signUpLink.Click += new System.EventHandler(this.signUpLink_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(43, 131);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(45, 13);
            this.label5.TabIndex = 18;
            this.label5.Text = "Пароль";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(43, 88);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(38, 13);
            this.label6.TabIndex = 17;
            this.label6.Text = "Логин";
            // 
            // password
            // 
            this.password.Location = new System.Drawing.Point(42, 146);
            this.password.Name = "password";
            this.password.PasswordChar = '*';
            this.password.Size = new System.Drawing.Size(196, 20);
            this.password.TabIndex = 16;
            this.password.Text = "pass";
            // 
            // username
            // 
            this.username.Location = new System.Drawing.Point(42, 104);
            this.username.Name = "username";
            this.username.Size = new System.Drawing.Size(196, 20);
            this.username.TabIndex = 15;
            this.username.Text = "admin";
            // 
            // signIn
            // 
            this.signIn.Location = new System.Drawing.Point(76, 187);
            this.signIn.Name = "signIn";
            this.signIn.Size = new System.Drawing.Size(124, 38);
            this.signIn.TabIndex = 14;
            this.signIn.Text = "Войти";
            this.signIn.UseVisualStyleBackColor = true;
            this.signIn.Click += new System.EventHandler(this.signIn_Click);
            // 
            // LoginPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(270, 275);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.signUpLink);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.password);
            this.Controls.Add(this.username);
            this.Controls.Add(this.signIn);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "LoginPage";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Авторизация";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label signUpLink;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox password;
        private System.Windows.Forms.TextBox username;
        private System.Windows.Forms.Button signIn;
    }
}

