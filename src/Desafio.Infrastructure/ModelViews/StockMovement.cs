namespace Desafio.Infrastructure.ModelViews;

public sealed class StockMovement
{
    public required int Id { get; init; }
    public required DateTime Date { get; init; }
    public required int ProductCode { get; init; }
    public required MovementType Type { get; init; }
    public required string Description { get; init; }
    public required int Quantity { get; init; }
    public required int BalanceAfter { get; init; }
}