using Desafio.Core;

namespace Desafio.Infrastructure;

/// <summary>
/// Considerando que o json abaixo tem registros de vendas de um time comercial, 
/// faça um programa que leia os dados e calcule a comissão de cada vendedor, seguindo a
/// seguinte regra para cada venda:
/// - Vendas abaixo de R$100,00 não gera comissão
/// - Vendas abaixo de R$500,00 gera 1% de comissão
/// - A partir de R$500,00 gera 5% de comissão
/// </summary>
public class CommissionCalculator
{
    public static async Task<IReadOnlyList<SellerCommission>> Calculate(string filename, JsonFileReader reader)
    {
        var salesFile = await reader.ReadJsonAsync<SalesFile>(filename);
        var sales = salesFile.Sales;
        return sales
            .GroupBy(sale => sale.Seller)
            .Select(group => new SellerCommission
            {
                Seller = group.Key,
                TotalSales = group.Sum(sale => sale.Value),
                TotalCommission = group.Sum(sale => sale.Commission)
            })
            .OrderBy(result => result.Seller)
            .ToList();
    }
}
