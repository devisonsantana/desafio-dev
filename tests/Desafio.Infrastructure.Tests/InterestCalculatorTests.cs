namespace Desafio.Infrastructure.Tests;

public class InterestCalculatorTests
{
    private static readonly DateOnly Today = new(2026, 10, 6);

    [Theory]
    [InlineData(0)]   // vence hoje
    [InlineData(-1)]  // vence amanhã
    [InlineData(-30)] // vence daqui a 30 dias
    public void NotLate_ReturnsZero(int daysLate)
    {
        var dueDate = Today.AddDays(-daysLate);

        Assert.Equal(0M, InterestCalculator.Calculate(1000M, dueDate, Today));
    }

    [Theory]
    [InlineData(1, 25.00)]
    [InlineData(10, 250.00)]
    [InlineData(30, 750.00)]
    public void Late_UsesSimpleInterest_2_5PercentPerDay(int daysLate, double expected)
    {
        var dueDate = Today.AddDays(-daysLate);

        Assert.Equal((decimal)expected, InterestCalculator.Calculate(1000M, dueDate, Today));
    }

    [Fact]
    public void Result_IsRoundedToTwoDecimals()
    {
        // 100,10 * 0,025 * 1 = 2,5025 -> 2,50
        var dueDate = Today.AddDays(-1);

        Assert.Equal(2.50M, InterestCalculator.Calculate(100.10M, dueDate, Today));
    }

    [Fact]
    public void ZeroValue_ReturnsZero()
        => Assert.Equal(0M, InterestCalculator.Calculate(0M, Today.AddDays(-5), Today));

    [Fact]
    public void NegativeValue_Throws()
        => Assert.Throws<ArgumentException>(
            () => InterestCalculator.Calculate(-1M, Today.AddDays(-1), Today));

    [Fact]
    public void Midpoint_RoundsAwayFromZero()
    {
        // 0,20 * 0,025 = 0,005 -> 0,01
        Assert.Equal(0.01M, InterestCalculator.Calculate(0.20M, Today.AddDays(-1), Today));
    }
}