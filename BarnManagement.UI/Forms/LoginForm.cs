using BarnManagement.UI.Services;

namespace BarnManagement.UI.Forms
{
    public partial class LoginForm : Form
    {
        private readonly BarnManagementApiClient _apiClient;
        private Guna.UI2.WinForms.Guna2TextBox? emailTextBox;
        private Guna.UI2.WinForms.Guna2TextBox? passwordTextBox;
        private Guna.UI2.WinForms.Guna2Button? loginButton;
        private Label? titleLabel;
        private Label? emailLabel;
        private Label? passwordLabel;

        public LoginForm(BarnManagementApiClient apiClient)
        {
            _apiClient = apiClient;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Form
            this.ClientSize = new Size(400, 350);
            this.Text = "Barn Management - Giriş";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(240, 240, 240);

            // Title
            titleLabel = new Label
            {
                Text = "Barn Management",
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                Location = new Point(50, 30),
                Size = new Size(300, 40),
                TextAlign = ContentAlignment.MiddleCenter
            };
            this.Controls.Add(titleLabel);

            // Email Label
            emailLabel = new Label
            {
                Text = "E-posta:",
                Font = new Font("Segoe UI", 10F),
                Location = new Point(50, 100),
                Size = new Size(100, 20)
            };
            this.Controls.Add(emailLabel);

            // Email TextBox
            emailTextBox = new Guna.UI2.WinForms.Guna2TextBox
            {
                Location = new Point(50, 125),
                Size = new Size(300, 36),
                PlaceholderText = "email@example.com"
            };
            this.Controls.Add(emailTextBox);

            // Password Label
            passwordLabel = new Label
            {
                Text = "Şifre:",
                Font = new Font("Segoe UI", 10F),
                Location = new Point(50, 175),
                Size = new Size(100, 20)
            };
            this.Controls.Add(passwordLabel);

            // Password TextBox
            passwordTextBox = new Guna.UI2.WinForms.Guna2TextBox
            {
                Location = new Point(50, 200),
                Size = new Size(300, 36),
                PasswordChar = '●',
                PlaceholderText = "Şifreniz"
            };
            this.Controls.Add(passwordTextBox);

            // Login Button
            loginButton = new Guna.UI2.WinForms.Guna2Button
            {
                Text = "Giriş Yap",
                Location = new Point(50, 260),
                Size = new Size(300, 45),
                FillColor = Color.FromArgb(94, 148, 255),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            };
            loginButton.Click += LoginButton_Click;
            this.Controls.Add(loginButton);

            // Register Link Button
            var registerLabel = new Label
            {
                Text = "Hesabınız yok mu?",
                Font = new Font("Segoe UI", 9F),
                Location = new Point(50, 315),
                Size = new Size(120, 20),
                TextAlign = ContentAlignment.MiddleLeft
            };
            this.Controls.Add(registerLabel);

            var registerButton = new LinkLabel
            {
                Text = "Kayıt Ol",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Location = new Point(170, 315),
                Size = new Size(60, 20),
                LinkColor = Color.FromArgb(94, 148, 255)
            };
            registerButton.LinkClicked += RegisterButton_Click;
            this.Controls.Add(registerButton);

            this.ResumeLayout(false);
        }

        private async void LoginButton_Click(object? sender, EventArgs e)
        {
            if (emailTextBox == null || passwordTextBox == null || loginButton == null)
                return;

            string email = emailTextBox.Text.Trim();
            string password = passwordTextBox.Text;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("E-posta ve şifre gerekli!", "Hata",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            loginButton.Enabled = false;
            loginButton.Text = "Giriş yapılıyor...";

            try
            {
                var authResponse = await _apiClient.LoginAsync(email, password);

                if (authResponse != null)
                {
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Giriş başarısız! E-posta veya şifre hatalı.", "Hata",
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
                loginButton.Enabled = true;
                loginButton.Text = "Giriş Yap";
            }
        }

        private void RegisterButton_Click(object? sender, LinkLabelLinkClickedEventArgs e)
        {
            using var registerForm = new RegisterForm(_apiClient);
            if (registerForm.ShowDialog() == DialogResult.OK)
            {
                MessageBox.Show("Kayıt başarılı! Şimdi giriş yapabilirsiniz.", "Başarılı",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
