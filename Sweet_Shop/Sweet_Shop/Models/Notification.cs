namespace Sweet_Shop.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public string productId { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; }
        public string CustomerId { get; set; }

    }
}
