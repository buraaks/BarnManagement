using BarnManagement.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace BarnManagement.Business.Services;

public class MarketService : IMarketService
{
    private readonly ILogger<MarketService> _logger;
    private readonly Dictionary<string, decimal> _basePrices = new()
    {
        { "Milk", 15.0m },
        { "Egg", 2.5m },
        { "Wool", 50.0m }
    };

    private readonly Dictionary<string, decimal> _currentPrices = new();
    private readonly Random _random = new();

    public MarketService(ILogger<MarketService> logger)
    {
        _logger = logger;
        foreach (var bp in _basePrices)
        {
            _currentPrices[bp.Key] = bp.Value;
        }
    }

    public decimal GetProductPrice(string productType)
    {
        return _currentPrices.TryGetValue(productType, out var price) ? price : 1.0m;
    }

    public void UpdatePrices()
    {
    }
}
