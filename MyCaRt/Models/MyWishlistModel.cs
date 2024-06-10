namespace MyCaRt.Models
{
    public class MyWishlistModel
    {

        public int WishList_Id { get; set; }
        public int Product_Id { get; set; }
        public int Customer_Id { get; set; }
        public string? Customer_FName { get; set; }
        public string? Product_Category { get; set; }

        public string? Product_Code { get; set; }

       
        public string? Product_Name { get; set; }

      
        public int? Product_Price { get; set; }

        
        public string? Product_Description { get; set; }

    
        public int? Available_Quantity { get; set; }
    }
}
