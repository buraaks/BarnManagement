using BarnManagement.UI.Services;
using BarnManagement.UI.Models;
using BarnManagement.UI.CustomControls;

namespace BarnManagement.UI.Forms
{
    public partial class MainForm : Form
    {
        private readonly BarnManagementApiClient _apiClient;
        private UserDto? _currentUser;
        private List<FarmDto> _farms = new();
        private FarmDto? _selectedFarm;

        public MainForm(BarnManagementApiClient apiClient)
        {
            _apiClient = apiClient;
            InitializeComponent();
            this.Load += MainForm_Load;
        }

        private async void MainForm_Load(object? sender, EventArgs e)
        {
            await LoadUserDataAsync();
        }

        private async Task LoadUserDataAsync()
        {
            try
            {
                // Get user profile
                _currentUser = await _apiClient.GetUserProfileAsync();
                if (_currentUser != null)
                {
                    cashLabel.Text = $"Nakit: {_currentUser.Balance:C}";
                }
                else
                {
                    MessageBox.Show("Kullanıcı profili alınamadı!", "Debug", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                // Get user farms
                _farms = await _apiClient.GetUserFarmsAsync();
                
                // Debug: Kaç çiftlik yüklendiğini göster
                this.Text = $"Barn Management - {_farms.Count} çiftlik yüklendi";
                
                if (_farms.Count > 0)
                {
                    _selectedFarm = _farms[0];
                }
                else
                {
                    MessageBox.Show("Hiç çiftlik bulunamadı! API'den 0 çiftlik döndü.", "Debug", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Veri yüklenirken hata: {ex.Message}", "Hata",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BuyAnimalButton_Click(object? sender, EventArgs e)
        {
            if (_selectedFarm == null)
            {
                MessageBox.Show("Önce bir çiftlik oluşturmalısınız!", "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using var buyForm = new BuyAnimalForm(_apiClient, _selectedFarm.Id);
            if (buyForm.ShowDialog() == DialogResult.OK)
            {
                // Hayvan satın alındı, bakiyeyi güncelle
                await LoadUserDataAsync();
            }
        }

        private async void SellProductsButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Ürün satma özelliği yakında eklenecek!", "Bilgi",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private async void SellAllProductsButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Tüm ürünleri satma özelliği yakında eklenecek!", "Bilgi",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private async void ResetGameButton_Click(object? sender, EventArgs e)
        {
            MessageBox.Show("Oyun sıfırlama özelliği yakında eklenecek!", "Bilgi",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
        }
    }
}
