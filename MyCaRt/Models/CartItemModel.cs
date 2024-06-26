using System.ComponentModel.DataAnnotations;

namespace MyCaRt.Models
{
    public class CartItemModel
    {
            [Key]
            public int CartItem_Id { get; set; }

            [Required]
             public int? Product_Id { get; set; }

             public string? Customer_FName { get; set; }

             public int? Quantity { get; set; }



    }
}
