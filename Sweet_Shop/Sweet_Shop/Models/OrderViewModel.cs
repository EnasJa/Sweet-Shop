using Sweet_Shop.Models;

public class OrderViewModel
{
    public Cart Cart { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public string PaymentMethod { get; set; }
    // Add any other relevant properties
}