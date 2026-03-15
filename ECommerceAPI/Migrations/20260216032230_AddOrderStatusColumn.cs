using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerceAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderStatusColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Order",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

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
                columns: new[] { "Created", "DeliveryDate", "OrderDate", "Status" },
                values: new object[] { new DateTime(2026, 2, 11, 8, 52, 29, 383, DateTimeKind.Local).AddTicks(7594), new DateTime(2026, 2, 18, 8, 52, 29, 383, DateTimeKind.Local).AddTicks(7594), new DateTime(2026, 2, 11, 8, 52, 29, 383, DateTimeKind.Local).AddTicks(7594), "Pending" });

            migrationBuilder.UpdateData(
                table: "Order",
                keyColumn: "OrderID",
                keyValue: 2,
                columns: new[] { "Created", "DeliveryDate", "OrderDate", "Status" },
                values: new object[] { new DateTime(2026, 2, 13, 8, 52, 29, 383, DateTimeKind.Local).AddTicks(7594), new DateTime(2026, 2, 17, 8, 52, 29, 383, DateTimeKind.Local).AddTicks(7594), new DateTime(2026, 2, 13, 8, 52, 29, 383, DateTimeKind.Local).AddTicks(7594), "Pending" });

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
                columns: new[] { "Created", "StockQuantity" },
                values: new object[] { new DateTime(2026, 2, 16, 8, 52, 29, 383, DateTimeKind.Local).AddTicks(7594), 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductID",
                keyValue: 2,
                columns: new[] { "Created", "StockQuantity" },
                values: new object[] { new DateTime(2026, 2, 16, 8, 52, 29, 383, DateTimeKind.Local).AddTicks(7594), 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductID",
                keyValue: 3,
                columns: new[] { "Created", "StockQuantity" },
                values: new object[] { new DateTime(2026, 2, 16, 8, 52, 29, 383, DateTimeKind.Local).AddTicks(7594), 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductID",
                keyValue: 4,
                columns: new[] { "Created", "StockQuantity" },
                values: new object[] { new DateTime(2026, 2, 16, 8, 52, 29, 383, DateTimeKind.Local).AddTicks(7594), 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductID",
                keyValue: 5,
                columns: new[] { "Created", "StockQuantity" },
                values: new object[] { new DateTime(2026, 2, 16, 8, 52, 29, 383, DateTimeKind.Local).AddTicks(7594), 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductID",
                keyValue: 6,
                columns: new[] { "Created", "StockQuantity" },
                values: new object[] { new DateTime(2026, 2, 16, 8, 52, 29, 383, DateTimeKind.Local).AddTicks(7594), 0 });

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Order");

            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "AddressID",
                keyValue: 1,
                column: "Created",
                value: new DateTime(2025, 12, 31, 12, 9, 33, 661, DateTimeKind.Local).AddTicks(9389));

            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "AddressID",
                keyValue: 2,
                column: "Created",
                value: new DateTime(2025, 12, 31, 12, 9, 33, 661, DateTimeKind.Local).AddTicks(9389));

            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "AddressID",
                keyValue: 3,
                column: "Created",
                value: new DateTime(2025, 12, 31, 12, 9, 33, 661, DateTimeKind.Local).AddTicks(9389));

            migrationBuilder.UpdateData(
                table: "CartItems",
                keyColumn: "CartItemID",
                keyValue: 1,
                column: "Created",
                value: new DateTime(2025, 12, 31, 12, 9, 33, 661, DateTimeKind.Local).AddTicks(9389));

            migrationBuilder.UpdateData(
                table: "CartItems",
                keyColumn: "CartItemID",
                keyValue: 2,
                column: "Created",
                value: new DateTime(2025, 12, 31, 12, 9, 33, 661, DateTimeKind.Local).AddTicks(9389));

            migrationBuilder.UpdateData(
                table: "CartItems",
                keyColumn: "CartItemID",
                keyValue: 3,
                column: "Created",
                value: new DateTime(2025, 12, 31, 12, 9, 33, 661, DateTimeKind.Local).AddTicks(9389));

            migrationBuilder.UpdateData(
                table: "Carts",
                keyColumn: "CartID",
                keyValue: 1,
                column: "Created",
                value: new DateTime(2025, 12, 31, 12, 9, 33, 661, DateTimeKind.Local).AddTicks(9389));

            migrationBuilder.UpdateData(
                table: "Carts",
                keyColumn: "CartID",
                keyValue: 2,
                column: "Created",
                value: new DateTime(2025, 12, 31, 12, 9, 33, 661, DateTimeKind.Local).AddTicks(9389));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "CategoryID",
                keyValue: 1,
                column: "Created",
                value: new DateTime(2025, 12, 31, 12, 9, 33, 661, DateTimeKind.Local).AddTicks(9389));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "CategoryID",
                keyValue: 2,
                column: "Created",
                value: new DateTime(2025, 12, 31, 12, 9, 33, 661, DateTimeKind.Local).AddTicks(9389));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "CategoryID",
                keyValue: 3,
                column: "Created",
                value: new DateTime(2025, 12, 31, 12, 9, 33, 661, DateTimeKind.Local).AddTicks(9389));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "CategoryID",
                keyValue: 4,
                column: "Created",
                value: new DateTime(2025, 12, 31, 12, 9, 33, 661, DateTimeKind.Local).AddTicks(9389));

            migrationBuilder.UpdateData(
                table: "Order",
                keyColumn: "OrderID",
                keyValue: 1,
                columns: new[] { "Created", "DeliveryDate", "OrderDate" },
                values: new object[] { new DateTime(2025, 12, 26, 12, 9, 33, 661, DateTimeKind.Local).AddTicks(9389), new DateTime(2026, 1, 2, 12, 9, 33, 661, DateTimeKind.Local).AddTicks(9389), new DateTime(2025, 12, 26, 12, 9, 33, 661, DateTimeKind.Local).AddTicks(9389) });

            migrationBuilder.UpdateData(
                table: "Order",
                keyColumn: "OrderID",
                keyValue: 2,
                columns: new[] { "Created", "DeliveryDate", "OrderDate" },
                values: new object[] { new DateTime(2025, 12, 28, 12, 9, 33, 661, DateTimeKind.Local).AddTicks(9389), new DateTime(2026, 1, 1, 12, 9, 33, 661, DateTimeKind.Local).AddTicks(9389), new DateTime(2025, 12, 28, 12, 9, 33, 661, DateTimeKind.Local).AddTicks(9389) });

            migrationBuilder.UpdateData(
                table: "OrderDetails",
                keyColumn: "OrderDetailID",
                keyValue: 1,
                column: "Created",
                value: new DateTime(2025, 12, 26, 12, 9, 33, 661, DateTimeKind.Local).AddTicks(9389));

            migrationBuilder.UpdateData(
                table: "OrderDetails",
                keyColumn: "OrderDetailID",
                keyValue: 2,
                column: "Created",
                value: new DateTime(2025, 12, 26, 12, 9, 33, 661, DateTimeKind.Local).AddTicks(9389));

            migrationBuilder.UpdateData(
                table: "OrderDetails",
                keyColumn: "OrderDetailID",
                keyValue: 3,
                column: "Created",
                value: new DateTime(2025, 12, 28, 12, 9, 33, 661, DateTimeKind.Local).AddTicks(9389));

            migrationBuilder.UpdateData(
                table: "Payments",
                keyColumn: "PaymentID",
                keyValue: 1,
                columns: new[] { "Created", "TransactionDate" },
                values: new object[] { new DateTime(2025, 12, 26, 12, 9, 33, 661, DateTimeKind.Local).AddTicks(9389), new DateTime(2025, 12, 26, 12, 9, 33, 661, DateTimeKind.Local).AddTicks(9389) });

            migrationBuilder.UpdateData(
                table: "Payments",
                keyColumn: "PaymentID",
                keyValue: 2,
                columns: new[] { "Created", "TransactionDate" },
                values: new object[] { new DateTime(2025, 12, 28, 12, 9, 33, 661, DateTimeKind.Local).AddTicks(9389), new DateTime(2025, 12, 28, 12, 9, 33, 661, DateTimeKind.Local).AddTicks(9389) });

            migrationBuilder.UpdateData(
                table: "ProductReviews",
                keyColumn: "ReviewID",
                keyValue: 1,
                column: "Created",
                value: new DateTime(2025, 12, 27, 12, 9, 33, 661, DateTimeKind.Local).AddTicks(9389));

            migrationBuilder.UpdateData(
                table: "ProductReviews",
                keyColumn: "ReviewID",
                keyValue: 2,
                column: "Created",
                value: new DateTime(2025, 12, 29, 12, 9, 33, 661, DateTimeKind.Local).AddTicks(9389));

            migrationBuilder.UpdateData(
                table: "ProductReviews",
                keyColumn: "ReviewID",
                keyValue: 3,
                column: "Created",
                value: new DateTime(2025, 12, 30, 12, 9, 33, 661, DateTimeKind.Local).AddTicks(9389));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductID",
                keyValue: 1,
                column: "Created",
                value: new DateTime(2025, 12, 31, 12, 9, 33, 661, DateTimeKind.Local).AddTicks(9389));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductID",
                keyValue: 2,
                column: "Created",
                value: new DateTime(2025, 12, 31, 12, 9, 33, 661, DateTimeKind.Local).AddTicks(9389));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductID",
                keyValue: 3,
                column: "Created",
                value: new DateTime(2025, 12, 31, 12, 9, 33, 661, DateTimeKind.Local).AddTicks(9389));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductID",
                keyValue: 4,
                column: "Created",
                value: new DateTime(2025, 12, 31, 12, 9, 33, 661, DateTimeKind.Local).AddTicks(9389));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductID",
                keyValue: 5,
                column: "Created",
                value: new DateTime(2025, 12, 31, 12, 9, 33, 661, DateTimeKind.Local).AddTicks(9389));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductID",
                keyValue: 6,
                column: "Created",
                value: new DateTime(2025, 12, 31, 12, 9, 33, 661, DateTimeKind.Local).AddTicks(9389));

            migrationBuilder.UpdateData(
                table: "UserTypes",
                keyColumn: "UserTypeID",
                keyValue: 1,
                column: "Created",
                value: new DateTime(2025, 12, 31, 12, 9, 33, 661, DateTimeKind.Local).AddTicks(9389));

            migrationBuilder.UpdateData(
                table: "UserTypes",
                keyColumn: "UserTypeID",
                keyValue: 2,
                column: "Created",
                value: new DateTime(2025, 12, 31, 12, 9, 33, 661, DateTimeKind.Local).AddTicks(9389));

            migrationBuilder.UpdateData(
                table: "UserTypes",
                keyColumn: "UserTypeID",
                keyValue: 3,
                column: "Created",
                value: new DateTime(2025, 12, 31, 12, 9, 33, 661, DateTimeKind.Local).AddTicks(9389));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserID",
                keyValue: 1,
                columns: new[] { "Created", "Password" },
                values: new object[] { new DateTime(2025, 12, 31, 12, 9, 33, 661, DateTimeKind.Local).AddTicks(9389), "Admin@123" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserID",
                keyValue: 2,
                columns: new[] { "Created", "Password" },
                values: new object[] { new DateTime(2025, 12, 31, 12, 9, 33, 661, DateTimeKind.Local).AddTicks(9389), "Customer@123" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserID",
                keyValue: 3,
                columns: new[] { "Created", "Password" },
                values: new object[] { new DateTime(2025, 12, 31, 12, 9, 33, 661, DateTimeKind.Local).AddTicks(9389), "Customer@123" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserID",
                keyValue: 4,
                columns: new[] { "Created", "Password" },
                values: new object[] { new DateTime(2025, 12, 31, 12, 9, 33, 661, DateTimeKind.Local).AddTicks(9389), "Seller@123" });
        }
    }
}
