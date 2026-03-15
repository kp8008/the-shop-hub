using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerceAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddProductImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductImages",
                columns: table => new
                {
                    ProductImageID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductID = table.Column<int>(type: "int", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductImages", x => x.ProductImageID);
                    table.ForeignKey(
                        name: "FK_ProductImages_Products_ProductID",
                        column: x => x.ProductID,
                        principalTable: "Products",
                        principalColumn: "ProductID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "AddressID",
                keyValue: 1,
                column: "Created",
                value: new DateTime(2026, 2, 25, 12, 46, 27, 398, DateTimeKind.Local).AddTicks(2587));

            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "AddressID",
                keyValue: 2,
                column: "Created",
                value: new DateTime(2026, 2, 25, 12, 46, 27, 398, DateTimeKind.Local).AddTicks(2587));

            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "AddressID",
                keyValue: 3,
                column: "Created",
                value: new DateTime(2026, 2, 25, 12, 46, 27, 398, DateTimeKind.Local).AddTicks(2587));

            migrationBuilder.UpdateData(
                table: "CartItems",
                keyColumn: "CartItemID",
                keyValue: 1,
                column: "Created",
                value: new DateTime(2026, 2, 25, 12, 46, 27, 398, DateTimeKind.Local).AddTicks(2587));

            migrationBuilder.UpdateData(
                table: "CartItems",
                keyColumn: "CartItemID",
                keyValue: 2,
                column: "Created",
                value: new DateTime(2026, 2, 25, 12, 46, 27, 398, DateTimeKind.Local).AddTicks(2587));

            migrationBuilder.UpdateData(
                table: "CartItems",
                keyColumn: "CartItemID",
                keyValue: 3,
                column: "Created",
                value: new DateTime(2026, 2, 25, 12, 46, 27, 398, DateTimeKind.Local).AddTicks(2587));

            migrationBuilder.UpdateData(
                table: "Carts",
                keyColumn: "CartID",
                keyValue: 1,
                column: "Created",
                value: new DateTime(2026, 2, 25, 12, 46, 27, 398, DateTimeKind.Local).AddTicks(2587));

            migrationBuilder.UpdateData(
                table: "Carts",
                keyColumn: "CartID",
                keyValue: 2,
                column: "Created",
                value: new DateTime(2026, 2, 25, 12, 46, 27, 398, DateTimeKind.Local).AddTicks(2587));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "CategoryID",
                keyValue: 1,
                column: "Created",
                value: new DateTime(2026, 2, 25, 12, 46, 27, 398, DateTimeKind.Local).AddTicks(2587));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "CategoryID",
                keyValue: 2,
                column: "Created",
                value: new DateTime(2026, 2, 25, 12, 46, 27, 398, DateTimeKind.Local).AddTicks(2587));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "CategoryID",
                keyValue: 3,
                column: "Created",
                value: new DateTime(2026, 2, 25, 12, 46, 27, 398, DateTimeKind.Local).AddTicks(2587));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "CategoryID",
                keyValue: 4,
                column: "Created",
                value: new DateTime(2026, 2, 25, 12, 46, 27, 398, DateTimeKind.Local).AddTicks(2587));

            migrationBuilder.UpdateData(
                table: "Order",
                keyColumn: "OrderID",
                keyValue: 1,
                columns: new[] { "Created", "DeliveryDate", "OrderDate" },
                values: new object[] { new DateTime(2026, 2, 20, 12, 46, 27, 398, DateTimeKind.Local).AddTicks(2587), new DateTime(2026, 2, 27, 12, 46, 27, 398, DateTimeKind.Local).AddTicks(2587), new DateTime(2026, 2, 20, 12, 46, 27, 398, DateTimeKind.Local).AddTicks(2587) });

            migrationBuilder.UpdateData(
                table: "Order",
                keyColumn: "OrderID",
                keyValue: 2,
                columns: new[] { "Created", "DeliveryDate", "OrderDate" },
                values: new object[] { new DateTime(2026, 2, 22, 12, 46, 27, 398, DateTimeKind.Local).AddTicks(2587), new DateTime(2026, 2, 26, 12, 46, 27, 398, DateTimeKind.Local).AddTicks(2587), new DateTime(2026, 2, 22, 12, 46, 27, 398, DateTimeKind.Local).AddTicks(2587) });

            migrationBuilder.UpdateData(
                table: "OrderDetails",
                keyColumn: "OrderDetailID",
                keyValue: 1,
                column: "Created",
                value: new DateTime(2026, 2, 20, 12, 46, 27, 398, DateTimeKind.Local).AddTicks(2587));

            migrationBuilder.UpdateData(
                table: "OrderDetails",
                keyColumn: "OrderDetailID",
                keyValue: 2,
                column: "Created",
                value: new DateTime(2026, 2, 20, 12, 46, 27, 398, DateTimeKind.Local).AddTicks(2587));

            migrationBuilder.UpdateData(
                table: "OrderDetails",
                keyColumn: "OrderDetailID",
                keyValue: 3,
                column: "Created",
                value: new DateTime(2026, 2, 22, 12, 46, 27, 398, DateTimeKind.Local).AddTicks(2587));

            migrationBuilder.UpdateData(
                table: "Payments",
                keyColumn: "PaymentID",
                keyValue: 1,
                columns: new[] { "Created", "TransactionDate" },
                values: new object[] { new DateTime(2026, 2, 20, 12, 46, 27, 398, DateTimeKind.Local).AddTicks(2587), new DateTime(2026, 2, 20, 12, 46, 27, 398, DateTimeKind.Local).AddTicks(2587) });

            migrationBuilder.UpdateData(
                table: "Payments",
                keyColumn: "PaymentID",
                keyValue: 2,
                columns: new[] { "Created", "TransactionDate" },
                values: new object[] { new DateTime(2026, 2, 22, 12, 46, 27, 398, DateTimeKind.Local).AddTicks(2587), new DateTime(2026, 2, 22, 12, 46, 27, 398, DateTimeKind.Local).AddTicks(2587) });

            migrationBuilder.UpdateData(
                table: "ProductReviews",
                keyColumn: "ReviewID",
                keyValue: 1,
                column: "Created",
                value: new DateTime(2026, 2, 21, 12, 46, 27, 398, DateTimeKind.Local).AddTicks(2587));

            migrationBuilder.UpdateData(
                table: "ProductReviews",
                keyColumn: "ReviewID",
                keyValue: 2,
                column: "Created",
                value: new DateTime(2026, 2, 23, 12, 46, 27, 398, DateTimeKind.Local).AddTicks(2587));

            migrationBuilder.UpdateData(
                table: "ProductReviews",
                keyColumn: "ReviewID",
                keyValue: 3,
                column: "Created",
                value: new DateTime(2026, 2, 24, 12, 46, 27, 398, DateTimeKind.Local).AddTicks(2587));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductID",
                keyValue: 1,
                column: "Created",
                value: new DateTime(2026, 2, 25, 12, 46, 27, 398, DateTimeKind.Local).AddTicks(2587));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductID",
                keyValue: 2,
                column: "Created",
                value: new DateTime(2026, 2, 25, 12, 46, 27, 398, DateTimeKind.Local).AddTicks(2587));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductID",
                keyValue: 3,
                column: "Created",
                value: new DateTime(2026, 2, 25, 12, 46, 27, 398, DateTimeKind.Local).AddTicks(2587));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductID",
                keyValue: 4,
                column: "Created",
                value: new DateTime(2026, 2, 25, 12, 46, 27, 398, DateTimeKind.Local).AddTicks(2587));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductID",
                keyValue: 5,
                column: "Created",
                value: new DateTime(2026, 2, 25, 12, 46, 27, 398, DateTimeKind.Local).AddTicks(2587));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductID",
                keyValue: 6,
                column: "Created",
                value: new DateTime(2026, 2, 25, 12, 46, 27, 398, DateTimeKind.Local).AddTicks(2587));

            migrationBuilder.UpdateData(
                table: "UserTypes",
                keyColumn: "UserTypeID",
                keyValue: 1,
                column: "Created",
                value: new DateTime(2026, 2, 25, 12, 46, 27, 398, DateTimeKind.Local).AddTicks(2587));

            migrationBuilder.UpdateData(
                table: "UserTypes",
                keyColumn: "UserTypeID",
                keyValue: 2,
                column: "Created",
                value: new DateTime(2026, 2, 25, 12, 46, 27, 398, DateTimeKind.Local).AddTicks(2587));

            migrationBuilder.UpdateData(
                table: "UserTypes",
                keyColumn: "UserTypeID",
                keyValue: 3,
                column: "Created",
                value: new DateTime(2026, 2, 25, 12, 46, 27, 398, DateTimeKind.Local).AddTicks(2587));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserID",
                keyValue: 1,
                columns: new[] { "Created", "Password" },
                values: new object[] { new DateTime(2026, 2, 25, 12, 46, 27, 398, DateTimeKind.Local).AddTicks(2587), "$2a$11$BPPu2vvvjiZE86G1mRgyqeq2OypRx.f0ClnJu37bCUPZ0EfrepJRS" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserID",
                keyValue: 2,
                columns: new[] { "Created", "Password" },
                values: new object[] { new DateTime(2026, 2, 25, 12, 46, 27, 398, DateTimeKind.Local).AddTicks(2587), "$2a$11$C.5DZbjs5b2RKP0WfEQq1OlUl3lAFe50fB.HxVJmPivAKJWlZ2hzC" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserID",
                keyValue: 3,
                columns: new[] { "Created", "Password" },
                values: new object[] { new DateTime(2026, 2, 25, 12, 46, 27, 398, DateTimeKind.Local).AddTicks(2587), "$2a$11$3aNho89LYR4UGLpx/AS/u.4D7m36ENDr0ZpQs.pUsfxoIpjU5RbLe" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserID",
                keyValue: 4,
                columns: new[] { "Created", "Password" },
                values: new object[] { new DateTime(2026, 2, 25, 12, 46, 27, 398, DateTimeKind.Local).AddTicks(2587), "$2a$11$w3bC8dFs4cjQW3dkqPwvue6Z61s9GFvtOvX84KR6eTSAR1UGwUj8y" });

            migrationBuilder.CreateIndex(
                name: "IX_ProductImages_ProductID",
                table: "ProductImages",
                column: "ProductID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductImages");

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
        }
    }
}
