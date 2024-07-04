namespace MyCaRt.Models
{
    public class ProductViewModel
    {
        public List<ProductModel> Products { get; set; }
        public List<IFormFile> Files { get; set; }
        public ProductViewModel()
        {
            Products = new List<ProductModel>();
            Files = new List<IFormFile>();
        }
    }
}
