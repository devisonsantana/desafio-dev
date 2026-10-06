namespace Desafio.Core.Tests;

public class ProductTests
{
    [Fact]
    public void Create_WithValidData_ShouldCreateProduct()
    {
        var product = new Product { Code = 101, Description = "Caneta Azul", StockQuantity = 20 };

        Assert.Equal(101, product.Code);
        Assert.Equal("Caneta Azul", product.Description);
        Assert.Equal(20, product.StockQuantity);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_WithEmptyOrNullDescription_ShouldThrow(string? description)
    {
        var action = () => new Product { Code = 101, Description = description!, StockQuantity = 1 };

        Assert.Throws<ArgumentException>(action);
    }

    [Fact]
    public void Create_WithWhitespaceInDescription_ShouldTrim()
    {
        var product = new Product { Code = 101, Description = "  Caneta Azul  ", StockQuantity = 1 };

        Assert.Equal("Caneta Azul", product.Description);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Create_WithNonPositiveCode_ShouldThrow(int code)
    {
        var action = () => new Product { Code = code, Description = "Caneta Azul", StockQuantity = 1 };

        Assert.Throws<ArgumentException>(action);
    }

    [Fact]
    public void Create_WithZeroStock_ShouldBeAllowed()
    {
        var product = new Product { Code = 101, Description = "Caneta Azul", StockQuantity = 0 };

        Assert.Equal(0, product.StockQuantity);
    }

    [Fact]
    public void Create_WithNegativeStock_ShouldThrow()
    {
        var action = () => new Product { Code = 101, Description = "Caneta Azul", StockQuantity = -5 };

        Assert.Throws<ArgumentException>(action);
    }

    [Fact]
    public void SetStockQuantity_ToNegative_ShouldThrow()
    {
        var product = new Product { Code = 101, Description = "Caneta Azul", StockQuantity = 10 };

        Assert.Throws<ArgumentException>(() => product.StockQuantity = -1);
    }
}