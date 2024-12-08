using System;
using System.Windows.Forms;
using System.Data;
using WindowsFormsApp1;
using System.Drawing;

namespace DatabaseCursovaya.UI
{
    public partial class UserAccountForm : Form
    {
        private readonly DatabaseManager _dbManager;
        private readonly int _entityId;
        private readonly string _entityType; // "doctor" или "patient"
        private readonly string _entityName; // ФИО доктора или пациента

        public UserAccountForm(int entityId, string entityType, string entityName)
        {
            InitializeComponent();
            _dbManager = new DatabaseManager();
            _entityId = entityId;
            _entityType = entityType;
            _entityName = entityName;

            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            InitializeComponents();
            LoadCurrentAccount();
        }

        private void InitializeComponents()
        {
            this.Text = $"Управление аккаунтом: {_entityName}";

            // Группа для существующего аккаунта
            var currentAccountGroup = new GroupBox
            {
                Text = "Текущий аккаунт",
                Location = new Point(12, 12),
                Size = new Size(360, 100)
            };

            _currentAccountLabel = new Label
            {
                Location = new Point(10, 20),
                Size = new Size(340, 40),
                Text = "Нет привязанного аккаунта"
            };

            _unlinkButton = new Button
            {
                Text = "Отвязать аккаунт",
                Location = new Point(10, 60),
                Size = new Size(340, 30),
                Enabled = false
            };
            _unlinkButton.Click += UnlinkButton_Click;

            currentAccountGroup.Controls.AddRange(new Control[] { _currentAccountLabel, _unlinkButton });

            // Группа для создания нового аккаунта
            var newAccountGroup = new GroupBox
            {
                Text = "Создать новый аккаунт",
                Location = new Point(12, 120),
                Size = new Size(360, 150)
            };

            var usernameLabel = new Label { Text = "Логин:", Location = new Point(10, 20) };
            _usernameTextBox = new TextBox { Location = new Point(10, 40), Size = new Size(340, 20) };

            var passwordLabel = new Label { Text = "Пароль:", Location = new Point(10, 70) };
            _passwordTextBox = new TextBox
            {
                Location = new Point(10, 90),
                Size = new Size(340, 20),
                UseSystemPasswordChar = true
            };

            _createAccountButton = new Button
            {
                Text = "Создать аккаунт",
                Location = new Point(10, 120),
                Size = new Size(340, 30)
            };
            _createAccountButton.Click += CreateAccountButton_Click;

            newAccountGroup.Controls.AddRange(new Control[]
            {
                usernameLabel, _usernameTextBox,
                passwordLabel, _passwordTextBox,
                _createAccountButton
            });

            // Группа для привязки существующего аккаунта
            var linkAccountGroup = new GroupBox
            {
                Text = "Привязать существующий аккаунт",
                Location = new Point(12, 280),
                Size = new Size(360, 100)
            };

            _availableAccountsCombo = new ComboBox
            {
                Location = new Point(10, 20),
                Size = new Size(340, 20),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            _linkAccountButton = new Button
            {
                Text = "Привязать аккаунт",
                Location = new Point(10, 50),
                Size = new Size(340, 30)
            };
            _linkAccountButton.Click += LinkAccountButton_Click;

            linkAccountGroup.Controls.AddRange(new Control[]
            {
                _availableAccountsCombo,
                _linkAccountButton
            });

            this.Controls.AddRange(new Control[]
            {
                currentAccountGroup,
                newAccountGroup,
                linkAccountGroup
            });

            this.Size = new Size(400, 430);

            LoadAvailableAccounts();
        }

        private void LoadCurrentAccount()
        {
            var currentAccount = _dbManager.GetLinkedAccount(_entityId, _entityType);
            if (currentAccount != null)
            {
                _currentAccountLabel.Text = $"Текущий аккаунт: {currentAccount.Username}";
                _unlinkButton.Enabled = true;
            }
        }

        private void LoadAvailableAccounts()
        {
            var accounts = _dbManager.GetAvailableUsers();
            _availableAccountsCombo.DataSource = accounts;
            _availableAccountsCombo.DisplayMember = "username";
            _availableAccountsCombo.ValueMember = "user_id";
        }

        private void CreateAccountButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_usernameTextBox.Text) ||
                string.IsNullOrWhiteSpace(_passwordTextBox.Text))
            {
                MessageBox.Show("Заполните все поля", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                if (_dbManager.CreateAndLinkAccount(
                    _usernameTextBox.Text.Trim(),
                    _passwordTextBox.Text,
                    _entityId,
                    _entityType))
                {
                    MessageBox.Show("Аккаунт успешно создан и привязан",
                        "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadCurrentAccount();
                    LoadAvailableAccounts();
                    _usernameTextBox.Clear();
                    _passwordTextBox.Clear();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при создании аккаунта: {ex.Message}",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LinkAccountButton_Click(object sender, EventArgs e)
        {
            if (_availableAccountsCombo.SelectedValue == null)
            {
                MessageBox.Show("Выберите аккаунт для привязки",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                int userId = Convert.ToInt32(_availableAccountsCombo.SelectedValue);
                if (_dbManager.LinkAccount(userId, _entityId, _entityType))
                {
                    MessageBox.Show("Аккаунт успешно привязан",
                        "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadCurrentAccount();
                    LoadAvailableAccounts();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при привязке аккаунта: {ex.Message}",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UnlinkButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Вы уверены, что хотите отвязать аккаунт?",
                "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    if (_dbManager.UnlinkAccount(_entityId, _entityType))
                    {
                        MessageBox.Show("Аккаунт успешно отвязан",
                            "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadCurrentAccount();
                        LoadAvailableAccounts();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при отвязке аккаунта: {ex.Message}",
                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private Label _currentAccountLabel;
        private Button _unlinkButton;
        private TextBox _usernameTextBox;
        private TextBox _passwordTextBox;
        private Button _createAccountButton;
        private ComboBox _availableAccountsCombo;
        private Button _linkAccountButton;
    }
}