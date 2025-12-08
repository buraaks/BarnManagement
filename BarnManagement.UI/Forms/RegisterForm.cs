using BarnManagement.UI.Services;

namespace BarnManagement.UI.Forms
{
    public partial class RegisterForm : Form
    {
        private readonly BarnManagementApiClient _apiClient;

        public RegisterForm(BarnManagementApiClient apiClient)
        {
            _apiClient = apiClient;
            InitializeComponent();
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

        private void CancelButton_Click(object? sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
