using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerceAPI.Models
{
    [Table("Order")]
    public class Order
    {
        [Key]
        public int OrderID { get; set; }

        [Required]
        public int UserID { get; set; }

        [ForeignKey(nameof(UserID))]
        public User User { get; set; }

        [Required]
        [StringLength(50)]
        public string OrderNo { get; set; }

        [Required]
        public DateTime OrderDate { get; set; }

        public DateTime? DeliveryDate { get; set; }

        public int? AddressID { get; set; }

        [ForeignKey(nameof(AddressID))]
        public Address Address { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? CouponDiscount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? NetAmount { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; }

        [Required]
        public DateTime Created { get; set; }

        public DateTime? Modified { get; set; }

        public ICollection<OrderDetail> OrderDetails { get; set; }
        public Payment Payment { get; set; }
    }

    public class OrderDTO
    {
        public int OrderID { get; set; }

        public int UserID { get; set; }

        public string OrderNo { get; set; }

        public DateTime? DeliveryDate { get; set; }

        public int? AddressID { get; set; }

        public decimal TotalAmount { get; set; }

        public decimal? CouponDiscount { get; set; }

        public decimal? NetAmount { get; set; }

        public string Status { get; set; }
    }

    public class UpdateOrderStatusDTO
    {
        public string Status { get; set; }
    }
}
