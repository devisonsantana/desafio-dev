namespace Desafio.Infrastructure;

public sealed class SellerCommission
{
    public required string Seller { get; init; }
    public required decimal TotalSales { get; init; }
    public required decimal TotalCommission { get; init; }
}
