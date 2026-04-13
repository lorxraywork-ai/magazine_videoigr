namespace VideoGameStoreSystem.Web.Permissions;

public static class AppRoles
{
    public const string SystemAdministrator = "Системный администратор";
    public const string Owner = "Владелец";
    public const string Manager = "Управляющий";
    public const string Seller = "Продавец-консультант";
    public const string Accountant = "Бухгалтер";
    public const string Warehouseman = "Кладовщик";
    public const string Client = "Клиент";

    public static IReadOnlyList<string> All { get; } =
    [
        SystemAdministrator,
        Owner,
        Manager,
        Seller,
        Accountant,
        Warehouseman,
        Client
    ];
}
