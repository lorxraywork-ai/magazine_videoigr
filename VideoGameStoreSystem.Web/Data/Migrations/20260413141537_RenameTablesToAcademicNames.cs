using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VideoGameStoreSystem.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenameTablesToAcademicNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CHANGE_LOGS_USERS_CHANGED_BY_USER_ID",
                table: "CHANGE_LOGS");

            migrationBuilder.DropForeignKey(
                name: "FK_CUSTOMERS_USERS_USER_ID",
                table: "CUSTOMERS");

            migrationBuilder.DropForeignKey(
                name: "FK_PRODUCTS_CATEGORIES_CATEGORY_ID",
                table: "PRODUCTS");

            migrationBuilder.DropForeignKey(
                name: "FK_SALE_ITEMS_PRODUCTS_PRODUCT_ID",
                table: "SALE_ITEMS");

            migrationBuilder.DropForeignKey(
                name: "FK_SALE_ITEMS_SALES_SALE_ID",
                table: "SALE_ITEMS");

            migrationBuilder.DropForeignKey(
                name: "FK_SALES_CUSTOMERS_CUSTOMER_ID",
                table: "SALES");

            migrationBuilder.DropForeignKey(
                name: "FK_SALES_USERS_SELLER_USER_ID",
                table: "SALES");

            migrationBuilder.DropForeignKey(
                name: "FK_SUPPLIES_SUPPLIERS_SUPPLIER_ID",
                table: "SUPPLIES");

            migrationBuilder.DropForeignKey(
                name: "FK_SUPPLY_ITEMS_PRODUCTS_PRODUCT_ID",
                table: "SUPPLY_ITEMS");

            migrationBuilder.DropForeignKey(
                name: "FK_SUPPLY_ITEMS_SUPPLIES_SUPPLY_ID",
                table: "SUPPLY_ITEMS");

            migrationBuilder.DropForeignKey(
                name: "FK_USERS_ROLES_ROLE_ID",
                table: "USERS");

            migrationBuilder.DropForeignKey(
                name: "FK_WRITE_OFF_ITEMS_PRODUCTS_PRODUCT_ID",
                table: "WRITE_OFF_ITEMS");

            migrationBuilder.DropForeignKey(
                name: "FK_WRITE_OFF_ITEMS_WRITE_OFFS_WRITE_OFF_ID",
                table: "WRITE_OFF_ITEMS");

            migrationBuilder.DropForeignKey(
                name: "FK_WRITE_OFFS_USERS_CREATED_BY_USER_ID",
                table: "WRITE_OFFS");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WRITE_OFFS",
                table: "WRITE_OFFS");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WRITE_OFF_ITEMS",
                table: "WRITE_OFF_ITEMS");

            migrationBuilder.DropPrimaryKey(
                name: "PK_USERS",
                table: "USERS");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SUPPLY_ITEMS",
                table: "SUPPLY_ITEMS");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SUPPLIES",
                table: "SUPPLIES");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SUPPLIERS",
                table: "SUPPLIERS");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SALES",
                table: "SALES");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SALE_ITEMS",
                table: "SALE_ITEMS");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ROLES",
                table: "ROLES");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PRODUCTS",
                table: "PRODUCTS");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CUSTOMERS",
                table: "CUSTOMERS");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CHANGE_LOGS",
                table: "CHANGE_LOGS");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CATEGORIES",
                table: "CATEGORIES");

            migrationBuilder.RenameTable(
                name: "WRITE_OFFS",
                newName: "WRITE_OFF");

            migrationBuilder.RenameTable(
                name: "WRITE_OFF_ITEMS",
                newName: "WRITE_OFF_ITEM");

            migrationBuilder.RenameTable(
                name: "USERS",
                newName: "APP_USER");

            migrationBuilder.RenameTable(
                name: "SUPPLY_ITEMS",
                newName: "SUPPLY_ITEM");

            migrationBuilder.RenameTable(
                name: "SUPPLIES",
                newName: "SUPPLY");

            migrationBuilder.RenameTable(
                name: "SUPPLIERS",
                newName: "SUPPLIER");

            migrationBuilder.RenameTable(
                name: "SALES",
                newName: "SALE");

            migrationBuilder.RenameTable(
                name: "SALE_ITEMS",
                newName: "SALE_ITEM");

            migrationBuilder.RenameTable(
                name: "ROLES",
                newName: "APP_ROLE");

            migrationBuilder.RenameTable(
                name: "PRODUCTS",
                newName: "PRODUCT");

            migrationBuilder.RenameTable(
                name: "CUSTOMERS",
                newName: "CUSTOMER");

            migrationBuilder.RenameTable(
                name: "CHANGE_LOGS",
                newName: "APP_CHANGE_LOG");

            migrationBuilder.RenameTable(
                name: "CATEGORIES",
                newName: "CATEGORY");

            migrationBuilder.RenameIndex(
                name: "IX_WRITE_OFFS_WRITE_OFF_DATE",
                table: "WRITE_OFF",
                newName: "IX_WRITE_OFF_WRITE_OFF_DATE");

            migrationBuilder.RenameIndex(
                name: "IX_WRITE_OFFS_CREATED_BY_USER_ID",
                table: "WRITE_OFF",
                newName: "IX_WRITE_OFF_CREATED_BY_USER_ID");

            migrationBuilder.RenameIndex(
                name: "IX_WRITE_OFF_ITEMS_WRITE_OFF_ID",
                table: "WRITE_OFF_ITEM",
                newName: "IX_WRITE_OFF_ITEM_WRITE_OFF_ID");

            migrationBuilder.RenameIndex(
                name: "IX_WRITE_OFF_ITEMS_PRODUCT_ID",
                table: "WRITE_OFF_ITEM",
                newName: "IX_WRITE_OFF_ITEM_PRODUCT_ID");

            migrationBuilder.RenameIndex(
                name: "IX_USERS_ROLE_ID",
                table: "APP_USER",
                newName: "IX_APP_USER_ROLE_ID");

            migrationBuilder.RenameIndex(
                name: "IX_USERS_LOGIN",
                table: "APP_USER",
                newName: "IX_APP_USER_LOGIN");

            migrationBuilder.RenameIndex(
                name: "IX_USERS_EMAIL",
                table: "APP_USER",
                newName: "IX_APP_USER_EMAIL");

            migrationBuilder.RenameIndex(
                name: "IX_SUPPLY_ITEMS_SUPPLY_ID",
                table: "SUPPLY_ITEM",
                newName: "IX_SUPPLY_ITEM_SUPPLY_ID");

            migrationBuilder.RenameIndex(
                name: "IX_SUPPLY_ITEMS_PRODUCT_ID",
                table: "SUPPLY_ITEM",
                newName: "IX_SUPPLY_ITEM_PRODUCT_ID");

            migrationBuilder.RenameIndex(
                name: "IX_SUPPLIES_SUPPLY_DATE",
                table: "SUPPLY",
                newName: "IX_SUPPLY_SUPPLY_DATE");

            migrationBuilder.RenameIndex(
                name: "IX_SUPPLIES_SUPPLIER_ID",
                table: "SUPPLY",
                newName: "IX_SUPPLY_SUPPLIER_ID");

            migrationBuilder.RenameIndex(
                name: "IX_SUPPLIERS_SUPPLIER_NAME",
                table: "SUPPLIER",
                newName: "IX_SUPPLIER_SUPPLIER_NAME");

            migrationBuilder.RenameIndex(
                name: "IX_SALES_SELLER_USER_ID",
                table: "SALE",
                newName: "IX_SALE_SELLER_USER_ID");

            migrationBuilder.RenameIndex(
                name: "IX_SALES_SALE_DATE",
                table: "SALE",
                newName: "IX_SALE_SALE_DATE");

            migrationBuilder.RenameIndex(
                name: "IX_SALES_CUSTOMER_ID",
                table: "SALE",
                newName: "IX_SALE_CUSTOMER_ID");

            migrationBuilder.RenameIndex(
                name: "IX_SALE_ITEMS_SALE_ID",
                table: "SALE_ITEM",
                newName: "IX_SALE_ITEM_SALE_ID");

            migrationBuilder.RenameIndex(
                name: "IX_SALE_ITEMS_PRODUCT_ID",
                table: "SALE_ITEM",
                newName: "IX_SALE_ITEM_PRODUCT_ID");

            migrationBuilder.RenameIndex(
                name: "IX_ROLES_ROLE_NAME",
                table: "APP_ROLE",
                newName: "IX_APP_ROLE_ROLE_NAME");

            migrationBuilder.RenameIndex(
                name: "IX_PRODUCTS_SKU",
                table: "PRODUCT",
                newName: "IX_PRODUCT_SKU");

            migrationBuilder.RenameIndex(
                name: "IX_PRODUCTS_PRODUCT_NAME",
                table: "PRODUCT",
                newName: "IX_PRODUCT_PRODUCT_NAME");

            migrationBuilder.RenameIndex(
                name: "IX_PRODUCTS_CATEGORY_ID",
                table: "PRODUCT",
                newName: "IX_PRODUCT_CATEGORY_ID");

            migrationBuilder.RenameIndex(
                name: "IX_CUSTOMERS_USER_ID",
                table: "CUSTOMER",
                newName: "IX_CUSTOMER_USER_ID");

            migrationBuilder.RenameIndex(
                name: "IX_CUSTOMERS_FULL_NAME",
                table: "CUSTOMER",
                newName: "IX_CUSTOMER_FULL_NAME");

            migrationBuilder.RenameIndex(
                name: "IX_CUSTOMERS_EMAIL",
                table: "CUSTOMER",
                newName: "IX_CUSTOMER_EMAIL");

            migrationBuilder.RenameIndex(
                name: "IX_CHANGE_LOGS_CHANGED_BY_USER_ID",
                table: "APP_CHANGE_LOG",
                newName: "IX_APP_CHANGE_LOG_CHANGED_BY_USER_ID");

            migrationBuilder.RenameIndex(
                name: "IX_CHANGE_LOGS_CHANGED_AT",
                table: "APP_CHANGE_LOG",
                newName: "IX_APP_CHANGE_LOG_CHANGED_AT");

            migrationBuilder.RenameIndex(
                name: "IX_CATEGORIES_CATEGORY_NAME",
                table: "CATEGORY",
                newName: "IX_CATEGORY_CATEGORY_NAME");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WRITE_OFF",
                table: "WRITE_OFF",
                column: "WRITE_OFF_ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WRITE_OFF_ITEM",
                table: "WRITE_OFF_ITEM",
                column: "WRITE_OFF_ITEM_ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_APP_USER",
                table: "APP_USER",
                column: "USER_ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SUPPLY_ITEM",
                table: "SUPPLY_ITEM",
                column: "SUPPLY_ITEM_ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SUPPLY",
                table: "SUPPLY",
                column: "SUPPLY_ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SUPPLIER",
                table: "SUPPLIER",
                column: "SUPPLIER_ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SALE",
                table: "SALE",
                column: "SALE_ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SALE_ITEM",
                table: "SALE_ITEM",
                column: "SALE_ITEM_ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_APP_ROLE",
                table: "APP_ROLE",
                column: "ROLE_ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PRODUCT",
                table: "PRODUCT",
                column: "PRODUCT_ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CUSTOMER",
                table: "CUSTOMER",
                column: "CUSTOMER_ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_APP_CHANGE_LOG",
                table: "APP_CHANGE_LOG",
                column: "CHANGE_LOG_ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CATEGORY",
                table: "CATEGORY",
                column: "CATEGORY_ID");

            migrationBuilder.AddForeignKey(
                name: "FK_APP_CHANGE_LOG_APP_USER_CHANGED_BY_USER_ID",
                table: "APP_CHANGE_LOG",
                column: "CHANGED_BY_USER_ID",
                principalTable: "APP_USER",
                principalColumn: "USER_ID",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_APP_USER_APP_ROLE_ROLE_ID",
                table: "APP_USER",
                column: "ROLE_ID",
                principalTable: "APP_ROLE",
                principalColumn: "ROLE_ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CUSTOMER_APP_USER_USER_ID",
                table: "CUSTOMER",
                column: "USER_ID",
                principalTable: "APP_USER",
                principalColumn: "USER_ID",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_PRODUCT_CATEGORY_CATEGORY_ID",
                table: "PRODUCT",
                column: "CATEGORY_ID",
                principalTable: "CATEGORY",
                principalColumn: "CATEGORY_ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SALE_APP_USER_SELLER_USER_ID",
                table: "SALE",
                column: "SELLER_USER_ID",
                principalTable: "APP_USER",
                principalColumn: "USER_ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SALE_CUSTOMER_CUSTOMER_ID",
                table: "SALE",
                column: "CUSTOMER_ID",
                principalTable: "CUSTOMER",
                principalColumn: "CUSTOMER_ID",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_SALE_ITEM_PRODUCT_PRODUCT_ID",
                table: "SALE_ITEM",
                column: "PRODUCT_ID",
                principalTable: "PRODUCT",
                principalColumn: "PRODUCT_ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SALE_ITEM_SALE_SALE_ID",
                table: "SALE_ITEM",
                column: "SALE_ID",
                principalTable: "SALE",
                principalColumn: "SALE_ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SUPPLY_SUPPLIER_SUPPLIER_ID",
                table: "SUPPLY",
                column: "SUPPLIER_ID",
                principalTable: "SUPPLIER",
                principalColumn: "SUPPLIER_ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SUPPLY_ITEM_PRODUCT_PRODUCT_ID",
                table: "SUPPLY_ITEM",
                column: "PRODUCT_ID",
                principalTable: "PRODUCT",
                principalColumn: "PRODUCT_ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SUPPLY_ITEM_SUPPLY_SUPPLY_ID",
                table: "SUPPLY_ITEM",
                column: "SUPPLY_ID",
                principalTable: "SUPPLY",
                principalColumn: "SUPPLY_ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WRITE_OFF_APP_USER_CREATED_BY_USER_ID",
                table: "WRITE_OFF",
                column: "CREATED_BY_USER_ID",
                principalTable: "APP_USER",
                principalColumn: "USER_ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WRITE_OFF_ITEM_PRODUCT_PRODUCT_ID",
                table: "WRITE_OFF_ITEM",
                column: "PRODUCT_ID",
                principalTable: "PRODUCT",
                principalColumn: "PRODUCT_ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WRITE_OFF_ITEM_WRITE_OFF_WRITE_OFF_ID",
                table: "WRITE_OFF_ITEM",
                column: "WRITE_OFF_ID",
                principalTable: "WRITE_OFF",
                principalColumn: "WRITE_OFF_ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_APP_CHANGE_LOG_APP_USER_CHANGED_BY_USER_ID",
                table: "APP_CHANGE_LOG");

            migrationBuilder.DropForeignKey(
                name: "FK_APP_USER_APP_ROLE_ROLE_ID",
                table: "APP_USER");

            migrationBuilder.DropForeignKey(
                name: "FK_CUSTOMER_APP_USER_USER_ID",
                table: "CUSTOMER");

            migrationBuilder.DropForeignKey(
                name: "FK_PRODUCT_CATEGORY_CATEGORY_ID",
                table: "PRODUCT");

            migrationBuilder.DropForeignKey(
                name: "FK_SALE_APP_USER_SELLER_USER_ID",
                table: "SALE");

            migrationBuilder.DropForeignKey(
                name: "FK_SALE_CUSTOMER_CUSTOMER_ID",
                table: "SALE");

            migrationBuilder.DropForeignKey(
                name: "FK_SALE_ITEM_PRODUCT_PRODUCT_ID",
                table: "SALE_ITEM");

            migrationBuilder.DropForeignKey(
                name: "FK_SALE_ITEM_SALE_SALE_ID",
                table: "SALE_ITEM");

            migrationBuilder.DropForeignKey(
                name: "FK_SUPPLY_SUPPLIER_SUPPLIER_ID",
                table: "SUPPLY");

            migrationBuilder.DropForeignKey(
                name: "FK_SUPPLY_ITEM_PRODUCT_PRODUCT_ID",
                table: "SUPPLY_ITEM");

            migrationBuilder.DropForeignKey(
                name: "FK_SUPPLY_ITEM_SUPPLY_SUPPLY_ID",
                table: "SUPPLY_ITEM");

            migrationBuilder.DropForeignKey(
                name: "FK_WRITE_OFF_APP_USER_CREATED_BY_USER_ID",
                table: "WRITE_OFF");

            migrationBuilder.DropForeignKey(
                name: "FK_WRITE_OFF_ITEM_PRODUCT_PRODUCT_ID",
                table: "WRITE_OFF_ITEM");

            migrationBuilder.DropForeignKey(
                name: "FK_WRITE_OFF_ITEM_WRITE_OFF_WRITE_OFF_ID",
                table: "WRITE_OFF_ITEM");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WRITE_OFF_ITEM",
                table: "WRITE_OFF_ITEM");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WRITE_OFF",
                table: "WRITE_OFF");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SUPPLY_ITEM",
                table: "SUPPLY_ITEM");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SUPPLY",
                table: "SUPPLY");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SUPPLIER",
                table: "SUPPLIER");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SALE_ITEM",
                table: "SALE_ITEM");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SALE",
                table: "SALE");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PRODUCT",
                table: "PRODUCT");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CUSTOMER",
                table: "CUSTOMER");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CATEGORY",
                table: "CATEGORY");

            migrationBuilder.DropPrimaryKey(
                name: "PK_APP_USER",
                table: "APP_USER");

            migrationBuilder.DropPrimaryKey(
                name: "PK_APP_ROLE",
                table: "APP_ROLE");

            migrationBuilder.DropPrimaryKey(
                name: "PK_APP_CHANGE_LOG",
                table: "APP_CHANGE_LOG");

            migrationBuilder.RenameTable(
                name: "WRITE_OFF_ITEM",
                newName: "WRITE_OFF_ITEMS");

            migrationBuilder.RenameTable(
                name: "WRITE_OFF",
                newName: "WRITE_OFFS");

            migrationBuilder.RenameTable(
                name: "SUPPLY_ITEM",
                newName: "SUPPLY_ITEMS");

            migrationBuilder.RenameTable(
                name: "SUPPLY",
                newName: "SUPPLIES");

            migrationBuilder.RenameTable(
                name: "SUPPLIER",
                newName: "SUPPLIERS");

            migrationBuilder.RenameTable(
                name: "SALE_ITEM",
                newName: "SALE_ITEMS");

            migrationBuilder.RenameTable(
                name: "SALE",
                newName: "SALES");

            migrationBuilder.RenameTable(
                name: "PRODUCT",
                newName: "PRODUCTS");

            migrationBuilder.RenameTable(
                name: "CUSTOMER",
                newName: "CUSTOMERS");

            migrationBuilder.RenameTable(
                name: "CATEGORY",
                newName: "CATEGORIES");

            migrationBuilder.RenameTable(
                name: "APP_USER",
                newName: "USERS");

            migrationBuilder.RenameTable(
                name: "APP_ROLE",
                newName: "ROLES");

            migrationBuilder.RenameTable(
                name: "APP_CHANGE_LOG",
                newName: "CHANGE_LOGS");

            migrationBuilder.RenameIndex(
                name: "IX_WRITE_OFF_ITEM_WRITE_OFF_ID",
                table: "WRITE_OFF_ITEMS",
                newName: "IX_WRITE_OFF_ITEMS_WRITE_OFF_ID");

            migrationBuilder.RenameIndex(
                name: "IX_WRITE_OFF_ITEM_PRODUCT_ID",
                table: "WRITE_OFF_ITEMS",
                newName: "IX_WRITE_OFF_ITEMS_PRODUCT_ID");

            migrationBuilder.RenameIndex(
                name: "IX_WRITE_OFF_WRITE_OFF_DATE",
                table: "WRITE_OFFS",
                newName: "IX_WRITE_OFFS_WRITE_OFF_DATE");

            migrationBuilder.RenameIndex(
                name: "IX_WRITE_OFF_CREATED_BY_USER_ID",
                table: "WRITE_OFFS",
                newName: "IX_WRITE_OFFS_CREATED_BY_USER_ID");

            migrationBuilder.RenameIndex(
                name: "IX_SUPPLY_ITEM_SUPPLY_ID",
                table: "SUPPLY_ITEMS",
                newName: "IX_SUPPLY_ITEMS_SUPPLY_ID");

            migrationBuilder.RenameIndex(
                name: "IX_SUPPLY_ITEM_PRODUCT_ID",
                table: "SUPPLY_ITEMS",
                newName: "IX_SUPPLY_ITEMS_PRODUCT_ID");

            migrationBuilder.RenameIndex(
                name: "IX_SUPPLY_SUPPLY_DATE",
                table: "SUPPLIES",
                newName: "IX_SUPPLIES_SUPPLY_DATE");

            migrationBuilder.RenameIndex(
                name: "IX_SUPPLY_SUPPLIER_ID",
                table: "SUPPLIES",
                newName: "IX_SUPPLIES_SUPPLIER_ID");

            migrationBuilder.RenameIndex(
                name: "IX_SUPPLIER_SUPPLIER_NAME",
                table: "SUPPLIERS",
                newName: "IX_SUPPLIERS_SUPPLIER_NAME");

            migrationBuilder.RenameIndex(
                name: "IX_SALE_ITEM_SALE_ID",
                table: "SALE_ITEMS",
                newName: "IX_SALE_ITEMS_SALE_ID");

            migrationBuilder.RenameIndex(
                name: "IX_SALE_ITEM_PRODUCT_ID",
                table: "SALE_ITEMS",
                newName: "IX_SALE_ITEMS_PRODUCT_ID");

            migrationBuilder.RenameIndex(
                name: "IX_SALE_SELLER_USER_ID",
                table: "SALES",
                newName: "IX_SALES_SELLER_USER_ID");

            migrationBuilder.RenameIndex(
                name: "IX_SALE_SALE_DATE",
                table: "SALES",
                newName: "IX_SALES_SALE_DATE");

            migrationBuilder.RenameIndex(
                name: "IX_SALE_CUSTOMER_ID",
                table: "SALES",
                newName: "IX_SALES_CUSTOMER_ID");

            migrationBuilder.RenameIndex(
                name: "IX_PRODUCT_SKU",
                table: "PRODUCTS",
                newName: "IX_PRODUCTS_SKU");

            migrationBuilder.RenameIndex(
                name: "IX_PRODUCT_PRODUCT_NAME",
                table: "PRODUCTS",
                newName: "IX_PRODUCTS_PRODUCT_NAME");

            migrationBuilder.RenameIndex(
                name: "IX_PRODUCT_CATEGORY_ID",
                table: "PRODUCTS",
                newName: "IX_PRODUCTS_CATEGORY_ID");

            migrationBuilder.RenameIndex(
                name: "IX_CUSTOMER_USER_ID",
                table: "CUSTOMERS",
                newName: "IX_CUSTOMERS_USER_ID");

            migrationBuilder.RenameIndex(
                name: "IX_CUSTOMER_FULL_NAME",
                table: "CUSTOMERS",
                newName: "IX_CUSTOMERS_FULL_NAME");

            migrationBuilder.RenameIndex(
                name: "IX_CUSTOMER_EMAIL",
                table: "CUSTOMERS",
                newName: "IX_CUSTOMERS_EMAIL");

            migrationBuilder.RenameIndex(
                name: "IX_CATEGORY_CATEGORY_NAME",
                table: "CATEGORIES",
                newName: "IX_CATEGORIES_CATEGORY_NAME");

            migrationBuilder.RenameIndex(
                name: "IX_APP_USER_ROLE_ID",
                table: "USERS",
                newName: "IX_USERS_ROLE_ID");

            migrationBuilder.RenameIndex(
                name: "IX_APP_USER_LOGIN",
                table: "USERS",
                newName: "IX_USERS_LOGIN");

            migrationBuilder.RenameIndex(
                name: "IX_APP_USER_EMAIL",
                table: "USERS",
                newName: "IX_USERS_EMAIL");

            migrationBuilder.RenameIndex(
                name: "IX_APP_ROLE_ROLE_NAME",
                table: "ROLES",
                newName: "IX_ROLES_ROLE_NAME");

            migrationBuilder.RenameIndex(
                name: "IX_APP_CHANGE_LOG_CHANGED_BY_USER_ID",
                table: "CHANGE_LOGS",
                newName: "IX_CHANGE_LOGS_CHANGED_BY_USER_ID");

            migrationBuilder.RenameIndex(
                name: "IX_APP_CHANGE_LOG_CHANGED_AT",
                table: "CHANGE_LOGS",
                newName: "IX_CHANGE_LOGS_CHANGED_AT");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WRITE_OFF_ITEMS",
                table: "WRITE_OFF_ITEMS",
                column: "WRITE_OFF_ITEM_ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WRITE_OFFS",
                table: "WRITE_OFFS",
                column: "WRITE_OFF_ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SUPPLY_ITEMS",
                table: "SUPPLY_ITEMS",
                column: "SUPPLY_ITEM_ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SUPPLIES",
                table: "SUPPLIES",
                column: "SUPPLY_ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SUPPLIERS",
                table: "SUPPLIERS",
                column: "SUPPLIER_ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SALE_ITEMS",
                table: "SALE_ITEMS",
                column: "SALE_ITEM_ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SALES",
                table: "SALES",
                column: "SALE_ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PRODUCTS",
                table: "PRODUCTS",
                column: "PRODUCT_ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CUSTOMERS",
                table: "CUSTOMERS",
                column: "CUSTOMER_ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CATEGORIES",
                table: "CATEGORIES",
                column: "CATEGORY_ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_USERS",
                table: "USERS",
                column: "USER_ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ROLES",
                table: "ROLES",
                column: "ROLE_ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CHANGE_LOGS",
                table: "CHANGE_LOGS",
                column: "CHANGE_LOG_ID");

            migrationBuilder.AddForeignKey(
                name: "FK_CHANGE_LOGS_USERS_CHANGED_BY_USER_ID",
                table: "CHANGE_LOGS",
                column: "CHANGED_BY_USER_ID",
                principalTable: "USERS",
                principalColumn: "USER_ID",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_CUSTOMERS_USERS_USER_ID",
                table: "CUSTOMERS",
                column: "USER_ID",
                principalTable: "USERS",
                principalColumn: "USER_ID",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_PRODUCTS_CATEGORIES_CATEGORY_ID",
                table: "PRODUCTS",
                column: "CATEGORY_ID",
                principalTable: "CATEGORIES",
                principalColumn: "CATEGORY_ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SALE_ITEMS_PRODUCTS_PRODUCT_ID",
                table: "SALE_ITEMS",
                column: "PRODUCT_ID",
                principalTable: "PRODUCTS",
                principalColumn: "PRODUCT_ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SALE_ITEMS_SALES_SALE_ID",
                table: "SALE_ITEMS",
                column: "SALE_ID",
                principalTable: "SALES",
                principalColumn: "SALE_ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SALES_CUSTOMERS_CUSTOMER_ID",
                table: "SALES",
                column: "CUSTOMER_ID",
                principalTable: "CUSTOMERS",
                principalColumn: "CUSTOMER_ID",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_SALES_USERS_SELLER_USER_ID",
                table: "SALES",
                column: "SELLER_USER_ID",
                principalTable: "USERS",
                principalColumn: "USER_ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SUPPLIES_SUPPLIERS_SUPPLIER_ID",
                table: "SUPPLIES",
                column: "SUPPLIER_ID",
                principalTable: "SUPPLIERS",
                principalColumn: "SUPPLIER_ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SUPPLY_ITEMS_PRODUCTS_PRODUCT_ID",
                table: "SUPPLY_ITEMS",
                column: "PRODUCT_ID",
                principalTable: "PRODUCTS",
                principalColumn: "PRODUCT_ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SUPPLY_ITEMS_SUPPLIES_SUPPLY_ID",
                table: "SUPPLY_ITEMS",
                column: "SUPPLY_ID",
                principalTable: "SUPPLIES",
                principalColumn: "SUPPLY_ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_USERS_ROLES_ROLE_ID",
                table: "USERS",
                column: "ROLE_ID",
                principalTable: "ROLES",
                principalColumn: "ROLE_ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WRITE_OFF_ITEMS_PRODUCTS_PRODUCT_ID",
                table: "WRITE_OFF_ITEMS",
                column: "PRODUCT_ID",
                principalTable: "PRODUCTS",
                principalColumn: "PRODUCT_ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WRITE_OFF_ITEMS_WRITE_OFFS_WRITE_OFF_ID",
                table: "WRITE_OFF_ITEMS",
                column: "WRITE_OFF_ID",
                principalTable: "WRITE_OFFS",
                principalColumn: "WRITE_OFF_ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WRITE_OFFS_USERS_CREATED_BY_USER_ID",
                table: "WRITE_OFFS",
                column: "CREATED_BY_USER_ID",
                principalTable: "USERS",
                principalColumn: "USER_ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
