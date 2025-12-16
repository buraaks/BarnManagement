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
        private System.Windows.Forms.Timer _refreshTimer;

        public MainForm(BarnManagementApiClient apiClient)
        {
            _apiClient = apiClient;
            InitializeComponent();

            // Timer kurulumu
            _refreshTimer = new System.Windows.Forms.Timer();
            _refreshTimer.Interval = 2000; // 2 saniyede bir yenile
            _refreshTimer.Tick += async (s, args) => await LoadUserDataAsync(true);

            this.Load += MainForm_Load;
            this.FormClosing += MainForm_FormClosing;
        }

        private async void MainForm_Load(object? sender, EventArgs e)
        {
            await LoadUserDataAsync(false);
            _refreshTimer.Start();
        }

        private void MainForm_FormClosing(object? sender, FormClosingEventArgs e)
        {
            _refreshTimer.Stop();
        }

        private async Task LoadUserDataAsync(bool silent = false)
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
                    if (!silent) MessageBox.Show("Kullanıcı profili alınamadı!", "Debug", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                // Get user farms
                _farms = await _apiClient.GetUserFarmsAsync();

                // Debug: Kaç çiftlik yüklendiğini göster
                this.Text = $"Barn Management - {_farms.Count} çiftlik yüklendi";

                if (_farms.Count > 0)
                {
                    _selectedFarm = _farms[0];

                    // Hayvanları yükle
                    var animals = await _apiClient.GetFarmAnimalsAsync(_selectedFarm.Id);
                    animalsGrid.Rows.Clear();
                    foreach (var animal in animals)
                    {
                        int prodProgress = 0;
                        if (animal.NextProductionAt.HasValue && animal.ProductionInterval > 0)
                        {
                            var now = DateTime.UtcNow;
                            var next = animal.NextProductionAt.Value;

                            if (next <= now)
                            {
                                prodProgress = 100;
                            }
                            else
                            {
                                // Kalan süreyi hesapla
                                var remainingSeconds = (next - now).TotalSeconds;
                                // Geçen süre oranı
                                var ratio = 1.0 - (remainingSeconds / animal.ProductionInterval);
                                prodProgress = (int)(ratio * 100);

                                // Sınırla
                                prodProgress = Math.Clamp(prodProgress, 0, 100);
                            }
                        }

                        // YAŞ HESABI: 30 Saniye = 1 Yıl
                        var totalSeconds = (DateTime.UtcNow - animal.BirthDate).TotalSeconds;
                        var ageYears = (int)(totalSeconds / 30.0); // 30 saniyede bir yıl artar

                        animalsGrid.Rows.Add(
                            animal.Id,
                            animal.Name,
                            $"{ageYears} Yıl", // Yıl olarak göster
                            animal.Species,
                            prodProgress
                        );
                    }

                    // Ürünleri yükle
                    var products = await _apiClient.GetFarmProductsAsync(_selectedFarm.Id);
                    productsGrid.Rows.Clear();

                    // Group products by ProductType
                    var groupedProducts = products
                        .GroupBy(p => p.ProductType)
                        .Select(g => new
                        {
                            ProductType = g.Key,
                            TotalQuantity = g.Sum(p => p.Quantity),
                            TotalValue = g.Sum(p => p.SalePrice * p.Quantity),
                            // We keep one ID for reference, though Sell-All doesn't need it.
                            // If single sell is needed, we'd need a different approach.
                            FirstId = g.First().Id
                        })
                        .ToList();

                    foreach (var group in groupedProducts)
                    {
                        // Quantity ve Value toplam olarak gösteriliyor
                        int rowId = productsGrid.Rows.Add(group.ProductType, group.TotalQuantity, group.TotalValue);
                        productsGrid.Rows[rowId].Tag = group.FirstId;
                    }
                }
                else
                {
                    animalsGrid.Rows.Clear();
                    productsGrid.Rows.Clear();
                    if (!silent) MessageBox.Show("Hiç çiftlik bulunamadı! API'den 0 çiftlik döndü.", "Debug", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                if (!silent)
                {
                    MessageBox.Show($"Veri yüklenirken hata: {ex.Message}", "Hata",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
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
                await LoadUserDataAsync(false);
            }
        }

        private async void SellProductsButton_Click(object sender, EventArgs e)
        {
            // Kullanıcı isteği üzerine "Sell Products" butonu "Sell All" işlevi görecek.
            await PerformSellAllProductsAsync();
        }

        private async void SellAllProductsButton_Click(object sender, EventArgs e)
        {
            await PerformSellAllProductsAsync();
        }

        private async Task PerformSellAllProductsAsync()
        {
            if (_selectedFarm == null) return;

            if (productsGrid.Rows.Count == 0)
            {
                MessageBox.Show("Satılacak ürün yok!", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var confirmResult = MessageBox.Show("Tüm ürünleri satmak istediğinize emin misiniz?",
                                     "Onay",
                                     MessageBoxButtons.YesNo);

            if (confirmResult == DialogResult.Yes)
            {
                var (success, earnings, error) = await _apiClient.SellAllProductsAsync(_selectedFarm.Id);

                if (success)
                {
                    MessageBox.Show($"Tüm ürünler satıldı! Toplam Kazanç: {earnings:C}", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await LoadUserDataAsync(false);
                }
                else
                {
                    MessageBox.Show($"Toplu satış başarısız: {error}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private async void ResetGameButton_Click(object? sender, EventArgs e)
        {
            var result = MessageBox.Show(
                "TÜM İLERLEMENİZ SİLİNECEK!\n\nÇiftlikleriniz, hayvanlarınız ve bakiyeniz sıfırlanacak.\nEmin misiniz?",
                "Kritik Uyarı",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2);

            if (result == DialogResult.Yes)
            {
                var success = await _apiClient.ResetGameAsync();
                if (success)
                {
                    MessageBox.Show("Oyun sıfırlandı! Yeni bir başlangıç yapabilirsiniz.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await LoadUserDataAsync(false);
                }
                else
                {
                    MessageBox.Show("Sıfırlama başarısız oldu.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
        }

        private void MainForm_Load_1(object sender, EventArgs e)
        {

        }
    }
}
