using System.Text.Json.Serialization;

namespace Desafio.Core;

public sealed class Product
{
    [JsonPropertyName("codigoProduto")]
    public required int Code
    {
        get; init
        {
            if (value <= 0)
                throw new ArgumentException("Code must be a positive number.");

            field = value;
        }
    }
    [JsonPropertyName("descricaoProduto")]
    public required string Description
    {
        get; init
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Description cannot be empty.");

            field = value.Trim();
        }
    }
    [JsonPropertyName("estoque")]
    public required int StockQuantity
    {
        get; set
        {
            if (value < 0)
                throw new ArgumentException("Quantity cannot be negative.");

            field = value;
        }
    }
}
