using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VideoGameStoreSystem.Web.Data;
using VideoGameStoreSystem.Web.Infrastructure;
using VideoGameStoreSystem.Web.Models;
using VideoGameStoreSystem.Web.Permissions;
using VideoGameStoreSystem.Web.ViewModels.Cart;
using VideoGameStoreSystem.Web.ViewModels.Common;
using VideoGameStoreSystem.Web.ViewModels.Sales;

namespace VideoGameStoreSystem.Web.Controllers;

[AllowAnonymous]
public class CartController(
    ApplicationDbContext dbContext,
    ICartService cartService,
    IInventoryService inventoryService) : Controller
{
    public async Task<IActionResult> Index()
    {
        return View(await BuildViewModelAsync());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(int productId, int quantity = 1, string? returnUrl = null)
    {
        if (quantity <= 0)
        {
            this.SetToastError("Количество товара должно быть больше нуля.");
            return RedirectToLocal(returnUrl);
        }

        var product = await dbContext.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.ProductId == productId);

        if (product is null || !product.IsActive)
        {
            this.SetToastError("Товар недоступен для добавления в корзину.");
            return RedirectToLocal(returnUrl);
        }

        if (product.StockQuantity <= 0)
        {
            this.SetToastError("Товара нет в наличии.");
            return RedirectToLocal(returnUrl);
        }

        var currentQuantity = cartService.GetItems()
            .Where(item => item.ProductId == productId)
            .Select(item => item.Quantity)
            .FirstOrDefault();

        cartService.AddItem(productId, quantity, product.StockQuantity);

        var actualQuantity = cartService.GetItems()
            .Where(item => item.ProductId == productId)
            .Select(item => item.Quantity)
            .FirstOrDefault();

        if (actualQuantity < currentQuantity + quantity)
        {
            this.SetToastInfo($"Количество товара «{product.ProductName}» в корзине ограничено остатком на складе.");
        }
        else
        {
            this.SetToastSuccess($"Товар «{product.ProductName}» добавлен в корзину.");
        }

        return RedirectToLocal(returnUrl);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(int productId, int quantity)
    {
        var product = await dbContext.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.ProductId == productId);

        if (product is null)
        {
            cartService.RemoveItem(productId);
            this.SetToastError("Товар больше не найден в каталоге и удален из корзины.");
            return RedirectToAction(nameof(Index));
        }

        if (!product.IsActive || product.StockQuantity <= 0 || quantity <= 0)
        {
            cartService.RemoveItem(productId);
            this.SetToastInfo($"Товар «{product.ProductName}» удален из корзины.");
            return RedirectToAction(nameof(Index));
        }

        cartService.SetQuantity(productId, quantity, product.StockQuantity);
        var actualQuantity = cartService.GetItems()
            .Where(item => item.ProductId == productId)
            .Select(item => item.Quantity)
            .FirstOrDefault();

        if (actualQuantity < quantity)
        {
            this.SetToastInfo($"Количество товара «{product.ProductName}» скорректировано по остатку на складе.");
        }
        else
        {
            this.SetToastSuccess("Корзина обновлена.");
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Checkout(CartIndexViewModel postedModel)
    {
        var model = await BuildViewModelAsync(postedModel.SelectedSellerUserId);
        if (model.Items.Count == 0)
        {
            this.SetToastError("Корзина пуста. Добавьте товары перед оформлением заказа.");
            return RedirectToAction(nameof(Index));
        }

        if (!model.SelectedSellerUserId.HasValue)
        {
            ModelState.AddModelError(nameof(model.SelectedSellerUserId), "Выберите продавца для оформления заказа.");
        }
        else
        {
            var allowedSellerIds = model.Sellers.Select(item => item.Value).ToHashSet();
            if (!allowedSellerIds.Contains(model.SelectedSellerUserId.Value))
            {
                ModelState.AddModelError(nameof(model.SelectedSellerUserId), "Выбранный продавец недоступен.");
            }
        }

        var saleInputItems = model.Items.Select(item => new SaleItemInputViewModel
        {
            ProductId = item.ProductId,
            Quantity = item.Quantity,
            UnitPrice = item.Price
        }).ToList();

        var inventoryErrors = await inventoryService.ValidateSaleAsync(saleInputItems);
        foreach (var error in inventoryErrors)
        {
            ModelState.AddModelError(string.Empty, error);
        }

        if (model.Items.Any(item => !item.IsActive))
        {
            ModelState.AddModelError(string.Empty, "В корзине есть товары, снятые с продажи. Удалите их перед оформлением заказа.");
        }

        if (!ModelState.IsValid)
        {
            return View("Index", model);
        }

        var currentUserId = User.GetUserId();
        var customerId = currentUserId.HasValue
            ? await dbContext.Customers
                .Where(item => item.UserId == currentUserId.Value)
                .Select(item => (int?)item.CustomerId)
                .FirstOrDefaultAsync()
            : null;

        var sale = new Sale
        {
            SaleDate = DateTime.Now,
            CustomerId = customerId,
            SellerUserId = model.SelectedSellerUserId!.Value,
            TotalAmount = saleInputItems.Sum(item => item.Quantity * item.UnitPrice),
            Items = saleInputItems.Select(item => new SaleItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                LineTotal = item.Quantity * item.UnitPrice
            }).ToList()
        };

        await using var transaction = await dbContext.Database.BeginTransactionAsync();
        try
        {
            await inventoryService.ApplySaleAsync(saleInputItems);
            dbContext.Sales.Add(sale);
            await dbContext.SaveChangesAsync();
            await transaction.CommitAsync();
            cartService.Clear();
            this.SetToastSuccess($"Заказ оформлен. Номер продажи: {sale.SaleId}.");
            return RedirectToAction(nameof(Index));
        }
        catch (Exception exception)
        {
            await transaction.RollbackAsync();
            ModelState.AddModelError(string.Empty, exception.Message);
            return View("Index", model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Remove(int productId)
    {
        cartService.RemoveItem(productId);
        this.SetToastInfo("Товар удален из корзины.");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Clear()
    {
        cartService.Clear();
        this.SetToastInfo("Корзина очищена.");
        return RedirectToAction(nameof(Index));
    }

    private IActionResult RedirectToLocal(string? returnUrl)
    {
        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction("Index", "Catalog");
    }

    private async Task<CartIndexViewModel> BuildViewModelAsync(int? selectedSellerUserId = null)
    {
        var cartItems = cartService.GetItems();
        var productIds = cartItems.Select(item => item.ProductId).Distinct().ToList();
        var products = productIds.Count == 0
            ? new Dictionary<int, Product>()
            : await dbContext.Products
                .AsNoTracking()
                .Include(item => item.Category)
                .Where(item => productIds.Contains(item.ProductId))
                .ToDictionaryAsync(item => item.ProductId);

        foreach (var missingProductId in productIds.Where(productId => !products.ContainsKey(productId)))
        {
            cartService.RemoveItem(missingProductId);
        }

        var currentUserId = User.GetUserId();
        var customerDisplayName = currentUserId.HasValue
            ? await dbContext.Customers
                .Where(item => item.UserId == currentUserId.Value)
                .Select(item => item.FullName)
                .FirstOrDefaultAsync()
            : null;

        var sellers = await LoadSellerOptionsAsync();
        var selectedSeller = selectedSellerUserId;
        if (!selectedSeller.HasValue && sellers.Count > 0)
        {
            selectedSeller = sellers[0].Value;
        }

        return new CartIndexViewModel
        {
            SelectedSellerUserId = selectedSeller,
            Sellers = sellers,
            CustomerDisplayName = customerDisplayName,
            Items = cartItems
                .Where(item => products.ContainsKey(item.ProductId))
                .Select(item =>
                {
                    var product = products[item.ProductId];
                    return new CartLineViewModel
                    {
                        ProductId = product.ProductId,
                        ProductName = product.ProductName,
                        CategoryName = product.Category?.CategoryName ?? "Без категории",
                        Sku = product.Sku,
                        Price = product.Price,
                        Quantity = item.Quantity,
                        StockQuantity = product.StockQuantity,
                        IsActive = product.IsActive
                    };
                })
                .OrderBy(item => item.ProductName)
                .ToList()
        };
    }

    private async Task<List<SelectOptionViewModel>> LoadSellerOptionsAsync()
    {
        return await dbContext.Users
            .AsNoTracking()
            .Include(item => item.Role)
            .Where(item => item.IsActive && (item.Role!.RoleName == AppRoles.Seller || item.Role.RoleName == AppRoles.SystemAdministrator))
            .OrderBy(item => item.FullName)
            .Select(item => new SelectOptionViewModel
            {
                Value = item.UserId,
                Text = item.FullName
            })
            .ToListAsync();
    }
}
