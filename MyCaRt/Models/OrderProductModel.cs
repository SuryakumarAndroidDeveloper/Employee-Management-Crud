namespace MyCaRt.Models
{
    public class OrderProductModel
    {
        public int Customer_Id { get; set; }
        public List<OrderProduct> OrderProducts { get; set; }
        
    }

    public class OrderProduct
    {
        public int Product_Id { get; set; }
        public int Quantity { get; set; }

        public decimal TotalPrice { get; set; }
    }
}
