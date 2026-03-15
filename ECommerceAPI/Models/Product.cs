using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerceAPI.Models
{
    public class Product
    {
        [Key]
        public int ProductID { get; set; }

        [Required]
        [StringLength(200)]
        public string ProductName { get; set; }

        [Required]
        [StringLength(50)]
        public string ProductCode { get; set; }

        [Required]
        public int CategoryID { get; set; }

        [ForeignKey(nameof(CategoryID))]
        public Category Category { get; set; }

        public int? UserID { get; set; }

        [ForeignKey(nameof(UserID))]
        public User User { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [StringLength(300)]
        public string Image { get; set; }

        [Required]
        public int StockQuantity { get; set; }

        [Required]
        public bool IsActive { get; set; }

        [Required]
        public DateTime Created { get; set; }

        public DateTime? Modified { get; set; }

        public ICollection<CartItem> CartItems { get; set; }
        public ICollection<OrderDetail> OrderDetails { get; set; }
        public ICollection<Favorite> Favorites { get; set; }
        public ICollection<ProductImage> ProductImages { get; set; }
        public ICollection<ProductReview> ProductReviews { get; set; }
    }

    public class ProductDTO
    {
        public int ProductID { get; set; }

        public string? ProductName { get; set; }

        public string? ProductCode { get; set; }

        public int CategoryID { get; set; }

        public int? UserID { get; set; }

        public decimal Price { get; set; }

        public int StockQuantity { get; set; }

        public string? Image { get; set; }

        public IFormFile? DocumentFile { get; set; }

        public List<IFormFile>? AdditionalImages { get; set; }

        public bool IsActive { get; set; }

        public List<IFormFile>? Files { get; set; }
        public List<int>? ImagesToDelete { get; set; }
    }
}
