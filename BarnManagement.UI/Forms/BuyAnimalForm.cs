using BarnManagement.UI.Services;

namespace BarnManagement.UI.Forms
{
    public partial class BuyAnimalForm : Form
    {
        private readonly BarnManagementApiClient _apiClient;

        public BuyAnimalForm(BarnManagementApiClient apiClient)
        {
            _apiClient = apiClient;
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
            // Hardcoded prices for now (will be fetched from API later)
            return animalType switch
            {
                "Chicken" => 50m,
                "Cow" => 500m,
                "Sheep" => 200m,
                _ => 0m
            };
        }

        private async void BuyButton_Click(object? sender, EventArgs e)
        {
            MessageBox.Show("API ile hayvan satın alma henüz implemente edilmedi!", "Bilgi",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            
            // TODO: Implement API call
            // var result = await _apiClient.BuyAnimalAsync(...);
        }

        private void CancelButton_Click(object? sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
