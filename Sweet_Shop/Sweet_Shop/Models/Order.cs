namespace Sweet_Shop.Models
{
    public class Order
    {
        public int OrdersId { get; set; }
        public int CustomerId { get; set; }
        public int productId { get; set; }
       public int Quantity {  get; set; }
        public DateTime OrderDate { get; set; } 
    }

}
