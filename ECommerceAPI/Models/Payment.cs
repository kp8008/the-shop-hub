using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerceAPI.Models
{
    public class Payment
    {
        [Key]
        public int PaymentID { get; set; }

        [Required]
        public int OrderID { get; set; }

        [ForeignKey(nameof(OrderID))]
        public Order Order { get; set; }

        [Required]
        public int PaymentModeID { get; set; }

        [ForeignKey(nameof(PaymentModeID))]
        public PaymentMode PaymentMode { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPayment { get; set; }

        [StringLength(100)]
        public string PaymentReference { get; set; }

        [Required]
        [StringLength(50)]
        public string PaymentStatus { get; set; }

        [StringLength(100)]
        public string TransactionID { get; set; }

        [Required]
        public DateTime TransactionDate { get; set; }

        [Required]
        public DateTime Created { get; set; }

        public DateTime? Modified { get; set; }
    }

    public class PaymentDTO
    {
        public int PaymentID { get; set; }

        public int OrderID { get; set; }

        public int PaymentModeID { get; set; }

        public decimal TotalPayment { get; set; }

        public string? PaymentReference { get; set; }

        public string? PaymentStatus { get; set; }

        public string? TransactionID { get; set; }
    }
}
