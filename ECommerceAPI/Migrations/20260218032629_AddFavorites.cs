using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerceAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddFavorites : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Favorites",
                columns: table => new
                {
                    FavoriteID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    ProductID = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Favorites", x => x.FavoriteID);
                    table.ForeignKey(
                        name: "FK_Favorites_Products_ProductID",
                        column: x => x.ProductID,
                        principalTable: "Products",
                        principalColumn: "ProductID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Favorites_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "AddressID",
                keyValue: 1,
                column: "Created",
                value: new DateTime(2026, 2, 18, 8, 56, 26, 379, DateTimeKind.Local).AddTicks(9157));

            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "AddressID",
                keyValue: 2,
                column: "Created",
                value: new DateTime(2026, 2, 18, 8, 56, 26, 379, DateTimeKind.Local).AddTicks(9157));

            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "AddressID",
                keyValue: 3,
                column: "Created",
                value: new DateTime(2026, 2, 18, 8, 56, 26, 379, DateTimeKind.Local).AddTicks(9157));

            migrationBuilder.UpdateData(
                table: "CartItems",
                keyColumn: "CartItemID",
                keyValue: 1,
                column: "Created",
                value: new DateTime(2026, 2, 18, 8, 56, 26, 379, DateTimeKind.Local).AddTicks(9157));

            migrationBuilder.UpdateData(
                table: "CartItems",
                keyColumn: "CartItemID",
                keyValue: 2,
                column: "Created",
                value: new DateTime(2026, 2, 18, 8, 56, 26, 379, DateTimeKind.Local).AddTicks(9157));

            migrationBuilder.UpdateData(
                table: "CartItems",
                keyColumn: "CartItemID",
                keyValue: 3,
                column: "Created",
                value: new DateTime(2026, 2, 18, 8, 56, 26, 379, DateTimeKind.Local).AddTicks(9157));

            migrationBuilder.UpdateData(
                table: "Carts",
                keyColumn: "CartID",
                keyValue: 1,
                column: "Created",
                value: new DateTime(2026, 2, 18, 8, 56, 26, 379, DateTimeKind.Local).AddTicks(9157));

            migrationBuilder.UpdateData(
                table: "Carts",
                keyColumn: "CartID",
                keyValue: 2,
                column: "Created",
                value: new DateTime(2026, 2, 18, 8, 56, 26, 379, DateTimeKind.Local).AddTicks(9157));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "CategoryID",
                keyValue: 1,
                column: "Created",
                value: new DateTime(2026, 2, 18, 8, 56, 26, 379, DateTimeKind.Local).AddTicks(9157));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "CategoryID",
                keyValue: 2,
                column: "Created",
                value: new DateTime(2026, 2, 18, 8, 56, 26, 379, DateTimeKind.Local).AddTicks(9157));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "CategoryID",
                keyValue: 3,
                column: "Created",
                value: new DateTime(2026, 2, 18, 8, 56, 26, 379, DateTimeKind.Local).AddTicks(9157));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "CategoryID",
                keyValue: 4,
                column: "Created",
                value: new DateTime(2026, 2, 18, 8, 56, 26, 379, DateTimeKind.Local).AddTicks(9157));

            migrationBuilder.UpdateData(
                table: "Order",
                keyColumn: "OrderID",
                keyValue: 1,
                columns: new[] { "Created", "DeliveryDate", "OrderDate" },
                values: new object[] { new DateTime(2026, 2, 13, 8, 56, 26, 379, DateTimeKind.Local).AddTicks(9157), new DateTime(2026, 2, 20, 8, 56, 26, 379, DateTimeKind.Local).AddTicks(9157), new DateTime(2026, 2, 13, 8, 56, 26, 379, DateTimeKind.Local).AddTicks(9157) });

            migrationBuilder.UpdateData(
                table: "Order",
                keyColumn: "OrderID",
                keyValue: 2,
                columns: new[] { "Created", "DeliveryDate", "OrderDate" },
                values: new object[] { new DateTime(2026, 2, 15, 8, 56, 26, 379, DateTimeKind.Local).AddTicks(9157), new DateTime(2026, 2, 19, 8, 56, 26, 379, DateTimeKind.Local).AddTicks(9157), new DateTime(2026, 2, 15, 8, 56, 26, 379, DateTimeKind.Local).AddTicks(9157) });

            migrationBuilder.UpdateData(
                table: "OrderDetails",
                keyColumn: "OrderDetailID",
                keyValue: 1,
                column: "Created",
                value: new DateTime(2026, 2, 13, 8, 56, 26, 379, DateTimeKind.Local).AddTicks(9157));

            migrationBuilder.UpdateData(
                table: "OrderDetails",
                keyColumn: "OrderDetailID",
                keyValue: 2,
                column: "Created",
                value: new DateTime(2026, 2, 13, 8, 56, 26, 379, DateTimeKind.Local).AddTicks(9157));

            migrationBuilder.UpdateData(
                table: "OrderDetails",
                keyColumn: "OrderDetailID",
                keyValue: 3,
                column: "Created",
                value: new DateTime(2026, 2, 15, 8, 56, 26, 379, DateTimeKind.Local).AddTicks(9157));

            migrationBuilder.UpdateData(
                table: "Payments",
                keyColumn: "PaymentID",
                keyValue: 1,
                columns: new[] { "Created", "TransactionDate" },
                values: new object[] { new DateTime(2026, 2, 13, 8, 56, 26, 379, DateTimeKind.Local).AddTicks(9157), new DateTime(2026, 2, 13, 8, 56, 26, 379, DateTimeKind.Local).AddTicks(9157) });

            migrationBuilder.UpdateData(
                table: "Payments",
                keyColumn: "PaymentID",
                keyValue: 2,
                columns: new[] { "Created", "TransactionDate" },
                values: new object[] { new DateTime(2026, 2, 15, 8, 56, 26, 379, DateTimeKind.Local).AddTicks(9157), new DateTime(2026, 2, 15, 8, 56, 26, 379, DateTimeKind.Local).AddTicks(9157) });

            migrationBuilder.UpdateData(
                table: "ProductReviews",
                keyColumn: "ReviewID",
                keyValue: 1,
                column: "Created",
                value: new DateTime(2026, 2, 14, 8, 56, 26, 379, DateTimeKind.Local).AddTicks(9157));

            migrationBuilder.UpdateData(
                table: "ProductReviews",
                keyColumn: "ReviewID",
                keyValue: 2,
                column: "Created",
                value: new DateTime(2026, 2, 16, 8, 56, 26, 379, DateTimeKind.Local).AddTicks(9157));

            migrationBuilder.UpdateData(
                table: "ProductReviews",
                keyColumn: "ReviewID",
                keyValue: 3,
                column: "Created",
                value: new DateTime(2026, 2, 17, 8, 56, 26, 379, DateTimeKind.Local).AddTicks(9157));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductID",
                keyValue: 1,
                column: "Created",
                value: new DateTime(2026, 2, 18, 8, 56, 26, 379, DateTimeKind.Local).AddTicks(9157));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductID",
                keyValue: 2,
                column: "Created",
                value: new DateTime(2026, 2, 18, 8, 56, 26, 379, DateTimeKind.Local).AddTicks(9157));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductID",
                keyValue: 3,
                column: "Created",
                value: new DateTime(2026, 2, 18, 8, 56, 26, 379, DateTimeKind.Local).AddTicks(9157));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductID",
                keyValue: 4,
                column: "Created",
                value: new DateTime(2026, 2, 18, 8, 56, 26, 379, DateTimeKind.Local).AddTicks(9157));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductID",
                keyValue: 5,
                column: "Created",
                value: new DateTime(2026, 2, 18, 8, 56, 26, 379, DateTimeKind.Local).AddTicks(9157));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductID",
                keyValue: 6,
                column: "Created",
                value: new DateTime(2026, 2, 18, 8, 56, 26, 379, DateTimeKind.Local).AddTicks(9157));

            migrationBuilder.UpdateData(
                table: "UserTypes",
                keyColumn: "UserTypeID",
                keyValue: 1,
                column: "Created",
                value: new DateTime(2026, 2, 18, 8, 56, 26, 379, DateTimeKind.Local).AddTicks(9157));

            migrationBuilder.UpdateData(
                table: "UserTypes",
                keyColumn: "UserTypeID",
                keyValue: 2,
                column: "Created",
                value: new DateTime(2026, 2, 18, 8, 56, 26, 379, DateTimeKind.Local).AddTicks(9157));

            migrationBuilder.UpdateData(
                table: "UserTypes",
                keyColumn: "UserTypeID",
                keyValue: 3,
                column: "Created",
                value: new DateTime(2026, 2, 18, 8, 56, 26, 379, DateTimeKind.Local).AddTicks(9157));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserID",
                keyValue: 1,
                columns: new[] { "Created", "Password" },
                values: new object[] { new DateTime(2026, 2, 18, 8, 56, 26, 379, DateTimeKind.Local).AddTicks(9157), "$2a$11$fT5a.jyLyfHVfHrgdAQwhO1RigV7syYzmjZrwhJL7LNoUDq35N.Su" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserID",
                keyValue: 2,
                columns: new[] { "Created", "Password" },
                values: new object[] { new DateTime(2026, 2, 18, 8, 56, 26, 379, DateTimeKind.Local).AddTicks(9157), "$2a$11$bYP/b0VZ6JLeOo6DVRPaTe1R.eMZxjse3/JVNr3x7N9mVsDKAESGG" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserID",
                keyValue: 3,
                columns: new[] { "Created", "Password" },
                values: new object[] { new DateTime(2026, 2, 18, 8, 56, 26, 379, DateTimeKind.Local).AddTicks(9157), "$2a$11$1e3ekogXfgcUc0YJfzrDC.Isrfdy8RwhgC6jK5z8bd3DfaznynB5e" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserID",
                keyValue: 4,
                columns: new[] { "Created", "Password" },
                values: new object[] { new DateTime(2026, 2, 18, 8, 56, 26, 379, DateTimeKind.Local).AddTicks(9157), "$2a$11$/51BGUJ3cZzvop6IE0OaFeUOgHJNkEmZR4rg7x4h6IX7hSxjcvO3G" });

            migrationBuilder.CreateIndex(
                name: "IX_Favorites_ProductID",
                table: "Favorites",
                column: "ProductID");

            migrationBuilder.CreateIndex(
                name: "IX_Favorites_UserID_ProductID",
                table: "Favorites",
                columns: new[] { "UserID", "ProductID" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Favorites");

            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "AddressID",
                keyValue: 1,
                column: "Created",
                value: new DateTime(2026, 2, 16, 8, 52, 29, 383, DateTimeKind.Local).AddTicks(7594));

            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "AddressID",
                keyValue: 2,
                column: "Created",
                value: new DateTime(2026, 2, 16, 8, 52, 29, 383, DateTimeKind.Local).AddTicks(7594));

            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "AddressID",
                keyValue: 3,
                column: "Created",
                value: new DateTime(2026, 2, 16, 8, 52, 29, 383, DateTimeKind.Local).AddTicks(7594));

            migrationBuilder.UpdateData(
                table: "CartItems",
                keyColumn: "CartItemID",
                keyValue: 1,
                column: "Created",
                value: new DateTime(2026, 2, 16, 8, 52, 29, 383, DateTimeKind.Local).AddTicks(7594));

            migrationBuilder.UpdateData(
                table: "CartItems",
                keyColumn: "CartItemID",
                keyValue: 2,
                column: "Created",
                value: new DateTime(2026, 2, 16, 8, 52, 29, 383, DateTimeKind.Local).AddTicks(7594));

            migrationBuilder.UpdateData(
                table: "CartItems",
                keyColumn: "CartItemID",
                keyValue: 3,
                column: "Created",
                value: new DateTime(2026, 2, 16, 8, 52, 29, 383, DateTimeKind.Local).AddTicks(7594));

            migrationBuilder.UpdateData(
                table: "Carts",
                keyColumn: "CartID",
                keyValue: 1,
                column: "Created",
                value: new DateTime(2026, 2, 16, 8, 52, 29, 383, DateTimeKind.Local).AddTicks(7594));

            migrationBuilder.UpdateData(
                table: "Carts",
                keyColumn: "CartID",
                keyValue: 2,
                column: "Created",
                value: new DateTime(2026, 2, 16, 8, 52, 29, 383, DateTimeKind.Local).AddTicks(7594));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "CategoryID",
                keyValue: 1,
                column: "Created",
                value: new DateTime(2026, 2, 16, 8, 52, 29, 383, DateTimeKind.Local).AddTicks(7594));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "CategoryID",
                keyValue: 2,
                column: "Created",
                value: new DateTime(2026, 2, 16, 8, 52, 29, 383, DateTimeKind.Local).AddTicks(7594));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "CategoryID",
                keyValue: 3,
                column: "Created",
                value: new DateTime(2026, 2, 16, 8, 52, 29, 383, DateTimeKind.Local).AddTicks(7594));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "CategoryID",
                keyValue: 4,
                column: "Created",
                value: new DateTime(2026, 2, 16, 8, 52, 29, 383, DateTimeKind.Local).AddTicks(7594));

            migrationBuilder.UpdateData(
                table: "Order",
                keyColumn: "OrderID",
                keyValue: 1,
                columns: new[] { "Created", "DeliveryDate", "OrderDate" },
                values: new object[] { new DateTime(2026, 2, 11, 8, 52, 29, 383, DateTimeKind.Local).AddTicks(7594), new DateTime(2026, 2, 18, 8, 52, 29, 383, DateTimeKind.Local).AddTicks(7594), new DateTime(2026, 2, 11, 8, 52, 29, 383, DateTimeKind.Local).AddTicks(7594) });

            migrationBuilder.UpdateData(
                table: "Order",
                keyColumn: "OrderID",
                keyValue: 2,
                columns: new[] { "Created", "DeliveryDate", "OrderDate" },
                values: new object[] { new DateTime(2026, 2, 13, 8, 52, 29, 383, DateTimeKind.Local).AddTicks(7594), new DateTime(2026, 2, 17, 8, 52, 29, 383, DateTimeKind.Local).AddTicks(7594), new DateTime(2026, 2, 13, 8, 52, 29, 383, DateTimeKind.Local).AddTicks(7594) });

            migrationBuilder.UpdateData(
                table: "OrderDetails",
                keyColumn: "OrderDetailID",
                keyValue: 1,
                column: "Created",
                value: new DateTime(2026, 2, 11, 8, 52, 29, 383, DateTimeKind.Local).AddTicks(7594));

            migrationBuilder.UpdateData(
                table: "OrderDetails",
                keyColumn: "OrderDetailID",
                keyValue: 2,
                column: "Created",
                value: new DateTime(2026, 2, 11, 8, 52, 29, 383, DateTimeKind.Local).AddTicks(7594));

            migrationBuilder.UpdateData(
                table: "OrderDetails",
                keyColumn: "OrderDetailID",
                keyValue: 3,
                column: "Created",
                value: new DateTime(2026, 2, 13, 8, 52, 29, 383, DateTimeKind.Local).AddTicks(7594));

            migrationBuilder.UpdateData(
                table: "Payments",
                keyColumn: "PaymentID",
                keyValue: 1,
                columns: new[] { "Created", "TransactionDate" },
                values: new object[] { new DateTime(2026, 2, 11, 8, 52, 29, 383, DateTimeKind.Local).AddTicks(7594), new DateTime(2026, 2, 11, 8, 52, 29, 383, DateTimeKind.Local).AddTicks(7594) });

            migrationBuilder.UpdateData(
                table: "Payments",
                keyColumn: "PaymentID",
                keyValue: 2,
                columns: new[] { "Created", "TransactionDate" },
                values: new object[] { new DateTime(2026, 2, 13, 8, 52, 29, 383, DateTimeKind.Local).AddTicks(7594), new DateTime(2026, 2, 13, 8, 52, 29, 383, DateTimeKind.Local).AddTicks(7594) });

            migrationBuilder.UpdateData(
                table: "ProductReviews",
                keyColumn: "ReviewID",
                keyValue: 1,
                column: "Created",
                value: new DateTime(2026, 2, 12, 8, 52, 29, 383, DateTimeKind.Local).AddTicks(7594));

            migrationBuilder.UpdateData(
                table: "ProductReviews",
                keyColumn: "ReviewID",
                keyValue: 2,
                column: "Created",
                value: new DateTime(2026, 2, 14, 8, 52, 29, 383, DateTimeKind.Local).AddTicks(7594));

            migrationBuilder.UpdateData(
                table: "ProductReviews",
                keyColumn: "ReviewID",
                keyValue: 3,
                column: "Created",
                value: new DateTime(2026, 2, 15, 8, 52, 29, 383, DateTimeKind.Local).AddTicks(7594));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductID",
                keyValue: 1,
                column: "Created",
                value: new DateTime(2026, 2, 16, 8, 52, 29, 383, DateTimeKind.Local).AddTicks(7594));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductID",
                keyValue: 2,
                column: "Created",
                value: new DateTime(2026, 2, 16, 8, 52, 29, 383, DateTimeKind.Local).AddTicks(7594));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductID",
                keyValue: 3,
                column: "Created",
                value: new DateTime(2026, 2, 16, 8, 52, 29, 383, DateTimeKind.Local).AddTicks(7594));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductID",
                keyValue: 4,
                column: "Created",
                value: new DateTime(2026, 2, 16, 8, 52, 29, 383, DateTimeKind.Local).AddTicks(7594));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductID",
                keyValue: 5,
                column: "Created",
                value: new DateTime(2026, 2, 16, 8, 52, 29, 383, DateTimeKind.Local).AddTicks(7594));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductID",
                keyValue: 6,
                column: "Created",
                value: new DateTime(2026, 2, 16, 8, 52, 29, 383, DateTimeKind.Local).AddTicks(7594));

            migrationBuilder.UpdateData(
                table: "UserTypes",
                keyColumn: "UserTypeID",
                keyValue: 1,
                column: "Created",
                value: new DateTime(2026, 2, 16, 8, 52, 29, 383, DateTimeKind.Local).AddTicks(7594));

            migrationBuilder.UpdateData(
                table: "UserTypes",
                keyColumn: "UserTypeID",
                keyValue: 2,
                column: "Created",
                value: new DateTime(2026, 2, 16, 8, 52, 29, 383, DateTimeKind.Local).AddTicks(7594));

            migrationBuilder.UpdateData(
                table: "UserTypes",
                keyColumn: "UserTypeID",
                keyValue: 3,
                column: "Created",
                value: new DateTime(2026, 2, 16, 8, 52, 29, 383, DateTimeKind.Local).AddTicks(7594));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserID",
                keyValue: 1,
                columns: new[] { "Created", "Password" },
                values: new object[] { new DateTime(2026, 2, 16, 8, 52, 29, 383, DateTimeKind.Local).AddTicks(7594), "$2a$11$/hGayGE07wBoFlOv2ZtL/e/Df4bcMLDwG8JSZbwhr2Kufo3pMfOTu" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserID",
                keyValue: 2,
                columns: new[] { "Created", "Password" },
                values: new object[] { new DateTime(2026, 2, 16, 8, 52, 29, 383, DateTimeKind.Local).AddTicks(7594), "$2a$11$btjtrtr37pn7a9nbvUsYd.J1TKqFWtDkZBOq2lpK4R.My96NJW3KK" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserID",
                keyValue: 3,
                columns: new[] { "Created", "Password" },
                values: new object[] { new DateTime(2026, 2, 16, 8, 52, 29, 383, DateTimeKind.Local).AddTicks(7594), "$2a$11$y3vEDFSMlNjF3MbIbRCROO5V9RryandIj6/NPlWdQsxirXmWYuRa." });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserID",
                keyValue: 4,
                columns: new[] { "Created", "Password" },
                values: new object[] { new DateTime(2026, 2, 16, 8, 52, 29, 383, DateTimeKind.Local).AddTicks(7594), "$2a$11$r39ffBMEKFiMOhigb0G/cuKIG.wPNsnfzo0RSgrkOlAxhiE66UrfS" });
        }
    }
}
