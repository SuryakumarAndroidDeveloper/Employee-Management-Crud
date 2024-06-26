using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MyCaRt.Models
{
    public class ProductCategoryModel
    {

      [Key]
      public int? Category_Id { get; set; }

      [Required]
      [DisplayName("Category_Name")]
      [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Category name must only contain letters.")]
        public string? Category_Name { get; set; }

    }
}
