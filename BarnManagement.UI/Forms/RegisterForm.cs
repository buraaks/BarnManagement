using BarnManagement.UI.Services;

namespace BarnManagement.UI.Forms
{
    public partial class RegisterForm : Form
    {
        private readonly BarnManagementApiClient _apiClient;
        private Guna.UI2.WinForms.Guna2TextBox? emailTextBox;
        private Guna.UI2.WinForms.Guna2TextBox? usernameTextBox;
        private Guna.UI2.WinForms.Guna2TextBox? passwordTextBox;
        private Guna.UI2.WinForms.Guna2TextBox? passwordConfirmTextBox;
        private Guna.UI2.WinForms.Guna2Button? registerButton;
        private Label? titleLabel;

        public RegisterForm(BarnManagementApiClient apiClient)
        {
            _apiClient = apiClient;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Form
            this.ClientSize = new Size(400, 450);
            this.Text = "Barn Management - Kayıt Ol";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(240, 240, 240);

            // Title
            titleLabel = new Label
            {
                Text = "Yeni Hesap Oluştur",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                Location = new Point(50, 30),
                Size = new Size(300, 35),
                TextAlign = ContentAlignment.MiddleCenter
            };
            this.Controls.Add(titleLabel);

            // Email
            var emailLabel = new Label
            {
                Text = "E-posta:",
                Font = new Font("Segoe UI", 10F),
                Location = new Point(50, 90),
                Size = new Size(100, 20)
            };
            this.Controls.Add(emailLabel);

            emailTextBox = new Guna.UI2.WinForms.Guna2TextBox
            {
                Location = new Point(50, 115),
                Size = new Size(300, 36),
                PlaceholderText = "email@example.com"
            };
            this.Controls.Add(emailTextBox);

            // Username
            var usernameLabel = new Label
            {
                Text = "Kullanıcı Adı:",
                Font = new Font("Segoe UI", 10F),
                Location = new Point(50, 165),
                Size = new Size(120, 20)
            };
            this.Controls.Add(usernameLabel);

            usernameTextBox = new Guna.UI2.WinForms.Guna2TextBox
            {
                Location = new Point(50, 190),
                Size = new Size(300, 36),
                PlaceholderText = "Kullanıcı adınız"
            };
            this.Controls.Add(usernameTextBox);

            // Password
            var passwordLabel = new Label
            {
                Text = "Şifre:",
                Font = new Font("Segoe UI", 10F),
                Location = new Point(50, 240),
                Size = new Size(100, 20)
            };
            this.Controls.Add(passwordLabel);

            passwordTextBox = new Guna.UI2.WinForms.Guna2TextBox
            {
                Location = new Point(50, 265),
                Size = new Size(300, 36),
                PasswordChar = '●',
                PlaceholderText = "Şifreniz"
            };
            this.Controls.Add(passwordTextBox);

            // Confirm Password
            var confirmLabel = new Label
            {
                Text = "Şifre (Tekrar):",
                Font = new Font("Segoe UI", 10F),
                Location = new Point(50, 315),
                Size = new Size(120, 20)
            };
            this.Controls.Add(confirmLabel);

            passwordConfirmTextBox = new Guna.UI2.WinForms.Guna2TextBox
            {
                Location = new Point(50, 340),
                Size = new Size(300, 36),
                PasswordChar = '●',
                PlaceholderText = "Şifrenizi tekrar girin"
            };
            this.Controls.Add(passwordConfirmTextBox);

            // Register Button
            registerButton = new Guna.UI2.WinForms.Guna2Button
            {
                Text = "Kayıt Ol",
                Location = new Point(50, 390),
                Size = new Size(300, 45),
                FillColor = Color.FromArgb(34, 139, 34),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            };
            registerButton.Click += RegisterButton_Click;
            this.Controls.Add(registerButton);

            this.ResumeLayout(false);
        }

        private async void RegisterButton_Click(object? sender, EventArgs e)
        {
            if (emailTextBox == null || usernameTextBox == null || 
                passwordTextBox == null || passwordConfirmTextBox == null || registerButton == null)
                return;

            string email = emailTextBox.Text.Trim();
            string username = usernameTextBox.Text.Trim();
            string password = passwordTextBox.Text;
            string passwordConfirm = passwordConfirmTextBox.Text;

            // Validation
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Tüm alanları doldurun!", "Hata",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (password != passwordConfirm)
            {
                MessageBox.Show("Şifreler eşleşmiyor!", "Hata",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (password.Length < 6)
            {
                MessageBox.Show("Şifre en az 6 karakter olmalı!", "Hata",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            registerButton.Enabled = false;
            registerButton.Text = "Kayıt ediliyor...";

            try
            {
                var result = await _apiClient.RegisterAsync(email, username, password);

                if (result)
                {
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Kayıt başarısız! Bu e-posta zaten kullanılıyor olabilir.", "Hata",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Bağlantı hatası: {ex.Message}", "Hata",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                registerButton.Enabled = true;
                registerButton.Text = "Kayıt Ol";
            }
        }
    }
}
