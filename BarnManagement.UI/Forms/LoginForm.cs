using BarnManagement.UI.Services;

namespace BarnManagement.UI.Forms
{
    public partial class LoginForm : Form
    {
        private readonly BarnManagementApiClient _apiClient;

        public LoginForm(BarnManagementApiClient apiClient)
        {
            _apiClient = apiClient;
            InitializeComponent();
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

        private void RegisterLink_LinkClicked(object? sender, LinkLabelLinkClickedEventArgs e)
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
