using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;

namespace ECommerceAPI.Models
{
    public class User
    {
        [Key]
        public int UserID { get; set; }

        [Required]
        [StringLength(100)]
        public string UserName { get; set; }

        [StringLength(300)]
        public string Address { get; set; }

        [Required]
        [StringLength(15)]
        public string Phone { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(150)]
        public string Email { get; set; }

        [Required]
        [StringLength(150)]
        public string Password { get; set; }

        // FK
        [Required]
        public int UserTypeID { get; set; }

        // Navigation
        [ForeignKey(nameof(UserTypeID))]
        public UserType UserType { get; set; }

        [Required]
        public bool IsActive { get; set; }

        public Cart Cart { get; set; }


        [Required]
        public DateTime Created { get; set; }

        public DateTime? Modified { get; set; }

        // Navigation collections
        public ICollection<Address> Addresses { get; set; }
        public ICollection<Category> Categories { get; set; }
        public ICollection<Product> Products { get; set; }
        public ICollection<Order> Orders { get; set; }
        public ICollection<ProductReview> ProductReviews { get; set; }
        public ICollection<Payment> Payments { get; set; }
        public ICollection<Favorite> Favorites { get; set; }
    }

    public class UserDTO
    {
        public int UserID { get; set; }

        
        public string UserName { get; set; }

        public string Address { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public int UserTypeID { get; set; }

        public bool IsActive { get; set; }
    }
}
