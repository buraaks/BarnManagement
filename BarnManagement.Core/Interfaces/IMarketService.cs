namespace BarnManagement.Core.Interfaces;
// Kısa mimari özet: Bu arayüz, pazar/fiyat bilgisi sağlayan servis davranışlarının sözleşmesini tanımlar.


public interface IMarketService
{
    decimal GetProductPrice(string productType);
    void UpdatePrices();
}
