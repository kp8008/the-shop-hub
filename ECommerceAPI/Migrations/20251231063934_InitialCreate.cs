using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ECommerceAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PaymentModes",
                columns: table => new
                {
                    PaymentModeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PaymentModeName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentModes", x => x.PaymentModeID);
                });

            migrationBuilder.CreateTable(
                name: "UserTypes",
                columns: table => new
                {
                    UserTypeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserTypeName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTypes", x => x.UserTypeID);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    UserTypeID = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserID);
                    table.ForeignKey(
                        name: "FK_Users_UserTypes_UserTypeID",
                        column: x => x.UserTypeID,
                        principalTable: "UserTypes",
                        principalColumn: "UserTypeID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Addresses",
                columns: table => new
                {
                    AddressID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    ReceiverName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    AddressLine1 = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Landmark = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    State = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Pincode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses", x => x.AddressID);
                    table.ForeignKey(
                        name: "FK_Addresses_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Carts",
                columns: table => new
                {
                    CartID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Carts", x => x.CartID);
                    table.ForeignKey(
                        name: "FK_Carts_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    CategoryID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UserID = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.CategoryID);
                    table.ForeignKey(
                        name: "FK_Categories_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID");
                });

            migrationBuilder.CreateTable(
                name: "Order",
                columns: table => new
                {
                    OrderID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    OrderNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    OrderDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeliveryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AddressID = table.Column<int>(type: "int", nullable: true),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CouponDiscount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    NetAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Order", x => x.OrderID);
                    table.ForeignKey(
                        name: "FK_Order_Addresses_AddressID",
                        column: x => x.AddressID,
                        principalTable: "Addresses",
                        principalColumn: "AddressID");
                    table.ForeignKey(
                        name: "FK_Order_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    ProductID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ProductCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CategoryID = table.Column<int>(type: "int", nullable: false),
                    UserID = table.Column<int>(type: "int", nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Image = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.ProductID);
                    table.ForeignKey(
                        name: "FK_Products_Categories_CategoryID",
                        column: x => x.CategoryID,
                        principalTable: "Categories",
                        principalColumn: "CategoryID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Products_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID");
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    PaymentID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderID = table.Column<int>(type: "int", nullable: false),
                    PaymentModeID = table.Column<int>(type: "int", nullable: false),
                    TotalPayment = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaymentReference = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PaymentStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TransactionID = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.PaymentID);
                    table.ForeignKey(
                        name: "FK_Payments_Order_OrderID",
                        column: x => x.OrderID,
                        principalTable: "Order",
                        principalColumn: "OrderID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Payments_PaymentModes_PaymentModeID",
                        column: x => x.PaymentModeID,
                        principalTable: "PaymentModes",
                        principalColumn: "PaymentModeID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Payments_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID");
                });

            migrationBuilder.CreateTable(
                name: "CartItems",
                columns: table => new
                {
                    CartItemID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CartID = table.Column<int>(type: "int", nullable: false),
                    ProductID = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartItems", x => x.CartItemID);
                    table.ForeignKey(
                        name: "FK_CartItems_Carts_CartID",
                        column: x => x.CartID,
                        principalTable: "Carts",
                        principalColumn: "CartID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CartItems_Products_ProductID",
                        column: x => x.ProductID,
                        principalTable: "Products",
                        principalColumn: "ProductID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderDetails",
                columns: table => new
                {
                    OrderDetailID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderID = table.Column<int>(type: "int", nullable: false),
                    ProductID = table.Column<int>(type: "int", nullable: false),
                    AddressID = table.Column<int>(type: "int", nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Discount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NetAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderDetails", x => x.OrderDetailID);
                    table.ForeignKey(
                        name: "FK_OrderDetails_Addresses_AddressID",
                        column: x => x.AddressID,
                        principalTable: "Addresses",
                        principalColumn: "AddressID");
                    table.ForeignKey(
                        name: "FK_OrderDetails_Order_OrderID",
                        column: x => x.OrderID,
                        principalTable: "Order",
                        principalColumn: "OrderID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderDetails_Products_ProductID",
                        column: x => x.ProductID,
                        principalTable: "Products",
                        principalColumn: "ProductID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductReviews",
                columns: table => new
                {
                    ReviewID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductID = table.Column<int>(type: "int", nullable: false),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductReviews", x => x.ReviewID);
                    table.ForeignKey(
                        name: "FK_ProductReviews_Products_ProductID",
                        column: x => x.ProductID,
                        principalTable: "Products",
                        principalColumn: "ProductID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductReviews_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "CategoryID", "CategoryName", "Created", "IsActive", "Modified", "UserID" },
                values: new object[,]
                {
                    { 1, "Electronics", new DateTime(2025, 12, 31, 12, 9, 33, 661, DateTimeKind.Local).AddTicks(9389), true, null, null },
                    { 2, "Clothing", new DateTime(2025, 12, 31, 12, 9, 33, 661, DateTimeKind.Local).AddTicks(9389), true, null, null },
                    { 3, "Books", new DateTime(2025, 12, 31, 12, 9, 33, 661, DateTimeKind.Local).AddTicks(9389), true, null, null }
                });

            migrationBuilder.InsertData(
                table: "PaymentModes",
                columns: new[] { "PaymentModeID", "PaymentModeName" },
                values: new object[,]
                {
                    { 1, "Credit Card" },
                    { 2, "Debit Card" },
                    { 3, "UPI" },
                    { 4, "Cash on Delivery" },
                    { 5, "Net Banking" }
                });

            migrationBuilder.InsertData(
                table: "UserTypes",
                columns: new[] { "UserTypeID", "Created", "Modified", "UserTypeName" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 12, 31, 12, 9, 33, 661, DateTimeKind.Local).AddTicks(9389), null, "Admin" },
                    { 2, new DateTime(2025, 12, 31, 12, 9, 33, 661, DateTimeKind.Local).AddTicks(9389), null, "Customer" },
                    { 3, new DateTime(2025, 12, 31, 12, 9, 33, 661, DateTimeKind.Local).AddTicks(9389), null, "Seller" }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "ProductID", "CategoryID", "Created", "Image", "IsActive", "Modified", "Price", "ProductCode", "ProductName", "UserID" },
                values: new object[,]
                {
                    { 3, 2, new DateTime(2025, 12, 31, 12, 9, 33, 661, DateTimeKind.Local).AddTicks(9389), "https://example.com/images/tshirt.jpg", true, null, 599.00m, "CLO001", "T-Shirt", null },
                    { 4, 2, new DateTime(2025, 12, 31, 12, 9, 33, 661, DateTimeKind.Local).AddTicks(9389), "https://example.com/images/jeans.jpg", true, null, 1299.00m, "CLO002", "Jeans", null },
                    { 5, 3, new DateTime(2025, 12, 31, 12, 9, 33, 661, DateTimeKind.Local).AddTicks(9389), "https://example.com/images/book.jpg", true, null, 499.00m, "BOOK001", "Programming Book", null }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserID", "Address", "Created", "Email", "IsActive", "Modified", "Password", "Phone", "UserName", "UserTypeID" },
                values: new object[,]
                {
                    { 1, "Admin Address, Ahmedabad", new DateTime(2025, 12, 31, 12, 9, 33, 661, DateTimeKind.Local).AddTicks(9389), "admin@example.com", true, null, "Admin@123", "9876543210", "Admin User", 1 },
                    { 2, "123 Main Street, Ahmedabad", new DateTime(2025, 12, 31, 12, 9, 33, 661, DateTimeKind.Local).AddTicks(9389), "rahul@example.com", true, null, "Customer@123", "9876543211", "Rahul Patel", 2 },
                    { 3, "456 Park Avenue, Surat", new DateTime(2025, 12, 31, 12, 9, 33, 661, DateTimeKind.Local).AddTicks(9389), "priya@example.com", true, null, "Customer@123", "9876543212", "Priya Shah", 2 },
                    { 4, "789 Business Park, Vadodara", new DateTime(2025, 12, 31, 12, 9, 33, 661, DateTimeKind.Local).AddTicks(9389), "seller@example.com", true, null, "Seller@123", "9876543213", "Tech Seller", 3 }
                });

            migrationBuilder.InsertData(
                table: "Addresses",
                columns: new[] { "AddressID", "AddressLine1", "City", "Country", "Created", "IsDefault", "Landmark", "Modified", "Phone", "Pincode", "ReceiverName", "State", "UserID" },
                values: new object[,]
                {
                    { 1, "123 Main Street", "Ahmedabad", "India", new DateTime(2025, 12, 31, 12, 9, 33, 661, DateTimeKind.Local).AddTicks(9389), true, "Near City Mall", null, "9876543211", "380001", "Rahul Patel", "Gujarat", 2 },
                    { 2, "456 Office Complex", "Ahmedabad", "India", new DateTime(2025, 12, 31, 12, 9, 33, 661, DateTimeKind.Local).AddTicks(9389), false, "Opposite Bank", null, "9876543211", "380015", "Rahul Patel", "Gujarat", 2 },
                    { 3, "789 Park Avenue", "Surat", "India", new DateTime(2025, 12, 31, 12, 9, 33, 661, DateTimeKind.Local).AddTicks(9389), true, "Near School", null, "9876543212", "395001", "Priya Shah", "Gujarat", 3 }
                });

            migrationBuilder.InsertData(
                table: "Carts",
                columns: new[] { "CartID", "Created", "Modified", "UserID" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 12, 31, 12, 9, 33, 661, DateTimeKind.Local).AddTicks(9389), null, 2 },
                    { 2, new DateTime(2025, 12, 31, 12, 9, 33, 661, DateTimeKind.Local).AddTicks(9389), null, 3 }
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "CategoryID", "CategoryName", "Created", "IsActive", "Modified", "UserID" },
                values: new object[] { 4, "Home & Kitchen", new DateTime(2025, 12, 31, 12, 9, 33, 661, DateTimeKind.Local).AddTicks(9389), true, null, 4 });

            migrationBuilder.InsertData(
                table: "ProductReviews",
                columns: new[] { "ReviewID", "Comment", "Created", "Modified", "ProductID", "Rating", "Title", "UserID" },
                values: new object[] { 3, "Good quality fabric and comfortable to wear.", new DateTime(2025, 12, 30, 12, 9, 33, 661, DateTimeKind.Local).AddTicks(9389), null, 3, 4, "Comfortable T-Shirt", 2 });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "ProductID", "CategoryID", "Created", "Image", "IsActive", "Modified", "Price", "ProductCode", "ProductName", "UserID" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2025, 12, 31, 12, 9, 33, 661, DateTimeKind.Local).AddTicks(9389), "https://example.com/images/smartphone.jpg", true, null, 25000.00m, "ELEC001", "Smartphone", 4 },
                    { 2, 1, new DateTime(2025, 12, 31, 12, 9, 33, 661, DateTimeKind.Local).AddTicks(9389), "https://example.com/images/laptop.jpg", true, null, 55000.00m, "ELEC002", "Laptop", 4 }
                });

            migrationBuilder.InsertData(
                table: "CartItems",
                columns: new[] { "CartItemID", "CartID", "Created", "Modified", "Price", "ProductID", "Quantity" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2025, 12, 31, 12, 9, 33, 661, DateTimeKind.Local).AddTicks(9389), null, 25000.00m, 1, 1 },
                    { 2, 1, new DateTime(2025, 12, 31, 12, 9, 33, 661, DateTimeKind.Local).AddTicks(9389), null, 599.00m, 3, 2 },
                    { 3, 2, new DateTime(2025, 12, 31, 12, 9, 33, 661, DateTimeKind.Local).AddTicks(9389), null, 55000.00m, 2, 1 }
                });

            migrationBuilder.InsertData(
                table: "Order",
                columns: new[] { "OrderID", "AddressID", "CouponDiscount", "Created", "DeliveryDate", "Modified", "NetAmount", "OrderDate", "OrderNo", "TotalAmount", "UserID" },
                values: new object[,]
                {
                    { 1, 1, 200.00m, new DateTime(2025, 12, 26, 12, 9, 33, 661, DateTimeKind.Local).AddTicks(9389), new DateTime(2026, 1, 2, 12, 9, 33, 661, DateTimeKind.Local).AddTicks(9389), null, 25998.00m, new DateTime(2025, 12, 26, 12, 9, 33, 661, DateTimeKind.Local).AddTicks(9389), "ORD001", 26198.00m, 2 },
                    { 2, 3, null, new DateTime(2025, 12, 28, 12, 9, 33, 661, DateTimeKind.Local).AddTicks(9389), new DateTime(2026, 1, 1, 12, 9, 33, 661, DateTimeKind.Local).AddTicks(9389), null, 55000.00m, new DateTime(2025, 12, 28, 12, 9, 33, 661, DateTimeKind.Local).AddTicks(9389), "ORD002", 55000.00m, 3 }
                });

            migrationBuilder.InsertData(
                table: "ProductReviews",
                columns: new[] { "ReviewID", "Comment", "Created", "Modified", "ProductID", "Rating", "Title", "UserID" },
                values: new object[,]
                {
                    { 1, "Very good quality and fast performance. Highly recommended!", new DateTime(2025, 12, 27, 12, 9, 33, 661, DateTimeKind.Local).AddTicks(9389), null, 1, 5, "Excellent Phone", 2 },
                    { 2, "Nice laptop with good features. Battery life could be better.", new DateTime(2025, 12, 29, 12, 9, 33, 661, DateTimeKind.Local).AddTicks(9389), null, 2, 4, "Good Laptop", 3 }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "ProductID", "CategoryID", "Created", "Image", "IsActive", "Modified", "Price", "ProductCode", "ProductName", "UserID" },
                values: new object[] { 6, 4, new DateTime(2025, 12, 31, 12, 9, 33, 661, DateTimeKind.Local).AddTicks(9389), "https://example.com/images/coffeemaker.jpg", true, null, 3499.00m, "HK001", "Coffee Maker", 4 });

            migrationBuilder.InsertData(
                table: "OrderDetails",
                columns: new[] { "OrderDetailID", "AddressID", "Amount", "Created", "Discount", "Modified", "NetAmount", "OrderID", "ProductID", "Quantity" },
                values: new object[,]
                {
                    { 1, 1, 25000.00m, new DateTime(2025, 12, 26, 12, 9, 33, 661, DateTimeKind.Local).AddTicks(9389), 0.00m, null, 25000.00m, 1, 1, 1 },
                    { 2, 1, 1198.00m, new DateTime(2025, 12, 26, 12, 9, 33, 661, DateTimeKind.Local).AddTicks(9389), 0.00m, null, 1198.00m, 1, 3, 2 },
                    { 3, 3, 55000.00m, new DateTime(2025, 12, 28, 12, 9, 33, 661, DateTimeKind.Local).AddTicks(9389), 0.00m, null, 55000.00m, 2, 2, 1 }
                });

            migrationBuilder.InsertData(
                table: "Payments",
                columns: new[] { "PaymentID", "Created", "Modified", "OrderID", "PaymentModeID", "PaymentReference", "PaymentStatus", "TotalPayment", "TransactionDate", "TransactionID", "UserID" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 12, 26, 12, 9, 33, 661, DateTimeKind.Local).AddTicks(9389), null, 1, 3, "UPI123456789", "Completed", 25998.00m, new DateTime(2025, 12, 26, 12, 9, 33, 661, DateTimeKind.Local).AddTicks(9389), "TXN001", null },
                    { 2, new DateTime(2025, 12, 28, 12, 9, 33, 661, DateTimeKind.Local).AddTicks(9389), null, 2, 1, "CC987654321", "Completed", 55000.00m, new DateTime(2025, 12, 28, 12, 9, 33, 661, DateTimeKind.Local).AddTicks(9389), "TXN002", null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_UserID",
                table: "Addresses",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_CartID",
                table: "CartItems",
                column: "CartID");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_ProductID",
                table: "CartItems",
                column: "ProductID");

            migrationBuilder.CreateIndex(
                name: "IX_Carts_UserID",
                table: "Carts",
                column: "UserID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categories_UserID",
                table: "Categories",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Order_AddressID",
                table: "Order",
                column: "AddressID");

            migrationBuilder.CreateIndex(
                name: "IX_Order_UserID",
                table: "Order",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_AddressID",
                table: "OrderDetails",
                column: "AddressID");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_OrderID",
                table: "OrderDetails",
                column: "OrderID");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_ProductID",
                table: "OrderDetails",
                column: "ProductID");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_OrderID",
                table: "Payments",
                column: "OrderID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_PaymentModeID",
                table: "Payments",
                column: "PaymentModeID");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_UserID",
                table: "Payments",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_ProductReviews_ProductID",
                table: "ProductReviews",
                column: "ProductID");

            migrationBuilder.CreateIndex(
                name: "IX_ProductReviews_UserID",
                table: "ProductReviews",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryID",
                table: "Products",
                column: "CategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_Products_UserID",
                table: "Products",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserTypeID",
                table: "Users",
                column: "UserTypeID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CartItems");

            migrationBuilder.DropTable(
                name: "OrderDetails");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "ProductReviews");

            migrationBuilder.DropTable(
                name: "Carts");

            migrationBuilder.DropTable(
                name: "Order");

            migrationBuilder.DropTable(
                name: "PaymentModes");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Addresses");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "UserTypes");
        }
    }
}
