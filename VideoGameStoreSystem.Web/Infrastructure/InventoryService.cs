using Microsoft.EntityFrameworkCore;
using VideoGameStoreSystem.Web.Data;
using VideoGameStoreSystem.Web.Models;
using VideoGameStoreSystem.Web.ViewModels.Sales;
using VideoGameStoreSystem.Web.ViewModels.Supplies;
using VideoGameStoreSystem.Web.ViewModels.WriteOffs;

namespace VideoGameStoreSystem.Web.Infrastructure;

public interface IInventoryService
{
    Task<IReadOnlyList<string>> ValidateSaleAsync(IEnumerable<SaleItemInputViewModel> items);

    Task<IReadOnlyList<string>> ValidateWriteOffAsync(IEnumerable<WriteOffItemInputViewModel> items);

    Task ApplySupplyAsync(IEnumerable<SupplyItemInputViewModel> items);

    Task RevertSupplyAsync(IEnumerable<SupplyItem> items);

    Task ApplySaleAsync(IEnumerable<SaleItemInputViewModel> items);

    Task RevertSaleAsync(IEnumerable<SaleItem> items);

    Task ApplyWriteOffAsync(IEnumerable<WriteOffItemInputViewModel> items);

    Task RevertWriteOffAsync(IEnumerable<WriteOffItem> items);
}

public class InventoryService(ApplicationDbContext dbContext) : IInventoryService
{
    public async Task<IReadOnlyList<string>> ValidateSaleAsync(IEnumerable<SaleItemInputViewModel> items)
    {
        return await ValidateNegativeStockAsync(
            items.Select(item => new StockChange(item.ProductId, -item.Quantity)));
    }

    public async Task<IReadOnlyList<string>> ValidateWriteOffAsync(IEnumerable<WriteOffItemInputViewModel> items)
    {
        return await ValidateNegativeStockAsync(
            items.Select(item => new StockChange(item.ProductId, -item.Quantity)));
    }

    public async Task ApplySupplyAsync(IEnumerable<SupplyItemInputViewModel> items)
    {
        await ApplyChangesAsync(items.Select(item => new StockChange(item.ProductId, item.Quantity)));
    }

    public async Task RevertSupplyAsync(IEnumerable<SupplyItem> items)
    {
        await ApplyChangesAsync(items.Select(item => new StockChange(item.ProductId, -item.Quantity)));
    }

    public async Task ApplySaleAsync(IEnumerable<SaleItemInputViewModel> items)
    {
        await ApplyChangesAsync(items.Select(item => new StockChange(item.ProductId, -item.Quantity)));
    }

    public async Task RevertSaleAsync(IEnumerable<SaleItem> items)
    {
        await ApplyChangesAsync(items.Select(item => new StockChange(item.ProductId, item.Quantity)));
    }

    public async Task ApplyWriteOffAsync(IEnumerable<WriteOffItemInputViewModel> items)
    {
        await ApplyChangesAsync(items.Select(item => new StockChange(item.ProductId, -item.Quantity)));
    }

    public async Task RevertWriteOffAsync(IEnumerable<WriteOffItem> items)
    {
        await ApplyChangesAsync(items.Select(item => new StockChange(item.ProductId, item.Quantity)));
    }

    private async Task<IReadOnlyList<string>> ValidateNegativeStockAsync(IEnumerable<StockChange> changes)
    {
        var changeList = changes.ToList();
        if (changeList.Count == 0)
        {
            return ["Добавьте хотя бы одну позицию документа."];
        }

        var grouped = changeList.GroupBy(item => item.ProductId)
            .Select(group => new StockChange(group.Key, group.Sum(item => item.QuantityDelta)))
            .ToList();

        var productIds = grouped.Select(item => item.ProductId).Distinct().ToList();
        var products = await dbContext.Products
            .Where(item => productIds.Contains(item.ProductId))
            .ToDictionaryAsync(item => item.ProductId);

        var errors = new List<string>();
        foreach (var change in grouped)
        {
            if (!products.TryGetValue(change.ProductId, out var product))
            {
                errors.Add($"Товар с кодом {change.ProductId} не найден.");
                continue;
            }

            var nextValue = product.StockQuantity + change.QuantityDelta;
            if (nextValue < 0)
            {
                errors.Add($"Недостаточно остатка для товара «{product.ProductName}». Доступно: {product.StockQuantity}.");
            }
        }

        return errors;
    }

    private async Task ApplyChangesAsync(IEnumerable<StockChange> changes)
    {
        var grouped = changes.GroupBy(item => item.ProductId)
            .Select(group => new StockChange(group.Key, group.Sum(item => item.QuantityDelta)))
            .ToList();

        var errors = await ValidateNegativeStockAsync(grouped);
        if (errors.Count > 0)
        {
            throw new InvalidOperationException(string.Join(Environment.NewLine, errors));
        }

        var productIds = grouped.Select(item => item.ProductId).Distinct().ToList();
        var products = await dbContext.Products
            .Where(item => productIds.Contains(item.ProductId))
            .ToDictionaryAsync(item => item.ProductId);

        foreach (var change in grouped)
        {
            products[change.ProductId].StockQuantity += change.QuantityDelta;
        }
    }

    private readonly record struct StockChange(int ProductId, int QuantityDelta);
}
