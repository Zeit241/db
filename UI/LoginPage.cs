using System;
using System.Windows.Forms;
using DatabaseCursovaya;
using DatabaseCursovaya.UI;

namespace WindowsFormsApp1
{
    public partial class LoginPage : Form
    {
        private readonly DatabaseManager dbManager;
        public LoginPage()
        {

            InitializeComponent();
            dbManager = new DatabaseManager();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (!dbManager.TestConnection())
            {
                MessageBox.Show("Ошибка подключения к базе данных", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void signUpLink_Click(object sender, EventArgs e)
        {
            RegPage registerForm = new RegPage();
            registerForm.FormClosed += (s, args) => this.Show();
            registerForm.StartPosition = FormStartPosition.CenterScreen;
            this.Hide();
            registerForm.FormClosed += (s, args) =>
            {
                this.Hide();
                this.Close();
            };
            registerForm.Show();

        }

        private void signIn_Click(object sender, EventArgs e)
        {
            string username = this.username.Text;
            string password = this.password.Text;


            if(string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Пожалуйста, введите логин и пароль", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var (success, userId, role) = dbManager.ValidateUser(username, password);

            if (success)
            {
                UserSession.Instance.Initialize(userId, username, role);

                MessageBox.Show("Авторизация успешна!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);

              
                Form nextForm;
                if (role == "admin")
                {
                    nextForm = new AdminDashboard(); // Форма для администратора
                }
                else if (role == "doctor")
                {
                    nextForm = new DoctorPage(); // Форма для обычного пользователя
                }
                else
                {
                    nextForm = new UserDashboard();

                }

                nextForm.FormClosed += (s, args) =>
                {
                    UserSession.Instance.Clear(); // Очищаем сессию при закрытии формы
             
                    this.Close();
                };

                this.Hide();
                nextForm.Show();
            }
            else
            {
                MessageBox.Show("Неверный логин или пароль", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
