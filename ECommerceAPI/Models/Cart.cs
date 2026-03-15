using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerceAPI.Models
{
    public class Cart
    {
        [Key]
        public int CartID { get; set; }

        [Required]
        public int UserID { get; set; }

        [ForeignKey(nameof(UserID))]
        public User User { get; set; }

        [Required]
        public DateTime Created { get; set; }

        public DateTime? Modified { get; set; }

        public ICollection<CartItem> CartItems { get; set; }
    }

    public class CartDTO
    {
        public int CartID { get; set; }

        public int UserID { get; set; }
    }

    public class AddToCartDto
    {
        public int ProductID { get; set; }
        public int Quantity { get; set; }
    }


}
