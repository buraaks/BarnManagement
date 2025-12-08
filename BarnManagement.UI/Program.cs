using BarnManagement.UI.Forms;
using BarnManagement.UI.Services;
using Microsoft.Extensions.Configuration;

namespace BarnManagement.UI
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            // Load configuration
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Application.StartupPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            // Create API client
            var apiClient = new BarnManagementApiClient(configuration);

            // Show login form first
            using var loginForm = new LoginForm(apiClient);
            if (loginForm.ShowDialog() == DialogResult.OK)
            {
                // If login successful, show main form
                Application.Run(new MainForm(apiClient));
            }
        }
    }
}