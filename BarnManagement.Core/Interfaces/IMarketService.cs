namespace BarnManagement.Core.Interfaces;

public interface IMarketService
{
    decimal GetProductPrice(string productType);
    void UpdatePrices();
}
