using System;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class RegPage : Form
    {
        private readonly DatabaseManager _dbManager;

        public RegPage()
        {
            InitializeComponent();
            _dbManager = new DatabaseManager();
            
            // Настройка положения окна
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            if (!_dbManager.TestConnection())
            {
                MessageBox.Show("Ошибка подключения к базе данных", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void signUpBtn_Click(object sender, EventArgs e)
        {
            string usernameValue = username.Text;
            string passwordValue = password.Text;
            string confirmPasswordValue = passwordConfirm.Text;

            if (string.IsNullOrEmpty(usernameValue) || string.IsNullOrEmpty(passwordValue) || string.IsNullOrEmpty(confirmPasswordValue))
            {
                MessageBox.Show("Пожалуйста, заполните все поля", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (passwordValue != confirmPasswordValue)
            {
                MessageBox.Show("Пароли не совпадают", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_dbManager.RegisterUser(usernameValue, passwordValue))
            {
                MessageBox.Show("Регистрация успешна!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoginPage loginForm = new LoginPage();
                loginForm.StartPosition = FormStartPosition.CenterScreen;
                this.Hide();
                loginForm.Show();
            }
            else
            {
                MessageBox.Show("Ошибка при регистрации. Возможно, пользователь уже существует.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void signInLink_Click(object sender, EventArgs e)
        {
            LoginPage loginForm = new LoginPage();
            loginForm.StartPosition = FormStartPosition.CenterScreen;
            loginForm.FormClosed += (s, args) => this.Close();
            this.Hide();
            loginForm.Show();
        }
    }
}
