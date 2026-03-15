using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ECommerceAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddProductReviewImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "ProductReviews",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.DeleteData(
                table: "ProductReviews",
                keyColumn: "ReviewID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ProductReviews",
                keyColumn: "ReviewID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "ProductReviews",
                keyColumn: "ReviewID",
                keyValue: 3);

            migrationBuilder.AlterColumn<string>(
                name: "Comment",
                table: "ProductReviews",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000);

            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "AddressID",
                keyValue: 1,
                column: "Created",
                value: new DateTime(2026, 3, 12, 12, 46, 39, 166, DateTimeKind.Local).AddTicks(8839));

            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "AddressID",
                keyValue: 2,
                column: "Created",
                value: new DateTime(2026, 3, 12, 12, 46, 39, 166, DateTimeKind.Local).AddTicks(8839));

            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "AddressID",
                keyValue: 3,
                column: "Created",
                value: new DateTime(2026, 3, 12, 12, 46, 39, 166, DateTimeKind.Local).AddTicks(8839));

            migrationBuilder.UpdateData(
                table: "CartItems",
                keyColumn: "CartItemID",
                keyValue: 1,
                column: "Created",
                value: new DateTime(2026, 3, 12, 12, 46, 39, 166, DateTimeKind.Local).AddTicks(8839));

            migrationBuilder.UpdateData(
                table: "CartItems",
                keyColumn: "CartItemID",
                keyValue: 2,
                column: "Created",
                value: new DateTime(2026, 3, 12, 12, 46, 39, 166, DateTimeKind.Local).AddTicks(8839));

            migrationBuilder.UpdateData(
                table: "CartItems",
                keyColumn: "CartItemID",
                keyValue: 3,
                column: "Created",
                value: new DateTime(2026, 3, 12, 12, 46, 39, 166, DateTimeKind.Local).AddTicks(8839));

            migrationBuilder.UpdateData(
                table: "Carts",
                keyColumn: "CartID",
                keyValue: 1,
                column: "Created",
                value: new DateTime(2026, 3, 12, 12, 46, 39, 166, DateTimeKind.Local).AddTicks(8839));

            migrationBuilder.UpdateData(
                table: "Carts",
                keyColumn: "CartID",
                keyValue: 2,
                column: "Created",
                value: new DateTime(2026, 3, 12, 12, 46, 39, 166, DateTimeKind.Local).AddTicks(8839));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "CategoryID",
                keyValue: 1,
                column: "Created",
                value: new DateTime(2026, 3, 12, 12, 46, 39, 166, DateTimeKind.Local).AddTicks(8839));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "CategoryID",
                keyValue: 2,
                column: "Created",
                value: new DateTime(2026, 3, 12, 12, 46, 39, 166, DateTimeKind.Local).AddTicks(8839));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "CategoryID",
                keyValue: 3,
                column: "Created",
                value: new DateTime(2026, 3, 12, 12, 46, 39, 166, DateTimeKind.Local).AddTicks(8839));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "CategoryID",
                keyValue: 4,
                column: "Created",
                value: new DateTime(2026, 3, 12, 12, 46, 39, 166, DateTimeKind.Local).AddTicks(8839));

            migrationBuilder.UpdateData(
                table: "Order",
                keyColumn: "OrderID",
                keyValue: 1,
                columns: new[] { "Created", "DeliveryDate", "OrderDate" },
                values: new object[] { new DateTime(2026, 3, 7, 12, 46, 39, 166, DateTimeKind.Local).AddTicks(8839), new DateTime(2026, 3, 14, 12, 46, 39, 166, DateTimeKind.Local).AddTicks(8839), new DateTime(2026, 3, 7, 12, 46, 39, 166, DateTimeKind.Local).AddTicks(8839) });

            migrationBuilder.UpdateData(
                table: "Order",
                keyColumn: "OrderID",
                keyValue: 2,
                columns: new[] { "Created", "DeliveryDate", "OrderDate" },
                values: new object[] { new DateTime(2026, 3, 9, 12, 46, 39, 166, DateTimeKind.Local).AddTicks(8839), new DateTime(2026, 3, 13, 12, 46, 39, 166, DateTimeKind.Local).AddTicks(8839), new DateTime(2026, 3, 9, 12, 46, 39, 166, DateTimeKind.Local).AddTicks(8839) });

            migrationBuilder.UpdateData(
                table: "OrderDetails",
                keyColumn: "OrderDetailID",
                keyValue: 1,
                column: "Created",
                value: new DateTime(2026, 3, 7, 12, 46, 39, 166, DateTimeKind.Local).AddTicks(8839));

            migrationBuilder.UpdateData(
                table: "OrderDetails",
                keyColumn: "OrderDetailID",
                keyValue: 2,
                column: "Created",
                value: new DateTime(2026, 3, 7, 12, 46, 39, 166, DateTimeKind.Local).AddTicks(8839));

            migrationBuilder.UpdateData(
                table: "OrderDetails",
                keyColumn: "OrderDetailID",
                keyValue: 3,
                column: "Created",
                value: new DateTime(2026, 3, 9, 12, 46, 39, 166, DateTimeKind.Local).AddTicks(8839));

            migrationBuilder.UpdateData(
                table: "Payments",
                keyColumn: "PaymentID",
                keyValue: 1,
                columns: new[] { "Created", "TransactionDate" },
                values: new object[] { new DateTime(2026, 3, 7, 12, 46, 39, 166, DateTimeKind.Local).AddTicks(8839), new DateTime(2026, 3, 7, 12, 46, 39, 166, DateTimeKind.Local).AddTicks(8839) });

            migrationBuilder.UpdateData(
                table: "Payments",
                keyColumn: "PaymentID",
                keyValue: 2,
                columns: new[] { "Created", "TransactionDate" },
                values: new object[] { new DateTime(2026, 3, 9, 12, 46, 39, 166, DateTimeKind.Local).AddTicks(8839), new DateTime(2026, 3, 9, 12, 46, 39, 166, DateTimeKind.Local).AddTicks(8839) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductID",
                keyValue: 1,
                column: "Created",
                value: new DateTime(2026, 3, 12, 12, 46, 39, 166, DateTimeKind.Local).AddTicks(8839));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductID",
                keyValue: 2,
                column: "Created",
                value: new DateTime(2026, 3, 12, 12, 46, 39, 166, DateTimeKind.Local).AddTicks(8839));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductID",
                keyValue: 3,
                column: "Created",
                value: new DateTime(2026, 3, 12, 12, 46, 39, 166, DateTimeKind.Local).AddTicks(8839));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductID",
                keyValue: 4,
                column: "Created",
                value: new DateTime(2026, 3, 12, 12, 46, 39, 166, DateTimeKind.Local).AddTicks(8839));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductID",
                keyValue: 5,
                column: "Created",
                value: new DateTime(2026, 3, 12, 12, 46, 39, 166, DateTimeKind.Local).AddTicks(8839));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductID",
                keyValue: 6,
                column: "Created",
                value: new DateTime(2026, 3, 12, 12, 46, 39, 166, DateTimeKind.Local).AddTicks(8839));

            migrationBuilder.UpdateData(
                table: "UserTypes",
                keyColumn: "UserTypeID",
                keyValue: 1,
                column: "Created",
                value: new DateTime(2026, 3, 12, 12, 46, 39, 166, DateTimeKind.Local).AddTicks(8839));

            migrationBuilder.UpdateData(
                table: "UserTypes",
                keyColumn: "UserTypeID",
                keyValue: 2,
                column: "Created",
                value: new DateTime(2026, 3, 12, 12, 46, 39, 166, DateTimeKind.Local).AddTicks(8839));

            migrationBuilder.UpdateData(
                table: "UserTypes",
                keyColumn: "UserTypeID",
                keyValue: 3,
                column: "Created",
                value: new DateTime(2026, 3, 12, 12, 46, 39, 166, DateTimeKind.Local).AddTicks(8839));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserID",
                keyValue: 1,
                columns: new[] { "Created", "Password" },
                values: new object[] { new DateTime(2026, 3, 12, 12, 46, 39, 166, DateTimeKind.Local).AddTicks(8839), "$2a$11$3aVUHVwvrHny.ECgwf1DVe.QZkvBvbJlnULQg0Bw.TeyVFATmuZ/q" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserID",
                keyValue: 2,
                columns: new[] { "Created", "Password" },
                values: new object[] { new DateTime(2026, 3, 12, 12, 46, 39, 166, DateTimeKind.Local).AddTicks(8839), "$2a$11$fB8lznovvFBQiDJYUGsPjegQB1C5/K5sLSoQpWuBFWbyFGJj0NDRa" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserID",
                keyValue: 3,
                columns: new[] { "Created", "Password" },
                values: new object[] { new DateTime(2026, 3, 12, 12, 46, 39, 166, DateTimeKind.Local).AddTicks(8839), "$2a$11$zCdPhjC9h6H41anxSmNoROClaMgvpydtg1h5DS16VdJzC43kZpaLO" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserID",
                keyValue: 4,
                columns: new[] { "Created", "Password" },
                values: new object[] { new DateTime(2026, 3, 12, 12, 46, 39, 166, DateTimeKind.Local).AddTicks(8839), "$2a$11$90r7MeXtHE9HwllF7lPxIO8VvpbJw6bX2Y2F./rXwzPVxJn0jJ5RW" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Comment",
                table: "ProductReviews",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

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

            migrationBuilder.InsertData(
                table: "ProductReviews",
                columns: new[] { "ReviewID", "Comment", "Created", "Image", "Modified", "ProductID", "Rating", "Title", "UserID" },
                values: new object[,]
                {
                    { 1, "Very good quality and fast performance. Highly recommended!", new DateTime(2026, 2, 21, 12, 46, 27, 398, DateTimeKind.Local).AddTicks(2587), null, null, 1, 5, "Excellent Phone", 2 },
                    { 2, "Nice laptop with good features. Battery life could be better.", new DateTime(2026, 2, 23, 12, 46, 27, 398, DateTimeKind.Local).AddTicks(2587), null, null, 2, 4, "Good Laptop", 3 },
                    { 3, "Good quality fabric and comfortable to wear.", new DateTime(2026, 2, 24, 12, 46, 27, 398, DateTimeKind.Local).AddTicks(2587), null, null, 3, 4, "Comfortable T-Shirt", 2 }
                });

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

            migrationBuilder.DropColumn(
                name: "Image",
                table: "ProductReviews");
        }
    }
}
