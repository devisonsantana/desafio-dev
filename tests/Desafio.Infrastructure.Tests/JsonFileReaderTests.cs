using Desafio.Core;

namespace Desafio.Infrastructure.Tests;

public class JsonFileReaderTests
{
    [Fact]
    public async Task ReadJsonAsync_ShouldReadSalesFromJson()
    {
        // arrange
        using var directory = new TempDirectory();
        var filename = "vendas.json";
        var json = """
        {
          "vendas": [
            {
              "vendedor": "João Silva",
              "valor": 1200.50
            },
            {
              "vendedor": "Maria Souza",
              "valor": 500.00
            }
          ]
        }
        """;
        var filePath = Path.Combine(directory.BaseDirectory, filename);

        await File.WriteAllTextAsync(filePath, json);

        // act
        var reader = new JsonFileReader(directory.BaseDirectory);
        var result = await reader.ReadJsonAsync<SalesFile>(filename);

        // assert
        Assert.Equal(2, result.Sales.Count);

        Assert.Equal("João Silva", result.Sales[0].Seller);
        Assert.Equal(1200.50M, result.Sales[0].Value);

        Assert.Equal("Maria Souza", result.Sales[1].Seller);
        Assert.Equal(500.00M, result.Sales[1].Value);
    }

    [Fact]
    public async Task ReadJsonAsync_ShouldReadStockFromJson()
    {
        // arrange
        using var directory = new TempDirectory();
        var filename = "estoque.json";
        var json = """
        {
            "estoque": [
                {
                    "codigoProduto": 101,
                    "descricaoProduto": "Caneta Azul",
                    "estoque": 150
                },
                {
                    "codigoProduto": 102,
                    "descricaoProduto": "Caderno Universitário",
                    "estoque": 75
                }
            ]
        }
        """;
        var filePath = Path.Combine(directory.BaseDirectory, filename);

        await File.WriteAllTextAsync(filePath, json);

        // act
        var reader = new JsonFileReader(directory.BaseDirectory);
        var result = await reader.ReadJsonAsync<StockFile>(filename);

        // assert
        Assert.Equal(2, result.Products.Count);

        Assert.Equal(101, result.Products[0].Code);
        Assert.Equal("Caneta Azul", result.Products[0].Description);
        Assert.Equal(150, result.Products[0].StockQuantity);

        Assert.Equal(102, result.Products[1].Code);
        Assert.Equal("Caderno Universitário", result.Products[1].Description);
        Assert.Equal(75, result.Products[1].StockQuantity);
    }
}
