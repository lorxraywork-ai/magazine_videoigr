using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VideoGameStoreSystem.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CATEGORIES",
                columns: table => new
                {
                    CATEGORY_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CATEGORY_NAME = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CATEGORIES", x => x.CATEGORY_ID);
                });

            migrationBuilder.CreateTable(
                name: "ROLES",
                columns: table => new
                {
                    ROLE_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ROLE_NAME = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ROLES", x => x.ROLE_ID);
                });

            migrationBuilder.CreateTable(
                name: "SUPPLIERS",
                columns: table => new
                {
                    SUPPLIER_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SUPPLIER_NAME = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    PHONE = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    EMAIL = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SUPPLIERS", x => x.SUPPLIER_ID);
                });

            migrationBuilder.CreateTable(
                name: "PRODUCTS",
                columns: table => new
                {
                    PRODUCT_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PRODUCT_NAME = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CATEGORY_ID = table.Column<int>(type: "int", nullable: false),
                    SKU = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    DESCRIPTION = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    PRICE = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    COST_PRICE = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    STOCK_QUANTITY = table.Column<int>(type: "int", nullable: false),
                    IS_ACTIVE = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PRODUCTS", x => x.PRODUCT_ID);
                    table.ForeignKey(
                        name: "FK_PRODUCTS_CATEGORIES_CATEGORY_ID",
                        column: x => x.CATEGORY_ID,
                        principalTable: "CATEGORIES",
                        principalColumn: "CATEGORY_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "USERS",
                columns: table => new
                {
                    USER_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LOGIN = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PASSWORD_HASH = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    FULL_NAME = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    EMAIL = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    ROLE_ID = table.Column<int>(type: "int", nullable: false),
                    IS_ACTIVE = table.Column<bool>(type: "bit", nullable: false),
                    CREATED_AT = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USERS", x => x.USER_ID);
                    table.ForeignKey(
                        name: "FK_USERS_ROLES_ROLE_ID",
                        column: x => x.ROLE_ID,
                        principalTable: "ROLES",
                        principalColumn: "ROLE_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SUPPLIES",
                columns: table => new
                {
                    SUPPLY_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SUPPLY_DATE = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SUPPLIER_ID = table.Column<int>(type: "int", nullable: false),
                    TOTAL_AMOUNT = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SUPPLIES", x => x.SUPPLY_ID);
                    table.ForeignKey(
                        name: "FK_SUPPLIES_SUPPLIERS_SUPPLIER_ID",
                        column: x => x.SUPPLIER_ID,
                        principalTable: "SUPPLIERS",
                        principalColumn: "SUPPLIER_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CHANGE_LOGS",
                columns: table => new
                {
                    CHANGE_LOG_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ENTITY_NAME = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    ENTITY_ID = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    ACTION_TYPE = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    OLD_VALUES = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NEW_VALUES = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CHANGED_AT = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CHANGED_BY_USER_ID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CHANGE_LOGS", x => x.CHANGE_LOG_ID);
                    table.ForeignKey(
                        name: "FK_CHANGE_LOGS_USERS_CHANGED_BY_USER_ID",
                        column: x => x.CHANGED_BY_USER_ID,
                        principalTable: "USERS",
                        principalColumn: "USER_ID",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "CUSTOMERS",
                columns: table => new
                {
                    CUSTOMER_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FULL_NAME = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    PHONE = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    EMAIL = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    USER_ID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CUSTOMERS", x => x.CUSTOMER_ID);
                    table.ForeignKey(
                        name: "FK_CUSTOMERS_USERS_USER_ID",
                        column: x => x.USER_ID,
                        principalTable: "USERS",
                        principalColumn: "USER_ID",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "WRITE_OFFS",
                columns: table => new
                {
                    WRITE_OFF_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WRITE_OFF_DATE = table.Column<DateTime>(type: "datetime2", nullable: false),
                    REASON = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    CREATED_BY_USER_ID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WRITE_OFFS", x => x.WRITE_OFF_ID);
                    table.ForeignKey(
                        name: "FK_WRITE_OFFS_USERS_CREATED_BY_USER_ID",
                        column: x => x.CREATED_BY_USER_ID,
                        principalTable: "USERS",
                        principalColumn: "USER_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SUPPLY_ITEMS",
                columns: table => new
                {
                    SUPPLY_ITEM_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SUPPLY_ID = table.Column<int>(type: "int", nullable: false),
                    PRODUCT_ID = table.Column<int>(type: "int", nullable: false),
                    QUANTITY = table.Column<int>(type: "int", nullable: false),
                    UNIT_COST = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SUPPLY_ITEMS", x => x.SUPPLY_ITEM_ID);
                    table.ForeignKey(
                        name: "FK_SUPPLY_ITEMS_PRODUCTS_PRODUCT_ID",
                        column: x => x.PRODUCT_ID,
                        principalTable: "PRODUCTS",
                        principalColumn: "PRODUCT_ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SUPPLY_ITEMS_SUPPLIES_SUPPLY_ID",
                        column: x => x.SUPPLY_ID,
                        principalTable: "SUPPLIES",
                        principalColumn: "SUPPLY_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SALES",
                columns: table => new
                {
                    SALE_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SALE_DATE = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CUSTOMER_ID = table.Column<int>(type: "int", nullable: true),
                    SELLER_USER_ID = table.Column<int>(type: "int", nullable: false),
                    TOTAL_AMOUNT = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SALES", x => x.SALE_ID);
                    table.ForeignKey(
                        name: "FK_SALES_CUSTOMERS_CUSTOMER_ID",
                        column: x => x.CUSTOMER_ID,
                        principalTable: "CUSTOMERS",
                        principalColumn: "CUSTOMER_ID",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_SALES_USERS_SELLER_USER_ID",
                        column: x => x.SELLER_USER_ID,
                        principalTable: "USERS",
                        principalColumn: "USER_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WRITE_OFF_ITEMS",
                columns: table => new
                {
                    WRITE_OFF_ITEM_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WRITE_OFF_ID = table.Column<int>(type: "int", nullable: false),
                    PRODUCT_ID = table.Column<int>(type: "int", nullable: false),
                    QUANTITY = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WRITE_OFF_ITEMS", x => x.WRITE_OFF_ITEM_ID);
                    table.ForeignKey(
                        name: "FK_WRITE_OFF_ITEMS_PRODUCTS_PRODUCT_ID",
                        column: x => x.PRODUCT_ID,
                        principalTable: "PRODUCTS",
                        principalColumn: "PRODUCT_ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WRITE_OFF_ITEMS_WRITE_OFFS_WRITE_OFF_ID",
                        column: x => x.WRITE_OFF_ID,
                        principalTable: "WRITE_OFFS",
                        principalColumn: "WRITE_OFF_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SALE_ITEMS",
                columns: table => new
                {
                    SALE_ITEM_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SALE_ID = table.Column<int>(type: "int", nullable: false),
                    PRODUCT_ID = table.Column<int>(type: "int", nullable: false),
                    QUANTITY = table.Column<int>(type: "int", nullable: false),
                    UNIT_PRICE = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LINE_TOTAL = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SALE_ITEMS", x => x.SALE_ITEM_ID);
                    table.ForeignKey(
                        name: "FK_SALE_ITEMS_PRODUCTS_PRODUCT_ID",
                        column: x => x.PRODUCT_ID,
                        principalTable: "PRODUCTS",
                        principalColumn: "PRODUCT_ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SALE_ITEMS_SALES_SALE_ID",
                        column: x => x.SALE_ID,
                        principalTable: "SALES",
                        principalColumn: "SALE_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CATEGORIES_CATEGORY_NAME",
                table: "CATEGORIES",
                column: "CATEGORY_NAME",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CHANGE_LOGS_CHANGED_AT",
                table: "CHANGE_LOGS",
                column: "CHANGED_AT");

            migrationBuilder.CreateIndex(
                name: "IX_CHANGE_LOGS_CHANGED_BY_USER_ID",
                table: "CHANGE_LOGS",
                column: "CHANGED_BY_USER_ID");

            migrationBuilder.CreateIndex(
                name: "IX_CUSTOMERS_EMAIL",
                table: "CUSTOMERS",
                column: "EMAIL");

            migrationBuilder.CreateIndex(
                name: "IX_CUSTOMERS_FULL_NAME",
                table: "CUSTOMERS",
                column: "FULL_NAME");

            migrationBuilder.CreateIndex(
                name: "IX_CUSTOMERS_USER_ID",
                table: "CUSTOMERS",
                column: "USER_ID",
                unique: true,
                filter: "[USER_ID] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_PRODUCTS_CATEGORY_ID",
                table: "PRODUCTS",
                column: "CATEGORY_ID");

            migrationBuilder.CreateIndex(
                name: "IX_PRODUCTS_PRODUCT_NAME",
                table: "PRODUCTS",
                column: "PRODUCT_NAME");

            migrationBuilder.CreateIndex(
                name: "IX_PRODUCTS_SKU",
                table: "PRODUCTS",
                column: "SKU",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ROLES_ROLE_NAME",
                table: "ROLES",
                column: "ROLE_NAME",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SALE_ITEMS_PRODUCT_ID",
                table: "SALE_ITEMS",
                column: "PRODUCT_ID");

            migrationBuilder.CreateIndex(
                name: "IX_SALE_ITEMS_SALE_ID",
                table: "SALE_ITEMS",
                column: "SALE_ID");

            migrationBuilder.CreateIndex(
                name: "IX_SALES_CUSTOMER_ID",
                table: "SALES",
                column: "CUSTOMER_ID");

            migrationBuilder.CreateIndex(
                name: "IX_SALES_SALE_DATE",
                table: "SALES",
                column: "SALE_DATE");

            migrationBuilder.CreateIndex(
                name: "IX_SALES_SELLER_USER_ID",
                table: "SALES",
                column: "SELLER_USER_ID");

            migrationBuilder.CreateIndex(
                name: "IX_SUPPLIERS_SUPPLIER_NAME",
                table: "SUPPLIERS",
                column: "SUPPLIER_NAME");

            migrationBuilder.CreateIndex(
                name: "IX_SUPPLIES_SUPPLIER_ID",
                table: "SUPPLIES",
                column: "SUPPLIER_ID");

            migrationBuilder.CreateIndex(
                name: "IX_SUPPLIES_SUPPLY_DATE",
                table: "SUPPLIES",
                column: "SUPPLY_DATE");

            migrationBuilder.CreateIndex(
                name: "IX_SUPPLY_ITEMS_PRODUCT_ID",
                table: "SUPPLY_ITEMS",
                column: "PRODUCT_ID");

            migrationBuilder.CreateIndex(
                name: "IX_SUPPLY_ITEMS_SUPPLY_ID",
                table: "SUPPLY_ITEMS",
                column: "SUPPLY_ID");

            migrationBuilder.CreateIndex(
                name: "IX_USERS_EMAIL",
                table: "USERS",
                column: "EMAIL");

            migrationBuilder.CreateIndex(
                name: "IX_USERS_LOGIN",
                table: "USERS",
                column: "LOGIN",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_USERS_ROLE_ID",
                table: "USERS",
                column: "ROLE_ID");

            migrationBuilder.CreateIndex(
                name: "IX_WRITE_OFF_ITEMS_PRODUCT_ID",
                table: "WRITE_OFF_ITEMS",
                column: "PRODUCT_ID");

            migrationBuilder.CreateIndex(
                name: "IX_WRITE_OFF_ITEMS_WRITE_OFF_ID",
                table: "WRITE_OFF_ITEMS",
                column: "WRITE_OFF_ID");

            migrationBuilder.CreateIndex(
                name: "IX_WRITE_OFFS_CREATED_BY_USER_ID",
                table: "WRITE_OFFS",
                column: "CREATED_BY_USER_ID");

            migrationBuilder.CreateIndex(
                name: "IX_WRITE_OFFS_WRITE_OFF_DATE",
                table: "WRITE_OFFS",
                column: "WRITE_OFF_DATE");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CHANGE_LOGS");

            migrationBuilder.DropTable(
                name: "SALE_ITEMS");

            migrationBuilder.DropTable(
                name: "SUPPLY_ITEMS");

            migrationBuilder.DropTable(
                name: "WRITE_OFF_ITEMS");

            migrationBuilder.DropTable(
                name: "SALES");

            migrationBuilder.DropTable(
                name: "SUPPLIES");

            migrationBuilder.DropTable(
                name: "PRODUCTS");

            migrationBuilder.DropTable(
                name: "WRITE_OFFS");

            migrationBuilder.DropTable(
                name: "CUSTOMERS");

            migrationBuilder.DropTable(
                name: "SUPPLIERS");

            migrationBuilder.DropTable(
                name: "CATEGORIES");

            migrationBuilder.DropTable(
                name: "USERS");

            migrationBuilder.DropTable(
                name: "ROLES");
        }
    }
}
