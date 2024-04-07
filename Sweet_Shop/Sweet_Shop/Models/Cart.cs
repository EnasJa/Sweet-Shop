namespace Sweet_Shop.Models
{
    public class Cart
    {
        private List<CartItem> _cartItems = new List<CartItem>();

        public IEnumerable<CartItem> CartItems => _cartItems;

        public void AddToCart(Product product)
        {
            var existingCartItem = _cartItems.FirstOrDefault(c => c.Product.Id == product.Id);
            if (existingCartItem != null)
            {
                existingCartItem.Quantity++;
            }
            else
            {
                _cartItems.Add(new CartItem { Product = product, Quantity = 1 });
            }
        }

        public void RemoveFromCart(Product product)
        {
            _cartItems.RemoveAll(c => c.Product.Id == product.Id);
        }

        public float GetTotalAmount()
        {
            return _cartItems.Sum(c => c.Product.price * c.Quantity);
        }

        public void ClearCart()
        {
            _cartItems.Clear();
        }
    }
}
