using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerceAPI.Models
{
    public class ProductImage
    {
        [Key]
        public int ProductImageID { get; set; }

        [Required]
        public int ProductID { get; set; }

        [ForeignKey("ProductID")]
        public Product Product { get; set; }

        [Required]
        public string ImageUrl { get; set; }

        public int DisplayOrder { get; set; }
    }
}
