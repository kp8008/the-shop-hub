using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ECommerceAPI.Models
{
    public class PaymentMode
    {
        [Key]
        public int PaymentModeID { get; set; }

        [Required]
        [StringLength(50)]
        public string PaymentModeName { get; set; }

        public ICollection<Payment> Payments { get; set; }
    }

    public class PaymentModeDTO
    {
        public int PaymentModeID { get; set; }

        public string PaymentModeName { get; set; }
    }
}
