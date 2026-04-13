using Microsoft.EntityFrameworkCore;
using VideoGameStoreSystem.Web.Infrastructure;
using VideoGameStoreSystem.Web.Models;
using VideoGameStoreSystem.Web.Permissions;

namespace VideoGameStoreSystem.Web.Data;

public class DbInitializer(ApplicationDbContext dbContext, IPasswordHashingService passwordHashingService)
{
    public async Task InitializeAsync()
    {
        await dbContext.Database.MigrateAsync();
        await SeedRolesAsync();
        await SeedUsersAndCustomersAsync();
        await SeedCatalogAsync();
        await SeedSuppliersAndSuppliesAsync();
        await SeedSalesAsync();
        await SeedWriteOffsAsync();
    }

    private async Task SeedRolesAsync()
    {
        if (await dbContext.Roles.AnyAsync())
        {
            return;
        }

        dbContext.Roles.AddRange(AppRoles.All.Select(roleName => new AppRole { RoleName = roleName }));
        await dbContext.SaveChangesAsync();
    }

    private async Task SeedUsersAndCustomersAsync()
    {
        if (!await dbContext.Users.AnyAsync())
        {
            var roles = await dbContext.Roles.ToDictionaryAsync(item => item.RoleName, item => item.RoleId);
            dbContext.Users.AddRange(
                new AppUser { Login = "admin", FullName = "Алексей Смирнов", Email = "admin@game-store.local", RoleId = roles[AppRoles.SystemAdministrator], PasswordHash = passwordHashingService.HashPassword("Admin123!"), IsActive = true },
                new AppUser { Login = "owner", FullName = "Марина Орлова", Email = "owner@game-store.local", RoleId = roles[AppRoles.Owner], PasswordHash = passwordHashingService.HashPassword("Owner123!"), IsActive = true },
                new AppUser { Login = "manager", FullName = "Никита Воронов", Email = "manager@game-store.local", RoleId = roles[AppRoles.Manager], PasswordHash = passwordHashingService.HashPassword("Manager123!"), IsActive = true },
                new AppUser { Login = "seller", FullName = "Екатерина Белова", Email = "seller@game-store.local", RoleId = roles[AppRoles.Seller], PasswordHash = passwordHashingService.HashPassword("Seller123!"), IsActive = true },
                new AppUser { Login = "accountant", FullName = "Ольга Кравцова", Email = "accountant@game-store.local", RoleId = roles[AppRoles.Accountant], PasswordHash = passwordHashingService.HashPassword("Account123!"), IsActive = true },
                new AppUser { Login = "warehouse", FullName = "Сергей Данилов", Email = "warehouse@game-store.local", RoleId = roles[AppRoles.Warehouseman], PasswordHash = passwordHashingService.HashPassword("Warehouse123!"), IsActive = true },
                new AppUser { Login = "client", FullName = "Иван Покупатель", Email = "client@game-store.local", RoleId = roles[AppRoles.Client], PasswordHash = passwordHashingService.HashPassword("Client123!"), IsActive = true });

            await dbContext.SaveChangesAsync();
        }

        if (await dbContext.Customers.AnyAsync())
        {
            return;
        }

        var clientUserId = await dbContext.Users.Where(item => item.Login == "client").Select(item => item.UserId).FirstAsync();
        dbContext.Customers.AddRange(
            new Customer { FullName = "Иван Покупатель", Phone = "+7 (999) 123-45-67", Email = "client@game-store.local", UserId = clientUserId },
            new Customer { FullName = "Анна Соколова", Phone = "+7 (912) 000-11-22", Email = "anna.sokolova@example.com" },
            new Customer { FullName = "Дмитрий Логинов", Phone = "+7 (921) 777-10-10", Email = "d.loginov@example.com" },
            new Customer { FullName = "Полина Миронова", Phone = "+7 (904) 222-44-55", Email = "polina@example.com" });

        await dbContext.SaveChangesAsync();
    }

