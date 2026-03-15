using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerceAPI.Models
{
    public class ProductReview
    {
        [Key]
        public int ReviewID { get; set; }

        [Required]
        public int ProductID { get; set; }

        [ForeignKey(nameof(ProductID))]
        public Product Product { get; set; }

        [Required]
        public int UserID { get; set; }

        [ForeignKey(nameof(UserID))]
        public User User { get; set; }

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [Required]
        [StringLength(500)]
        public string Comment { get; set; }

        /// <summary>Optional review image URL (e.g. from Cloudinary).</summary>
        [StringLength(500)]
        public string? Image { get; set; }

        [Required]
        public DateTime Created { get; set; }

        public DateTime? Modified { get; set; }
    }

    public class ProductReviewDTO
    {
        public int ReviewID { get; set; }

        [Required]
        public int ProductID { get; set; }

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Comment { get; set; }

        /// <summary>Optional review image URL.</summary>
        public string? Image { get; set; }
    }
}
