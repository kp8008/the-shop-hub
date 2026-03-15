using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ECommerceAPI.Models
{
    public class UserType
    {
        [Key]
        public int UserTypeID { get; set; }

        [Required]
        [StringLength(50)]
        public string UserTypeName { get; set; }

        [Required]
        public DateTime Created { get; set; }

        public DateTime? Modified { get; set; }

        // Navigation
        public ICollection<User> Users { get; set; }
    }

    public class UserTypeDTO
    {
        public int UserTypeID { get; set; }

        
        public string UserTypeName { get; set; }
       
    }
}
