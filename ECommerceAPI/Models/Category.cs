using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerceAPI.Models
{
    public class Category
    {
        [Key]
        public int CategoryID { get; set; }

        [Required]
        [StringLength(100)]
        public string CategoryName { get; set; }

        public int? UserID { get; set; }

        [ForeignKey(nameof(UserID))]
        public User User { get; set; }

        [Required]
        public bool IsActive { get; set; }

        [Required]
        public DateTime Created { get; set; }

        public DateTime? Modified { get; set; }

        public ICollection<Product> Products { get; set; }
    }

    public class CategoryDTO
    {
        public int CategoryID { get; set; }

        public string? CategoryName { get; set; }

        public int? UserID { get; set; }

        public bool IsActive { get; set; }
    }
}
