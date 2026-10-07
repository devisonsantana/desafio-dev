namespace Desafio.Console;

using System;
using System.Globalization;
using Desafio.Infrastructure.ModelViews;
using Desafio.Infrastructure.Services;

class Program
{
    static StockCalculator? _stock;
    static bool Running = true;

    public static async Task Main()
    {
        var culture = new CultureInfo("pt-BR");
        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;
        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        var dataDirectory = Path.Combine(AppContext.BaseDirectory, "Data");
        var reader = new JsonFileReader(dataDirectory);

        while (Running)
        {
            Console.WriteLine();
            Console.WriteLine("=== Desafio Técnico ===");
            Console.WriteLine("1 - Comissão por vendedor");
            Console.WriteLine("2 - Controle de Estoque");
            Console.WriteLine("3 - Cálculo de Juros");
            Console.WriteLine("0 - Sair");
            Console.Write("Escolha uma opção: ");

            switch (Console.ReadLine()?.Trim())
            {
                case "1":
                    await ExecuteDesafio1(reader);
                    break;
                case "2":
                    await ExecuteDesafio2(reader);
                    break;
                case "3":
                    ExecuteDesafio3();
                    break;
                case "0":
                    Running = false;
                    break;
                default:
                    Console.WriteLine("Opção inválida.");
                    break;
            }
        }
    }
    private static async Task ExecuteDesafio1(JsonFileReader reader)
    {
        Console.WriteLine();
        Console.WriteLine("=== Comissão por vendedor ===");
        var commissions = await CommissionCalculator.Calculate("vendas.json", reader);

        foreach (var commission in commissions)
        {
            Console.WriteLine(
                $"{commission.Seller}: " +
                $"Vendas = {commission.TotalSales:C2} | " +
                $"Comissão = {commission.TotalCommission:C2}");
        }
    }

    private static async Task ExecuteDesafio2(JsonFileReader reader)
    {
        _stock ??= await StockCalculator.Create("estoque.json", reader);

        var back = false;
        while (!back)
        {
            Console.WriteLine();
            Console.WriteLine("=== Controle de Estoque ===");
            Console.WriteLine("1 - Lançar movimentação");
            Console.WriteLine("2 - Ver movimentações");
            Console.WriteLine("0 - Voltar");
            Console.Write("Escolha uma opção: ");

            switch (Console.ReadLine()?.Trim())
            {
                case "1":
                    RegisterMovement(_stock);
                    break;
                case "2":
                    ShowMovements(_stock);
                    break;
                case "0":
                    back = true;
                    break;
                default:
                    Console.WriteLine("Opção inválida.");
                    break;
            }
        }
    }

    private static void RegisterMovement(StockCalculator stock)
    {
        Console.WriteLine();
        foreach (var item in stock.Items)
            Console.WriteLine($"{item.Code} - {item.Description}: {item.StockQuantity}");

        Console.WriteLine();
        Console.Write("Código do produto: ");
        if (!int.TryParse(Console.ReadLine(), out var code))
        {
            Console.WriteLine("Código inválido.");
            return;
        }

        Console.Write("Tipo (1 - Entrada, 2 - Saída): ");
        if (!int.TryParse(Console.ReadLine(), out var typeNumber) ||
            !Enum.IsDefined(typeof(MovementType), typeNumber))
        {
            Console.WriteLine("Tipo inválido.");
            return;
        }

        Console.Write("Quantidade: ");
        if (!int.TryParse(Console.ReadLine(), out var quantity))
        {
            Console.WriteLine("Quantidade inválida.");
            return;
        }

        Console.Write("Descrição (ex.: Compra, Venda, Devolução): ");
        var description = Console.ReadLine() ?? string.Empty;

        try
        {
            var movement = stock.Register(code, (MovementType)typeNumber, quantity, description);
            Console.WriteLine();
            Console.WriteLine(
                $"Movimentação #{movement.Id} registrada ({movement.Description}). " +
                $"Estoque final do produto {movement.ProductCode}: {movement.BalanceAfter}");
        }
        catch (Exception ex) when (ex is ArgumentException or KeyNotFoundException or InvalidOperationException)
        {
            Console.WriteLine($"Erro: {ex.Message}");
        }
    }

    private static void ShowMovements(StockCalculator stock)
    {
        Console.WriteLine();
        Console.WriteLine("=== Movimentações ===");

        if (stock.Movements.Count == 0)
        {
            Console.WriteLine("Nenhuma movimentação registrada.");
            return;
        }

        foreach (var m in stock.Movements)
        {
            var type = m.Type == MovementType.Entry ? "Entrada" : "Saída";
            Console.WriteLine(
                $"#{m.Id} | {m.Date:dd/MM/yyyy HH:mm:ss} | Produto {m.ProductCode} | " +
                $"{type} de {m.Quantity} | {m.Description} | Saldo: {m.BalanceAfter}");
        }
    }

    private static void ExecuteDesafio3()
    {
        Console.WriteLine();
        Console.WriteLine("=== Cálculo de Juros ===");

        Console.Write("Valor (ex.: 1500,00): ");
          if (!decimal.TryParse(Console.ReadLine(), NumberStyles.AllowDecimalPoint,
          CultureInfo.CurrentCulture, out var value) || value < 0)
        {
            Console.WriteLine("Valor inválido.");
            return;
        }

        Console.Write("Data de vencimento (dd/MM/aaaa): ");
        if (!DateOnly.TryParseExact(Console.ReadLine(), "dd/MM/yyyy", out var dueDate))
        {
            Console.WriteLine("Data inválida.");
            return;
        }

        var today = DateOnly.FromDateTime(DateTime.Today);
        var interest = InterestCalculator.Calculate(value, dueDate, today);
        var daysLate = Math.Max(0, today.DayNumber - dueDate.DayNumber);

        Console.WriteLine();
        Console.WriteLine($"Dias em atraso: {daysLate}");
        Console.WriteLine($"Juros: {interest:C2}");
        Console.WriteLine($"Total a pagar: {(value + interest):C2}");
    }
}