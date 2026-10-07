namespace Desafio.Infrastructure.Services;

public static class InterestCalculator
{
    public const decimal DailyRate = 0.025M;

    public static decimal Calculate(decimal value, DateOnly dueDate, DateOnly today)
    {
        if (value < 0)
            throw new ArgumentException("Value cannot be less than zero.", nameof(value));

        var daysLate = today.DayNumber - dueDate.DayNumber;

        return daysLate <= 0
            ? 0
            : Math.Round(value * DailyRate * daysLate, 2, MidpointRounding.AwayFromZero);
    }
}