    private async Task SeedCatalogAsync()
    {
        if (!await dbContext.Categories.AnyAsync())
        {
            dbContext.Categories.AddRange(
                new Category { CategoryName = "Видеоигры" },
                new Category { CategoryName = "Консоли" },
                new Category { CategoryName = "Аксессуары" },
                new Category { CategoryName = "Подарочные карты" },
                new Category { CategoryName = "Коллекционные издания" });

            await dbContext.SaveChangesAsync();
        }

        if (await dbContext.Products.AnyAsync())
        {
            return;
        }

        var categories = await dbContext.Categories.ToDictionaryAsync(item => item.CategoryName, item => item.CategoryId);
        dbContext.Products.AddRange(
            new Product { ProductName = "EA Sports FC 25", CategoryId = categories["Видеоигры"], Sku = "GAME-001", Description = "Футбольный симулятор для PlayStation 5.", Price = 4990m, CostPrice = 3600m, IsActive = true },
            new Product { ProductName = "The Witcher 3 Complete Edition", CategoryId = categories["Видеоигры"], Sku = "GAME-002", Description = "Полное издание культовой RPG.", Price = 2590m, CostPrice = 1700m, IsActive = true },
            new Product { ProductName = "Cyberpunk 2077 Ultimate", CategoryId = categories["Видеоигры"], Sku = "GAME-003", Description = "Футуристическая RPG с дополнением Phantom Liberty.", Price = 4290m, CostPrice = 3000m, IsActive = true },
            new Product { ProductName = "Marvel's Spider-Man 2", CategoryId = categories["Видеоигры"], Sku = "GAME-004", Description = "Экшен о приключениях двух Человеков-пауков.", Price = 5390m, CostPrice = 3900m, IsActive = true },
            new Product { ProductName = "PlayStation 5 Slim", CategoryId = categories["Консоли"], Sku = "CONS-001", Description = "Современная игровая консоль Sony.", Price = 59990m, CostPrice = 52000m, IsActive = true },
            new Product { ProductName = "Xbox Series X", CategoryId = categories["Консоли"], Sku = "CONS-002", Description = "Флагманская консоль Microsoft.", Price = 57990m, CostPrice = 50000m, IsActive = true },
            new Product { ProductName = "Nintendo Switch OLED", CategoryId = categories["Консоли"], Sku = "CONS-003", Description = "Портативная консоль Nintendo с OLED-экраном.", Price = 34990m, CostPrice = 30000m, IsActive = true },
            new Product { ProductName = "Геймпад DualSense", CategoryId = categories["Аксессуары"], Sku = "ACC-001", Description = "Беспроводной контроллер для PlayStation 5.", Price = 8990m, CostPrice = 6900m, IsActive = true },
            new Product { ProductName = "Геймпад Xbox Wireless Controller", CategoryId = categories["Аксессуары"], Sku = "ACC-002", Description = "Оригинальный контроллер Xbox.", Price = 7590m, CostPrice = 5800m, IsActive = true },
            new Product { ProductName = "Гарнитура HyperX Cloud Alpha", CategoryId = categories["Аксессуары"], Sku = "ACC-003", Description = "Игровая гарнитура для консолей и ПК.", Price = 10990m, CostPrice = 8400m, IsActive = true },
            new Product { ProductName = "Подарочная карта PlayStation Store 2500", CategoryId = categories["Подарочные карты"], Sku = "CARD-001", Description = "Цифровая карта пополнения PS Store.", Price = 2500m, CostPrice = 2500m, IsActive = true },
            new Product { ProductName = "Коллекционное издание Elden Ring", CategoryId = categories["Коллекционные издания"], Sku = "COLL-001", Description = "Коллекционный набор для поклонников Elden Ring.", Price = 14990m, CostPrice = 11800m, IsActive = true });

        await dbContext.SaveChangesAsync();
    }

    private async Task SeedSuppliersAndSuppliesAsync()
    {
        if (!await dbContext.Suppliers.AnyAsync())
        {
            dbContext.Suppliers.AddRange(
                new Supplier { SupplierName = "ООО Гейм Дистрибуция", Phone = "+7 (495) 700-11-22", Email = "sales@gamedistr.ru" },
                new Supplier { SupplierName = "ИгроМаркет Опт", Phone = "+7 (812) 900-55-66", Email = "opt@igromarket.ru" },
                new Supplier { SupplierName = "Консольный мир", Phone = "+7 (343) 400-88-99", Email = "supply@consoleworld.ru" });

            await dbContext.SaveChangesAsync();
        }

        if (await dbContext.Supplies.AnyAsync())
        {
            return;
        }

        var suppliers = await dbContext.Suppliers.ToDictionaryAsync(item => item.SupplierName, item => item.SupplierId);
        var products = await dbContext.Products.ToDictionaryAsync(item => item.Sku, item => item);

        var supplies = new[]
        {
            new Supply
            {
                SupplyDate = DateTime.Today.AddDays(-18),
                SupplierId = suppliers["ООО Гейм Дистрибуция"],
                Items =
                [
                    new SupplyItem { ProductId = products["GAME-001"].ProductId, Quantity = 8, UnitCost = 3600m },
                    new SupplyItem { ProductId = products["GAME-004"].ProductId, Quantity = 6, UnitCost = 3900m },
                    new SupplyItem { ProductId = products["ACC-001"].ProductId, Quantity = 10, UnitCost = 6900m }
                ]
            },
            new Supply
            {
                SupplyDate = DateTime.Today.AddDays(-11),
                SupplierId = suppliers["Консольный мир"],
                Items =
                [
                    new SupplyItem { ProductId = products["CONS-001"].ProductId, Quantity = 4, UnitCost = 52000m },
                    new SupplyItem { ProductId = products["CONS-002"].ProductId, Quantity = 3, UnitCost = 50000m },
                    new SupplyItem { ProductId = products["CONS-003"].ProductId, Quantity = 5, UnitCost = 30000m }
                ]
            },
            new Supply
            {
                SupplyDate = DateTime.Today.AddDays(-7),
                SupplierId = suppliers["ИгроМаркет Опт"],
                Items =
                [
                    new SupplyItem { ProductId = products["GAME-002"].ProductId, Quantity = 7, UnitCost = 1700m },
                    new SupplyItem { ProductId = products["GAME-003"].ProductId, Quantity = 5, UnitCost = 3000m },
                    new SupplyItem { ProductId = products["ACC-003"].ProductId, Quantity = 6, UnitCost = 8400m },
                    new SupplyItem { ProductId = products["COLL-001"].ProductId, Quantity = 2, UnitCost = 11800m }
                ]
            }
        };

        foreach (var supply in supplies)
        {
            supply.TotalAmount = supply.Items.Sum(item => item.Quantity * item.UnitCost);
            foreach (var item in supply.Items)
            {
                products.Single(product => product.Value.ProductId == item.ProductId).Value.StockQuantity += item.Quantity;
            }

            dbContext.Supplies.Add(supply);
        }

        await dbContext.SaveChangesAsync();
    }

