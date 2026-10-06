using System.Text.Json.Serialization;

namespace Desafio.Core;

public sealed record class StockFile
{
    [JsonPropertyName("estoque")] public IReadOnlyList<Product> Products { get; init; } = [];
}
