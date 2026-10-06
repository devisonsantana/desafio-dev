using System.Text.Json.Serialization;

namespace Desafio.Core;

public sealed record class Product
{
    [JsonPropertyName("codigoProduto")]
    public required int Code { get; init; }
    [JsonPropertyName("descricaoProduto")]
    public required string Description { get; init; }
    [JsonPropertyName("estoque")]
    public required int StockQuantity { get; init; }
}