    private async Task SeedSalesAsync()
    {
        if (await dbContext.Sales.AnyAsync())
        {
            return;
        }

        var customers = await dbContext.Customers.ToDictionaryAsync(item => item.FullName, item => item.CustomerId);
        var users = await dbContext.Users.ToDictionaryAsync(item => item.Login, item => item.UserId);
        var products = await dbContext.Products.ToDictionaryAsync(item => item.Sku, item => item);

        var sales = new[]
        {
            new Sale
            {
                SaleDate = DateTime.Today.AddDays(-5),
                CustomerId = customers["Анна Соколова"],
                SellerUserId = users["seller"],
                Items =
                [
                    new SaleItem { ProductId = products["GAME-001"].ProductId, Quantity = 1, UnitPrice = 4990m, LineTotal = 4990m },
                    new SaleItem { ProductId = products["ACC-001"].ProductId, Quantity = 1, UnitPrice = 8990m, LineTotal = 8990m }
                ]
            },
            new Sale
            {
                SaleDate = DateTime.Today.AddDays(-3),
                CustomerId = customers["Дмитрий Логинов"],
                SellerUserId = users["seller"],
                Items =
                [
                    new SaleItem { ProductId = products["CONS-003"].ProductId, Quantity = 1, UnitPrice = 34990m, LineTotal = 34990m },
                    new SaleItem { ProductId = products["GAME-002"].ProductId, Quantity = 1, UnitPrice = 2590m, LineTotal = 2590m }
                ]
            },
            new Sale
            {
                SaleDate = DateTime.Today.AddDays(-1),
                CustomerId = customers["Иван Покупатель"],
                SellerUserId = users["seller"],
                Items =
                [
                    new SaleItem { ProductId = products["GAME-003"].ProductId, Quantity = 1, UnitPrice = 4290m, LineTotal = 4290m },
                    new SaleItem { ProductId = products["GAME-004"].ProductId, Quantity = 1, UnitPrice = 5390m, LineTotal = 5390m }
                ]
            }
        };

        foreach (var sale in sales)
        {
            sale.TotalAmount = sale.Items.Sum(item => item.LineTotal);
            foreach (var item in sale.Items)
            {
                products.Single(product => product.Value.ProductId == item.ProductId).Value.StockQuantity -= item.Quantity;
            }

            dbContext.Sales.Add(sale);
        }

        await dbContext.SaveChangesAsync();
    }

    private async Task SeedWriteOffsAsync()
    {
        if (await dbContext.WriteOffs.AnyAsync())
        {
            return;
        }

        var products = await dbContext.Products.ToDictionaryAsync(item => item.Sku, item => item);
        var warehouseUserId = await dbContext.Users
            .Where(item => item.Login == "warehouse")
            .Select(item => item.UserId)
            .FirstAsync();

        var writeOff = new WriteOff
        {
            WriteOffDate = DateTime.Today.AddDays(-2),
            Reason = "Повреждение упаковки при транспортировке",
            CreatedByUserId = warehouseUserId,
            Items =
            [
                new WriteOffItem { ProductId = products["ACC-003"].ProductId, Quantity = 1 },
                new WriteOffItem { ProductId = products["GAME-001"].ProductId, Quantity = 1 }
            ]
        };

        foreach (var item in writeOff.Items)
        {
            products.Single(product => product.Value.ProductId == item.ProductId).Value.StockQuantity -= item.Quantity;
        }

        dbContext.WriteOffs.Add(writeOff);
        await dbContext.SaveChangesAsync();
    }
}
