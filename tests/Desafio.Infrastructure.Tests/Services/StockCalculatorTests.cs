using Desafio.Infrastructure.ModelViews;
using Desafio.Infrastructure.Services;

namespace Desafio.Infrastructure.Tests.Services;

public class StockCalculatorTests
{
    private static async Task<StockCalculator> CreateAsync()
    {
        using var directory = new TempDirectory();
        await File.WriteAllTextAsync(Path.Combine(directory.BaseDirectory, "estoque.json"), """
            {
              "estoque": [
                { "codigoProduto": 101, "descricaoProduto": "Caneta Azul", "estoque": 150 },
                { "codigoProduto": 102, "descricaoProduto": "Caderno Universitário", "estoque": 75 }
              ]
            }
            """);

        return await StockCalculator.Create("estoque.json", new JsonFileReader(directory.BaseDirectory));
    }

    private static int QuantityOf(StockCalculator stock, int code)
        => stock.Items.Single(p => p.Code == code).StockQuantity;

    [Fact]
    public async Task Entry_IncreasesStock_AndReturnsFinalBalance()
    {
        var stock = await CreateAsync();

        var movement = stock.Register(101, MovementType.Entry, 50, "Compra");

        Assert.Equal(200, movement.BalanceAfter);
        Assert.Equal(200, QuantityOf(stock, 101));
    }

    [Fact]
    public async Task Exit_DecreasesStock_AndReturnsFinalBalance()
    {
        var stock = await CreateAsync();

        var movement = stock.Register(101, MovementType.Exit, 30, "Venda");

        Assert.Equal(120, movement.BalanceAfter);
        Assert.Equal(120, QuantityOf(stock, 101));
    }

    [Fact]
    public async Task Exit_OfEntireStock_IsAllowed()
    {
        var stock = await CreateAsync();

        var movement = stock.Register(102, MovementType.Exit, 75, "Venda");

        Assert.Equal(0, movement.BalanceAfter);
    }

    [Fact]
    public async Task Exit_AboveAvailable_Throws_AndDoesNotChangeStockOrHistory()
    {
        var stock = await CreateAsync();

        Assert.Throws<InvalidOperationException>(
            () => stock.Register(102, MovementType.Exit, 76, "Venda"));

        Assert.Equal(75, QuantityOf(stock, 102));
        Assert.Empty(stock.Movements);
    }

    [Fact]
    public async Task UnknownProduct_Throws()
    {
        var stock = await CreateAsync();

        Assert.Throws<KeyNotFoundException>(
            () => stock.Register(999, MovementType.Entry, 1, "Compra"));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public async Task InvalidQuantity_Throws(int quantity)
    {
        var stock = await CreateAsync();

        Assert.Throws<ArgumentException>(
            () => stock.Register(101, MovementType.Entry, quantity, "Compra"));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task EmptyDescription_Throws(string description)
    {
        var stock = await CreateAsync();

        Assert.Throws<ArgumentException>(
            () => stock.Register(101, MovementType.Entry, 1, description));
    }

    [Fact]
    public async Task Movements_HaveSequentialUniqueIds()
    {
        var stock = await CreateAsync();

        var first = stock.Register(101, MovementType.Entry, 10, "Compra");
        var second = stock.Register(102, MovementType.Exit, 5, "Venda");

        Assert.Equal(1, first.Id);
        Assert.Equal(2, second.Id);
        Assert.Equal(2, stock.Movements.Count);
    }

    [Fact]
    public async Task FailedMovement_DoesNotConsumeId()
    {
        var stock = await CreateAsync();

        Assert.Throws<InvalidOperationException>(
            () => stock.Register(101, MovementType.Exit, 1000, "Venda"));
        var movement = stock.Register(101, MovementType.Entry, 1, "Compra");

        Assert.Equal(1, movement.Id);
    }

    [Fact]
    public async Task Movement_KeepsDescriptionTrimmed()
    {
        var stock = await CreateAsync();

        var movement = stock.Register(101, MovementType.Entry, 1, "  Devolução  ");

        Assert.Equal("Devolução", movement.Description);
    }

    [Fact]
    public async Task InvalidMovementType_Throws()
    {
        var stock = await CreateAsync();

        Assert.Throws<ArgumentOutOfRangeException>(
            () => stock.Register(101, (MovementType)99, 1, "Teste"));
    }

    [Fact]
    public async Task Register_StoresMovementDetailsInHistory()
    {
        var stock = await CreateAsync();

        var movement = stock.Register(101, MovementType.Exit, 30, "Venda");

        var stored = Assert.Single(stock.Movements);
        Assert.Same(movement, stored);
        Assert.Equal(101, stored.ProductCode);
        Assert.Equal(MovementType.Exit, stored.Type);
        Assert.Equal(30, stored.Quantity);
        Assert.Equal(120, stored.BalanceAfter);
    }
}
