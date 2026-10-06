namespace Desafio.Core.Tests;

public class SaleTests
{
    [Fact]
    public void Create_WithValidSellerAndValue_ShouldCreateSale()
    {
        var seller = "João Silva";
        var value = 1200.50M;

        var sale = new Sale { Seller = seller, Value = value };

        Assert.Equal(seller, sale.Seller);
        Assert.Equal(value, sale.Value);
    }

    [Fact]
    public void Create_WithContainingWhitespaces_ShouldTrimSeller()
    {
        var sale = new Sale { Seller = "  João Silva  ", Value = 1200.00M };

        Assert.Equal("João Silva", sale.Seller);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("  ")]
    [InlineData(null)]
    public void Create_WithEmptyOrNullSeller_ShouldThrowException(string? seller)
    {
        var action = () => new Sale { Seller = seller!, Value = 1200.00M };

        Assert.Throws<ArgumentException>(action);
    }

    [Fact]
    public void Create_WithValueLessThanZero_ShouldThrowException()
    {
        var action = () => new Sale { Seller = "João Silva", Value = -0.1M };

        Assert.Throws<ArgumentException>(action);
    }

    [Theory]
    [InlineData(99, 0)]
    [InlineData(99.99, 0)]
    [InlineData(100, 1)]
    [InlineData(200, 2)]
    [InlineData(499.99, 4.9999)]
    [InlineData(500, 25)]
    [InlineData(1000, 50)]
    public void ShouldCalculateCommission(decimal value, decimal expected)
    {
        var sale = new Sale { Seller = "João Silva", Value = value };

        Assert.Equal(expected, sale.Commission);
    }
}
