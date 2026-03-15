using ECommerceAPI.Models;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;


namespace ECommerceAPI.Services
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        // User & Roles
        public DbSet<UserType> UserTypes { get; set; }
        public DbSet<User> Users { get; set; }

        // Product & Category
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }

        // Address 
        public DbSet<Address> Addresses { get; set; }

        // Cart
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }

        // Orders
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }

        // Payment
        public DbSet<PaymentMode> PaymentModes { get; set; }
        public DbSet<Payment> Payments { get; set; }

        // Reviews
        public DbSet<ProductReview> ProductReviews { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        
        // Favorites
        public DbSet<Favorite> Favorites { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var seedDate = DateTime.Now;

            // Seed UserTypes
            modelBuilder.Entity<UserType>().HasData(
                new UserType { UserTypeID = 1, UserTypeName = "Admin", Created = seedDate },
                new UserType { UserTypeID = 2, UserTypeName = "Customer", Created = seedDate },
                new UserType { UserTypeID = 3, UserTypeName = "Seller", Created = seedDate }
            );

            // Seed Users (with hashed passwords)
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    UserID = 1,
                    UserName = "Admin User",
                    Address = "Admin Address, Ahmedabad",
                    Phone = "9876543210",
                    Email = "admin@example.com",
                    Password = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                    UserTypeID = 1,
                    IsActive = true,
                    Created = seedDate
                },
                new User
                {
                    UserID = 2,
                    UserName = "Rahul Patel",
                    Address = "123 Main Street, Ahmedabad",
                    Phone = "9876543211",
                    Email = "rahul@example.com",
                    Password = BCrypt.Net.BCrypt.HashPassword("Customer@123"),
                    UserTypeID = 2,
                    IsActive = true,
                    Created = seedDate
                },
                new User
                {
                    UserID = 3,
                    UserName = "Priya Shah",
                    Address = "456 Park Avenue, Surat",
                    Phone = "9876543212",
                    Email = "priya@example.com",
                    Password = BCrypt.Net.BCrypt.HashPassword("Customer@123"),
                    UserTypeID = 2,
                    IsActive = true,
                    Created = seedDate
                },
                new User
                {
                    UserID = 4,
                    UserName = "Tech Seller",
                    Address = "789 Business Park, Vadodara",
                    Phone = "9876543213",
                    Email = "seller@example.com",
                    Password = BCrypt.Net.BCrypt.HashPassword("Seller@123"),
                    UserTypeID = 3,
                    IsActive = true,
                    Created = seedDate
                }
            );

            // Seed Categories
            modelBuilder.Entity<Category>().HasData(
                new Category
                {
                    CategoryID = 1,
                    CategoryName = "Electronics",
                    UserID = null,
                    IsActive = true,
                    Created = seedDate
                },
                new Category
                {
                    CategoryID = 2,
                    CategoryName = "Clothing",
                    UserID = null,
                    IsActive = true,
                    Created = seedDate
                },
                new Category
                {
                    CategoryID = 3,
                    CategoryName = "Books",
                    UserID = null,
                    IsActive = true,
                    Created = seedDate
                },
                new Category
                {
                    CategoryID = 4,
                    CategoryName = "Home & Kitchen",
                    UserID = 4,
                    IsActive = true,
                    Created = seedDate
                }
            );

            // Seed Products
            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    ProductID = 1,
                    ProductName = "Smartphone",
                    ProductCode = "ELEC001",
                    CategoryID = 1,
                    UserID = 4,
                    Price = 25000.00m,
                    Image = "https://example.com/images/smartphone.jpg",
                    IsActive = true,
                    Created = seedDate
                },
                new Product
                {
                    ProductID = 2,
                    ProductName = "Laptop",
                    ProductCode = "ELEC002",
                    CategoryID = 1,
                    UserID = 4,
                    Price = 55000.00m,
                    Image = "https://example.com/images/laptop.jpg",
                    IsActive = true,
                    Created = seedDate
                },
                new Product
                {
                    ProductID = 3,
                    ProductName = "T-Shirt",
                    ProductCode = "CLO001",
                    CategoryID = 2,
                    UserID = null,
                    Price = 599.00m,
                    Image = "https://example.com/images/tshirt.jpg",
                    IsActive = true,
                    Created = seedDate
                },
                new Product
                {
                    ProductID = 4,
                    ProductName = "Jeans",
                    ProductCode = "CLO002",
                    CategoryID = 2,
                    UserID = null,
                    Price = 1299.00m,
                    Image = "https://example.com/images/jeans.jpg",
                    IsActive = true,
                    Created = seedDate
                },
                new Product
                {
                    ProductID = 5,
                    ProductName = "Programming Book",
                    ProductCode = "BOOK001",
                    CategoryID = 3,
                    UserID = null,
                    Price = 499.00m,
                    Image = "https://example.com/images/book.jpg",
                    IsActive = true,
                    Created = seedDate
                },
                new Product
                {
                    ProductID = 6,
                    ProductName = "Coffee Maker",
                    ProductCode = "HK001",
                    CategoryID = 4,
                    UserID = 4,
                    Price = 3499.00m,
                    Image = "https://example.com/images/coffeemaker.jpg",
                    IsActive = true,
                    Created = seedDate
                }
            );

            // Seed Addresses
            modelBuilder.Entity<Address>().HasData(
                new Address
                {
                    AddressID = 1,
                    UserID = 2,
                    ReceiverName = "Rahul Patel",
                    Phone = "9876543211",
                    AddressLine1 = "123 Main Street",
                    Landmark = "Near City Mall",
                    City = "Ahmedabad",
                    State = "Gujarat",
                    Country = "India",
                    Pincode = "380001",
                    IsDefault = true,
                    Created = seedDate
                },
                new Address
                {
                    AddressID = 2,
                    UserID = 2,
                    ReceiverName = "Rahul Patel",
                    Phone = "9876543211",
                    AddressLine1 = "456 Office Complex",
                    Landmark = "Opposite Bank",
                    City = "Ahmedabad",
                    State = "Gujarat",
                    Country = "India",
                    Pincode = "380015",
                    IsDefault = false,
                    Created = seedDate
                },
                new Address
                {
                    AddressID = 3,
                    UserID = 3,
                    ReceiverName = "Priya Shah",
                    Phone = "9876543212",
                    AddressLine1 = "789 Park Avenue",
                    Landmark = "Near School",
                    City = "Surat",
                    State = "Gujarat",
                    Country = "India",
                    Pincode = "395001",
                    IsDefault = true,
                    Created = seedDate
                }
            );

            // Seed Carts
            modelBuilder.Entity<Cart>().HasData(
                new Cart { CartID = 1, UserID = 2, Created = seedDate },
                new Cart { CartID = 2, UserID = 3, Created = seedDate }
            );

            // Seed CartItems
            modelBuilder.Entity<CartItem>().HasData(
                new CartItem
                {
                    CartItemID = 1,
                    CartID = 1,
                    ProductID = 1,
                    Quantity = 1,
                    Price = 25000.00m,
                    Created = seedDate
                },
                new CartItem
                {
                    CartItemID = 2,
                    CartID = 1,
                    ProductID = 3,
                    Quantity = 2,
                    Price = 599.00m,
                    Created = seedDate
                },
                new CartItem
                {
                    CartItemID = 3,
                    CartID = 2,
                    ProductID = 2,
                    Quantity = 1,
                    Price = 55000.00m,
                    Created = seedDate
                }
            );

            // Seed PaymentModes
            modelBuilder.Entity<PaymentMode>().HasData(
                new PaymentMode { PaymentModeID = 1, PaymentModeName = "Credit Card" },
                new PaymentMode { PaymentModeID = 2, PaymentModeName = "Debit Card" },
                new PaymentMode { PaymentModeID = 3, PaymentModeName = "UPI" },
                new PaymentMode { PaymentModeID = 4, PaymentModeName = "Cash on Delivery" },
                new PaymentMode { PaymentModeID = 5, PaymentModeName = "Net Banking" }
            );

            // Seed Orders
            modelBuilder.Entity<Order>().HasData(
                new Order
                {
                    OrderID = 1,
                    UserID = 2,
                    OrderNo = "ORD001",
                    OrderDate = seedDate.AddDays(-5),
                    DeliveryDate = seedDate.AddDays(2),
                    AddressID = 1,
                    TotalAmount = 26198.00m,
                    CouponDiscount = 200.00m,
                    NetAmount = 25998.00m,
                    Status = "Pending",
                    Created = seedDate.AddDays(-5)
                },
                new Order
                {
                    OrderID = 2,
                    UserID = 3,
                    OrderNo = "ORD002",
                    OrderDate = seedDate.AddDays(-3),
                    DeliveryDate = seedDate.AddDays(1),
                    AddressID = 3,
                    TotalAmount = 55000.00m,
                    CouponDiscount = null,
                    NetAmount = 55000.00m,
                    Status = "Pending",
                    Created = seedDate.AddDays(-3)
                }
            );

            // Seed OrderDetails
            modelBuilder.Entity<OrderDetail>().HasData(
                new OrderDetail
                {
                    OrderDetailID = 1,
                    OrderID = 1,
                    ProductID = 1,
                    AddressID = 1,
                    Quantity = 1,
                    Amount = 25000.00m,
                    Discount = 0.00m,
                    NetAmount = 25000.00m,
                    Created = seedDate.AddDays(-5)
                },
                new OrderDetail
                {
                    OrderDetailID = 2,
                    OrderID = 1,
                    ProductID = 3,
                    AddressID = 1,
                    Quantity = 2,
                    Amount = 1198.00m,
                    Discount = 0.00m,
                    NetAmount = 1198.00m,
                    Created = seedDate.AddDays(-5)
                },
                new OrderDetail
                {
                    OrderDetailID = 3,
                    OrderID = 2,
                    ProductID = 2,
                    AddressID = 3,
                    Quantity = 1,
                    Amount = 55000.00m,
                    Discount = 0.00m,
                    NetAmount = 55000.00m,
                    Created = seedDate.AddDays(-3)
                }
            );

            // Seed Payments
            modelBuilder.Entity<Payment>().HasData(
                new Payment
                {
                    PaymentID = 1,
                    OrderID = 1,
                    PaymentModeID = 3,
                    TotalPayment = 25998.00m,
                    PaymentReference = "UPI123456789",
                    PaymentStatus = "Completed",
                    TransactionID = "TXN001",
                    TransactionDate = seedDate.AddDays(-5),
                    Created = seedDate.AddDays(-5)
                },
                new Payment
                {
                    PaymentID = 2,
                    OrderID = 2,
                    PaymentModeID = 1,
                    TotalPayment = 55000.00m,
                    PaymentReference = "CC987654321",
                    PaymentStatus = "Completed",
                    TransactionID = "TXN002",
                    TransactionDate = seedDate.AddDays(-3),
                    Created = seedDate.AddDays(-3)
                }
            );

            modelBuilder.Entity<Favorite>()
                .HasIndex(f => new { f.UserID, f.ProductID })
                .IsUnique();
        }
    }



}



