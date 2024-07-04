using System.ComponentModel.DataAnnotations;

namespace MyCaRt.Models
{
    public class ProductModel
    {
        [Key]
        public int? Product_Id { get; set; }

        [Required]
        public string? Product_Category { get; set; }

        [Required]
        public string? Product_Code { get; set; }
        
        [Required]
        public string? Product_Name { get; set; }

        [Required]
        public decimal? Product_Price { get; set; }

        [Required]
        public string? Product_Description { get; set; }

        [Required]
        public int? Available_Quantity { get; set; }

        
        public string? FilePath { get; set; }
        public string? ImageName { get; set; }

    }
}
