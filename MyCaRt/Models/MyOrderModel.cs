namespace MyCaRt.Models
{
    public class MyOrderModel
    {
        public int OrderId { get; set; }
        public string? Customer_FName {  get; set; }
        public string? Product_Name { get; set; }

        public int Quantity { get; set; }

        public decimal? TotalPrice { get; set; }

        public DateTime? OrderDate { get; set; }

        public string? IsPaid { get; set; }
    }
}
