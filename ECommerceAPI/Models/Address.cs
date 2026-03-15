using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerceAPI.Models
{
    public class Address
    {
        [Key]
        public int AddressID { get; set; }

        [Required]
        public int UserID { get; set; }

        [ForeignKey(nameof(UserID))]
        public User User { get; set; }

        [Required]
        [StringLength(100)]
        public string ReceiverName { get; set; }

        [Required]
        [StringLength(15)]
        public string Phone { get; set; }

        [Required]
        [StringLength(200)]
        public string AddressLine1 { get; set; }

        //[StringLength(200)]
        //public string AddressLine2 { get; set; }

        [StringLength(150)]
        public string Landmark { get; set; }

        [Required]
        [StringLength(100)]
        public string City { get; set; }

        [Required]
        [StringLength(100)]
        public string State { get; set; }

        [Required]
        [StringLength(100)]
        public string Country { get; set; }

        [Required]
        [StringLength(10)]
        public string Pincode { get; set; }

        [Required]
        public bool IsDefault { get; set; }

        [Required]
        public DateTime Created { get; set; }

        public DateTime? Modified { get; set; }
    }

    public class AddressDTO
    {
        public int AddressID { get; set; }

        public int UserID { get; set; }

        public string ReceiverName { get; set; }

        public string Phone { get; set; }

        public string AddressLine1 { get; set; }

        //[StringLength(200)]
        //public string AddressLine2 { get; set; }

        public string Landmark { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Country { get; set; }

        public string Pincode { get; set; }

        public bool IsDefault { get; set; }
    }
}
