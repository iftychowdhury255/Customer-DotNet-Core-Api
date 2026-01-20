namespace CustomerCoreApi.Models
{
    public class ProductDetails
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public Guid CustomerId { get; set; }
        public virtual Product? Product { get; set; }
        public virtual Customer? Customer { get; set; }
    }
}
