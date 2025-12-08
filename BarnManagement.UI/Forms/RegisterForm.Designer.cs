namespace BarnManagement.UI.Forms
{
    partial class RegisterForm
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

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.SuspendLayout();
            
            // Form
            this.ClientSize = new System.Drawing.Size(400, 480);
            this.Text = "Barn Management - Kayıt Ol";
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.MaximizeBox = false;
            this.BackColor = System.Drawing.Color.FromArgb(240, 240, 240);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            
            // Title Label
            this.titleLabel = new System.Windows.Forms.Label();
            this.titleLabel.Text = "Yeni Hesap Oluştur";
            this.titleLabel.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.titleLabel.Location = new System.Drawing.Point(50, 20);
            this.titleLabel.Size = new System.Drawing.Size(300, 35);
            this.titleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.Controls.Add(this.titleLabel);
            
            // Email Label
            this.emailLabel = new System.Windows.Forms.Label();
            this.emailLabel.Text = "E-posta:";
            this.emailLabel.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.emailLabel.Location = new System.Drawing.Point(50, 70);
            this.emailLabel.Size = new System.Drawing.Size(100, 20);
            this.Controls.Add(this.emailLabel);
            
            // Email TextBox
            this.emailTextBox = new System.Windows.Forms.TextBox();
            this.emailTextBox.Location = new System.Drawing.Point(50, 93);
            this.emailTextBox.Size = new System.Drawing.Size(300, 28);
            this.emailTextBox.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.Controls.Add(this.emailTextBox);
            
            // Username Label
            this.usernameLabel = new System.Windows.Forms.Label();
            this.usernameLabel.Text = "Kullanıcı Adı:";
            this.usernameLabel.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.usernameLabel.Location = new System.Drawing.Point(50, 130);
            this.usernameLabel.Size = new System.Drawing.Size(120, 20);
            this.Controls.Add(this.usernameLabel);
            
            // Username TextBox
            this.usernameTextBox = new System.Windows.Forms.TextBox();
            this.usernameTextBox.Location = new System.Drawing.Point(50, 153);
            this.usernameTextBox.Size = new System.Drawing.Size(300, 28);
            this.usernameTextBox.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.Controls.Add(this.usernameTextBox);
            
            // Password Label
            this.passwordLabel = new System.Windows.Forms.Label();
            this.passwordLabel.Text = "Şifre:";
            this.passwordLabel.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.passwordLabel.Location = new System.Drawing.Point(50, 190);
            this.passwordLabel.Size = new System.Drawing.Size(100, 20);
            this.Controls.Add(this.passwordLabel);
            
            // Password TextBox
            this.passwordTextBox = new System.Windows.Forms.TextBox();
            this.passwordTextBox.Location = new System.Drawing.Point(50, 213);
            this.passwordTextBox.Size = new System.Drawing.Size(300, 28);
            this.passwordTextBox.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.passwordTextBox.PasswordChar = '●';
            this.Controls.Add(this.passwordTextBox);
            
            // Confirm Password Label
            this.confirmLabel = new System.Windows.Forms.Label();
            this.confirmLabel.Text = "Şifre (Tekrar):";
            this.confirmLabel.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.confirmLabel.Location = new System.Drawing.Point(50, 250);
            this.confirmLabel.Size = new System.Drawing.Size(120, 20);
            this.Controls.Add(this.confirmLabel);
            
            // Confirm Password TextBox
            this.passwordConfirmTextBox = new System.Windows.Forms.TextBox();
            this.passwordConfirmTextBox.Location = new System.Drawing.Point(50, 273);
            this.passwordConfirmTextBox.Size = new System.Drawing.Size(300, 28);
            this.passwordConfirmTextBox.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.passwordConfirmTextBox.PasswordChar = '●';
            this.Controls.Add(this.passwordConfirmTextBox);
            
            // Register Button
            this.registerButton = new System.Windows.Forms.Button();
            this.registerButton.Text = "Kayıt Ol";
            this.registerButton.Location = new System.Drawing.Point(50, 330);
            this.registerButton.Size = new System.Drawing.Size(300, 40);
            this.registerButton.BackColor = System.Drawing.Color.ForestGreen;
            this.registerButton.ForeColor = System.Drawing.Color.White;
            this.registerButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.registerButton.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.registerButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.registerButton.Click += new System.EventHandler(this.RegisterButton_Click);
            this.Controls.Add(this.registerButton);
            
            // Cancel Button
            this.cancelButton = new System.Windows.Forms.Button();
            this.cancelButton.Text = "İptal";
            this.cancelButton.Location = new System.Drawing.Point(50, 385);
            this.cancelButton.Size = new System.Drawing.Size(300, 35);
            this.cancelButton.BackColor = System.Drawing.Color.LightGray;
            this.cancelButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cancelButton.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cancelButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            this.Controls.Add(this.cancelButton);
            
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label titleLabel;
        private System.Windows.Forms.Label emailLabel;
        private System.Windows.Forms.TextBox emailTextBox;
        private System.Windows.Forms.Label usernameLabel;
        private System.Windows.Forms.TextBox usernameTextBox;
        private System.Windows.Forms.Label passwordLabel;
        private System.Windows.Forms.TextBox passwordTextBox;
        private System.Windows.Forms.Label confirmLabel;
        private System.Windows.Forms.TextBox passwordConfirmTextBox;
        private System.Windows.Forms.Button registerButton;
        private System.Windows.Forms.Button cancelButton;
    }
}
