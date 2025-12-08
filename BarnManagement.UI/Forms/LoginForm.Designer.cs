namespace BarnManagement.UI.Forms
{
    partial class LoginForm
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
            this.ClientSize = new System.Drawing.Size(400, 350);
            this.Text = "Barn Management - Giriş";
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.MaximizeBox = false;
            this.BackColor = System.Drawing.Color.FromArgb(240, 240, 240);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            
            // Title Label
            this.titleLabel = new System.Windows.Forms.Label();
            this.titleLabel.Text = "Barn Management";
            this.titleLabel.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.titleLabel.Location = new System.Drawing.Point(50, 30);
            this.titleLabel.Size = new System.Drawing.Size(300, 40);
            this.titleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.Controls.Add(this.titleLabel);
            
            // Email Label
            this.emailLabel = new System.Windows.Forms.Label();
            this.emailLabel.Text = "E-posta:";
            this.emailLabel.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.emailLabel.Location = new System.Drawing.Point(50, 100);
            this.emailLabel.Size = new System.Drawing.Size(100, 20);
            this.Controls.Add(this.emailLabel);
            
            // Email TextBox
            this.emailTextBox = new System.Windows.Forms.TextBox();
            this.emailTextBox.Location = new System.Drawing.Point(50, 125);
            this.emailTextBox.Size = new System.Drawing.Size(300, 28);
            this.emailTextBox.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.Controls.Add(this.emailTextBox);
            
            // Password Label
            this.passwordLabel = new System.Windows.Forms.Label();
            this.passwordLabel.Text = "Şifre:";
            this.passwordLabel.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.passwordLabel.Location = new System.Drawing.Point(50, 165);
            this.passwordLabel.Size = new System.Drawing.Size(100, 20);
            this.Controls.Add(this.passwordLabel);
            
            // Password TextBox
            this.passwordTextBox = new System.Windows.Forms.TextBox();
            this.passwordTextBox.Location = new System.Drawing.Point(50, 190);
            this.passwordTextBox.Size = new System.Drawing.Size(300, 28);
            this.passwordTextBox.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.passwordTextBox.PasswordChar = '●';
            this.Controls.Add(this.passwordTextBox);
            
            // Login Button
            this.loginButton = new System.Windows.Forms.Button();
            this.loginButton.Text = "Giriş Yap";
            this.loginButton.Location = new System.Drawing.Point(50, 240);
            this.loginButton.Size = new System.Drawing.Size(300, 40);
            this.loginButton.BackColor = System.Drawing.Color.FromArgb(94, 148, 255);
            this.loginButton.ForeColor = System.Drawing.Color.White;
            this.loginButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.loginButton.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.loginButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.loginButton.Click += new System.EventHandler(this.LoginButton_Click);
            this.Controls.Add(this.loginButton);
            
            // Register Info Label
            this.registerInfoLabel = new System.Windows.Forms.Label();
            this.registerInfoLabel.Text = "Hesabınız yok mu?";
            this.registerInfoLabel.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.registerInfoLabel.Location = new System.Drawing.Point(50, 295);
            this.registerInfoLabel.Size = new System.Drawing.Size(120, 20);
            this.registerInfoLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Controls.Add(this.registerInfoLabel);
            
            // Register Link
            this.registerLink = new System.Windows.Forms.LinkLabel();
            this.registerLink.Text = "Kayıt Ol";
            this.registerLink.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.registerLink.Location = new System.Drawing.Point(170, 295);
            this.registerLink.Size = new System.Drawing.Size(60, 20);
            this.registerLink.LinkColor = System.Drawing.Color.FromArgb(94, 148, 255);
            this.registerLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.RegisterLink_LinkClicked);
            this.Controls.Add(this.registerLink);
            
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label titleLabel;
        private System.Windows.Forms.Label emailLabel;
        private System.Windows.Forms.TextBox emailTextBox;
        private System.Windows.Forms.Label passwordLabel;
        private System.Windows.Forms.TextBox passwordTextBox;
        private System.Windows.Forms.Button loginButton;
        private System.Windows.Forms.Label registerInfoLabel;
        private System.Windows.Forms.LinkLabel registerLink;
    }
}
