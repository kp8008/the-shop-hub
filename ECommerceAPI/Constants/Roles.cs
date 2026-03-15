namespace ECommerceAPI.Constants
{
    public static class Roles
    {
        public const string Admin = "Admin";
        public const string Customer = "Customer";
        
        // Combined roles for convenience
        public const string AdminOnly = Admin;
        public const string CustomerOnly = Customer;
        public const string AdminOrCustomer = Admin + "," + Customer;
    }
}