namespace EnhanceClub.Domain.Entities
{
    public class Product
    {
        public int ProductSizeId { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
     // public decimal ProductPrice { get; set; }
     // based on our business logic, only product size has price so price is irrelevant here
        public string ProductCategory { get; set; }
      
    }
}
