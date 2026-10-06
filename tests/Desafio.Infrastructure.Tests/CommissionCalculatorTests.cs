using Desafio.Core;

namespace Desafio.Infrastructure.Tests;

public class CommissionCalculatorTests
{
    private static async Task<IReadOnlyList<SellerCommission>> CalculateAsync(string json)
    {
        using var directory = new TempDirectory();
        await File.WriteAllTextAsync(Path.Combine(directory.BaseDirectory, "vendas.json"), json);

        return await CommissionCalculator.Calculate("vendas.json", new JsonFileReader(directory.BaseDirectory));
    }

    [Fact]
    public async Task Calculate_GroupsSalesBySeller()
    {
        var result = await CalculateAsync("""
            { "vendas": [
              { "vendedor": "João Silva",  "valor": 1000.00 },
              { "vendedor": "Maria Souza", "valor": 100.00 },
              { "vendedor": "João Silva",  "valor": 200.00 }
            ] }
            """);

        Assert.Equal(2, result.Count);

        var joao = result.Single(r => r.Seller == "João Silva");
        Assert.Equal(1200.00M, joao.TotalSales);
        Assert.Equal(52.00M, joao.TotalCommission);   // 50 + 2

        var maria = result.Single(r => r.Seller == "Maria Souza");
        Assert.Equal(100.00M, maria.TotalSales);
        Assert.Equal(1.00M, maria.TotalCommission);
    }

    [Fact]
    public async Task Calculate_AppliesRuleToEachSale_NotToTheTotal()
    {
        var result = await CalculateAsync("""
            { "vendas": [
              { "vendedor": "Ana Lima", "valor": 99.99 },
              { "vendedor": "Ana Lima", "valor": 100.00 },
              { "vendedor": "Ana Lima", "valor": 499.99 },
              { "vendedor": "Ana Lima", "valor": 500.00 }
            ] }
            """);

        var ana = Assert.Single(result);
        Assert.Equal(1199.98M, ana.TotalSales);
        Assert.Equal(30.9999M, ana.TotalCommission);  // 0 + 1 + 4,9999 + 25
    }

    [Fact]
    public async Task Calculate_ReturnsSellersInAlphabeticalOrder()
    {
        var result = await CalculateAsync("""
            { "vendas": [
              { "vendedor": "Maria Souza", "valor": 100.00 },
              { "vendedor": "Ana Lima",    "valor": 100.00 },
              { "vendedor": "Carlos Oliveira", "valor": 100.00 }
            ] }
            """);

        Assert.Equal(["Ana Lima", "Carlos Oliveira", "Maria Souza"], result.Select(r => r.Seller));
    }

    [Fact]
    public async Task Calculate_TreatsNamesWithExtraWhitespaceAsTheSameSeller()
    {
        var result = await CalculateAsync("""
            { "vendas": [
              { "vendedor": "João Silva ",  "valor": 100.00 },
              { "vendedor": " João Silva",  "valor": 100.00 }
            ] }
            """);

        var joao = Assert.Single(result);
        Assert.Equal(200.00M, joao.TotalSales);
    }

    [Fact]
    public async Task Calculate_WithNoSales_ReturnsEmptyList()
    {
        var result = await CalculateAsync("""{ "vendas": [] }""");

        Assert.Empty(result);
    }

    [Fact]
    public async Task Calculate_WhenFileDoesNotExist_Throws()
    {
        using var directory = new TempDirectory();

        await Assert.ThrowsAsync<FileNotFoundException>(
            () => CommissionCalculator.Calculate("vendas.json", new JsonFileReader(directory.BaseDirectory)));
    }
}