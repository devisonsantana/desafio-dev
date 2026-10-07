using Desafio.Core;
using Desafio.Infrastructure.ModelViews;

namespace Desafio.Infrastructure.Services;

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
