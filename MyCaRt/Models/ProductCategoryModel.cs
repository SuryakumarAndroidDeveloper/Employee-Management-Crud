using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MyCaRt.Models
{
    public class ProductCategoryModel
    {

      [Key]
      public int Category_Id { get; set; }

      [Required]
      [DisplayName("Category_Name")]
      public string Category_Name { get; set; }

    }
}
