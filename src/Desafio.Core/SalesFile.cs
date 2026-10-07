using System.Text.Json.Serialization;

namespace Desafio.Core;

public sealed class SalesFile
{
    [JsonPropertyName("vendas")] public required IReadOnlyList<Sale> Sales { get; init; }
}
