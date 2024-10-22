using ECommerceApp.Models;
using Newtonsoft.Json;
using ECommerceApp.Models;

namespace ECommerceApp.Services
{
    public class CartService
    {
        private const string CartSessionKey = "Cart";

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly List<Product> _products; 

        public CartService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;

        }

        public List<CartItem> GetCart()
        {
            // get cart using the httpContextAccessor to access the Session
            var session = _httpContextAccessor.HttpContext.Session;
            // use the CartSessionKey
            var cartJson = session.GetString(CartSessionKey);
            // if the cart is null or empty, create a new one, else deserialize the object we got from the session
            return string.IsNullOrEmpty(cartJson)
                ? new List<CartItem>()
                : JsonConvert.DeserializeObject<List<CartItem>>(cartJson);
        }

        public Product GetProductById(int id)
        {
            return _products.FirstOrDefault(p => p.Id == id);
        }


        public void AddToCart(Product product, int quantity)
        {
            //if (product == null) return; // If the product does not exist, exit

            var cart = GetCart();
            var existingItem = cart.FirstOrDefault(i => i.Product.Id == product.Id);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity; // Increment quantity if the product is already in the cart
            }
            else
            {
                cart.Add(new CartItem { Product = product, Quantity = quantity });
            }

            SaveCart(cart);
        }

        public void RemoveFromCart(int productId)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(i => i.Product.Id == productId);

            if (item != null)
            {
                cart.Remove(item);
                SaveCart(cart);
            }
        }

        public decimal GetTotal()
        {
            // calculate total price
            var cart = GetCart();
            return cart.Sum(item => item.Product.Price * item.Quantity);
        }

        private void SaveCart(List<CartItem> cart)
        {
            // save the cart in the session using SerializeObject
            var session = _httpContextAccessor.HttpContext.Session;
            session.SetString(CartSessionKey, JsonConvert.SerializeObject(cart));
        }

        public void UpdateCart(int productId, int newQuantity)
        {
            var cart = GetCart();
            var cartItem = cart.FirstOrDefault(item => item.Product.Id == productId);
            if (cartItem != null)
            {
                cartItem.Quantity = newQuantity;
            }

            SaveCart(cart);
        }

    }
}
