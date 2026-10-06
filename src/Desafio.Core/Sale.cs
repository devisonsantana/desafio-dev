using System.Text.Json.Serialization;

namespace Desafio.Core;

public sealed class Sale
{
    [JsonPropertyName("vendedor")]
    public required string Seller
    {
        get;
        init
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Seller cannot be empty.");

            field = value.Trim();
        }
    }
    [JsonPropertyName("valor")]
    public required decimal Value
    {
        get;
        init
        {
            if (value < 0)
                throw new ArgumentException("Value cannot be less than zero.");

            field = value;
        }
    }
    public decimal Commission => Value switch
    {
        < 100 => 0,
        < 500 => Value * 0.01M,
        _ => Value * 0.05M
    };
}