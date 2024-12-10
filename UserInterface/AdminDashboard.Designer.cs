namespace WindowsFormsApp1
{
    partial class AdminDashboard
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.doctors = new System.Windows.Forms.TabPage();
            this.patients = new System.Windows.Forms.TabPage();
            this.visits = new System.Windows.Forms.TabPage();
            this.logoutBtn = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.tabControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.doctors);
            this.tabControl1.Controls.Add(this.patients);
            this.tabControl1.Controls.Add(this.visits);
            this.tabControl1.Location = new System.Drawing.Point(-1, 39);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(802, 413);
            this.tabControl1.TabIndex = 0;
            // 
            // doctors
            // 
            this.doctors.Location = new System.Drawing.Point(4, 22);
            this.doctors.Name = "doctors";
            this.doctors.Padding = new System.Windows.Forms.Padding(3);
            this.doctors.Size = new System.Drawing.Size(794, 387);
            this.doctors.TabIndex = 0;
            this.doctors.Text = "Доктора";
            this.doctors.UseVisualStyleBackColor = true;
            // 
            // patients
            // 
            this.patients.Location = new System.Drawing.Point(4, 22);
            this.patients.Name = "patients";
            this.patients.Size = new System.Drawing.Size(794, 387);
            this.patients.TabIndex = 2;
            this.patients.Text = "Пациенты";
            this.patients.UseVisualStyleBackColor = true;
            // 
            // visits
            // 
            this.visits.Location = new System.Drawing.Point(4, 22);
            this.visits.Name = "visits";
            this.visits.Padding = new System.Windows.Forms.Padding(3);
            this.visits.Size = new System.Drawing.Size(794, 387);
            this.visits.TabIndex = 1;
            this.visits.Text = "Визиты";
            this.visits.UseVisualStyleBackColor = true;
            // 
            // logoutBtn
            // 
            this.logoutBtn.Location = new System.Drawing.Point(681, 12);
            this.logoutBtn.Name = "logoutBtn";
            this.logoutBtn.Size = new System.Drawing.Size(113, 34);
            this.logoutBtn.TabIndex = 1;
            this.logoutBtn.Text = "Выйти";
            this.logoutBtn.UseVisualStyleBackColor = true;
            this.logoutBtn.Click += new System.EventHandler(this.logoutBtn_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(127, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Режим: Администратор";
            // 
            // AdminDashboard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.logoutBtn);
            this.Controls.Add(this.tabControl1);
            this.Name = "AdminDashboard";
            this.Text = "AdminDashboard";
            this.tabControl1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage doctors;
        private System.Windows.Forms.TabPage visits;
        private System.Windows.Forms.Button logoutBtn;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabPage patients;
    }
}