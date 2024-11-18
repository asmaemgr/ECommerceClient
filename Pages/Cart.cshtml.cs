using ECommerceApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ECommerceApp.Pages
{
    [Authorize(Roles = "Client")]
    public class CartModel : PageModel
    {
        private readonly CartService _cartService;

        public CartModel(CartService cartService)
        {
            _cartService = cartService;
        }

        public List<ECommerceApp.Models.CartItem> CartItems { get; set; }
        public decimal TotalPrice { get; set; }

        public void OnGet()
        {
            CartItems = _cartService.GetCart();
            TotalPrice = _cartService.GetTotal();
        }

        public IActionResult OnPostUpdateQuantity(int productId, string action, int quantity)
        {
            int newquantity = quantity;
            if (action == "add")
            {
                newquantity++;
            }
            else if (action == "subtract" && quantity > 1)
            {
                newquantity--;
            }
            _cartService.UpdateCart(productId, newquantity);
            return RedirectToPage();
        }

        public IActionResult OnPostRemoveFromCart(int id)
        {
            _cartService.RemoveFromCart(id);
            return RedirectToPage();
        }
    }
}
