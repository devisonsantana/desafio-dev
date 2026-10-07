using Desafio.Core;
using Desafio.Infrastructure.ModelViews;

namespace Desafio.Infrastructure.Services;

public sealed class StockCalculator
{
    private readonly Dictionary<int, Product> _items;
    private readonly List<StockMovement> _movements = [];
    private int _nextId = 1;

    private StockCalculator(IEnumerable<Product> products)
    {
        _items = products.ToDictionary(item => item.Code);
    }
    public static async Task<StockCalculator> Create(string filename, JsonFileReader reader)
    {
        var file = await reader.ReadJsonAsync<StockFile>(filename);
        var items = file.Products;
        return new StockCalculator(items);
    }
    public IReadOnlyCollection<Product> Items => _items.Values;
    public IReadOnlyList<StockMovement> Movements => _movements;

    public StockMovement Register(int productCode, MovementType type, int quantity, string description)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description cannot be empty.", nameof(description));

        if (!_items.TryGetValue(productCode, out var item))
            throw new KeyNotFoundException($"Product {productCode} not found.");

        var balance = type switch
        {
            MovementType.Entry => item.StockQuantity + quantity,
            MovementType.Exit when item.StockQuantity < quantity =>
                throw new InvalidOperationException(
                    $"Insufficient stock. Available: {item.StockQuantity}, requested: {quantity}."),
            MovementType.Exit => item.StockQuantity - quantity,
            _ => throw new ArgumentOutOfRangeException(nameof(type))
        };

        var movement = new StockMovement
        {
            Id = _nextId++,
            Date = DateTime.Now,
            ProductCode = productCode,
            Type = type,
            Description = description.Trim(),
            Quantity = quantity,
            BalanceAfter = balance
        };

        item.StockQuantity = balance;

        _movements.Add(movement);
        return movement;
    }
}