using BarnManagement.UI.Models;
using BarnManagement.UI.Services;

namespace BarnManagement.UI.Forms
{
    public partial class BuyAnimalForm : Form
    {
        private readonly BarnManagementApiClient _apiClient;
        private readonly Guid _farmId;

        public BuyAnimalForm(BarnManagementApiClient apiClient, Guid farmId)
        {
            _apiClient = apiClient;
            _farmId = farmId;
            InitializeComponent();
            InitializeAnimalTypes();
        }

        private void InitializeAnimalTypes()
        {
            animalTypeComboBox.Items.Add("Chicken");
            animalTypeComboBox.Items.Add("Cow");
            animalTypeComboBox.Items.Add("Sheep");
            animalTypeComboBox.SelectedIndex = 0;
        }

        private void AnimalTypeComboBox_SelectedIndexChanged(object? sender, EventArgs e)
        {
            UpdatePrice();
        }

        private void UpdatePrice()
        {
            string? selectedType = animalTypeComboBox.SelectedItem?.ToString();
            if (!string.IsNullOrEmpty(selectedType))
            {
                decimal price = GetAnimalPrice(selectedType);
                priceLabel.Text = price.ToString("C");
            }
        }

        private decimal GetAnimalPrice(string? animalType)
        {
            return animalType switch
            {
                "Chicken" => 50m,
                "Cow" => 400m,
                "Sheep" => 200m,
                _ => 0m
            };
        }




        private int GetProductionInterval(string? animalType)
        {
            return animalType switch
            {
                "Chicken" => 10,     
                "Cow" => 15,      
                "Sheep" => 20,       
                _ => 20
            };
        }

        private async void BuyButton_Click(object? sender, EventArgs e)
        {
            string? animalName = animalNameTextBox.Text?.Trim();
            string? animalType = animalTypeComboBox.SelectedItem?.ToString();

            if (string.IsNullOrEmpty(animalName))
            {
                MessageBox.Show("Lütfen hayvan adı girin!", "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(animalType))
            {
                MessageBox.Show("Lütfen hayvan türü seçin!", "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            buyButton.Enabled = false;
            buyButton.Text = "Satın Alınıyor...";

            try
            {
                var request = new BuyAnimalRequest(
                    Species: animalType,
                    Name: animalName,
                    PurchasePrice: GetAnimalPrice(animalType),
                    ProductionInterval: GetProductionInterval(animalType)
                );

                var (success, error) = await _apiClient.BuyAnimalAsync(_farmId, request);

                if (success)
                {
                    MessageBox.Show($"{request.Name} ({request.Species}) başarıyla satın alındı!\nFiyat: {request.PurchasePrice:C}",
                        "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show($"Satın alma başarısız: {error}", "Hata",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Bir hata oluştu: {ex.Message}", "Hata",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                buyButton.Enabled = true;
                buyButton.Text = "Buy";
            }
        }

        private void CancelButton_Click(object? sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
