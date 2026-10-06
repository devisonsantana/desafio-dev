using System.Text.Json.Serialization;

namespace Desafio.Core;

public sealed class SalesFile
{
    [JsonPropertyName("vendas")] public IReadOnlyList<Sale> Sales { get; init; } = [];
}
