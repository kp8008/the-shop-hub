 using System;
 using System.ComponentModel.DataAnnotations;
 using System.ComponentModel.DataAnnotations.Schema;
 
 namespace ECommerceAPI.Models
 {
     public class Favorite
     {
         [Key]
         public int FavoriteID { get; set; }
 
         [Required]
         public int UserID { get; set; }
 
         [ForeignKey(nameof(UserID))]
         public User User { get; set; }
 
         [Required]
         public int ProductID { get; set; }
 
         [ForeignKey(nameof(ProductID))]
         public Product Product { get; set; }
 
         [Required]
         public DateTime Created { get; set; }
 
         public DateTime? Modified { get; set; }
     }
 
     public class FavoriteDTO
     {
         public int FavoriteID { get; set; }
         public int UserID { get; set; }
         public int ProductID { get; set; }
     }
 }
