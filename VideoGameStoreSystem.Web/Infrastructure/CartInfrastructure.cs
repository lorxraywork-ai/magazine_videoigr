using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace VideoGameStoreSystem.Web.Infrastructure;

public interface ICartService
{
    IReadOnlyList<CartSessionItem> GetItems();

    int GetTotalQuantity();

    void AddItem(int productId, int quantity, int maxQuantity);

    void SetQuantity(int productId, int quantity, int maxQuantity);

    void RemoveItem(int productId);

    void Clear();
}

public class SessionCartService(IHttpContextAccessor httpContextAccessor) : ICartService
{
    private const string SessionKey = "shopping_cart";
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public IReadOnlyList<CartSessionItem> GetItems()
    {
        var session = GetSession();
        if (session is null)
        {
            return [];
        }

        var payload = session.GetString(SessionKey);
        if (string.IsNullOrWhiteSpace(payload))
        {
            return [];
        }

        return JsonSerializer.Deserialize<List<CartSessionItem>>(payload, JsonOptions) ?? [];
    }

    public int GetTotalQuantity()
    {
        return GetItems().Sum(item => item.Quantity);
    }

    public void AddItem(int productId, int quantity, int maxQuantity)
    {
        if (quantity <= 0 || maxQuantity <= 0)
        {
            return;
        }

        var items = GetItems().ToList();
        var existingItem = items.FirstOrDefault(item => item.ProductId == productId);
        if (existingItem is null)
        {
            items.Add(new CartSessionItem
            {
                ProductId = productId,
                Quantity = Math.Min(quantity, maxQuantity)
            });
        }
        else
        {
            existingItem.Quantity = Math.Min(existingItem.Quantity + quantity, maxQuantity);
        }

        SaveItems(items);
    }

    public void SetQuantity(int productId, int quantity, int maxQuantity)
    {
        var items = GetItems().ToList();
        var existingItem = items.FirstOrDefault(item => item.ProductId == productId);
        if (existingItem is null)
        {
            return;
        }

        if (quantity <= 0 || maxQuantity <= 0)
        {
            items.Remove(existingItem);
        }
        else
        {
            existingItem.Quantity = Math.Min(quantity, maxQuantity);
        }

        SaveItems(items);
    }

    public void RemoveItem(int productId)
    {
        var items = GetItems().Where(item => item.ProductId != productId).ToList();
        SaveItems(items);
    }

    public void Clear()
    {
        GetSession()?.Remove(SessionKey);
    }

    private void SaveItems(IEnumerable<CartSessionItem> items)
    {
        var session = GetSession();
        if (session is null)
        {
            return;
        }

        var cleanItems = items
            .Where(item => item.ProductId > 0 && item.Quantity > 0)
            .GroupBy(item => item.ProductId)
            .Select(group => new CartSessionItem
            {
                ProductId = group.Key,
                Quantity = group.Sum(item => item.Quantity)
            })
            .ToList();

        if (cleanItems.Count == 0)
        {
            session.Remove(SessionKey);
            return;
        }

        session.SetString(SessionKey, JsonSerializer.Serialize(cleanItems, JsonOptions));
    }

    private ISession? GetSession()
    {
        return httpContextAccessor.HttpContext?.Session;
    }
}

public class CartSessionItem
{
    public int ProductId { get; set; }

    public int Quantity { get; set; }
}
