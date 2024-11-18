using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ECommerceApp.Models;
using ECommerceApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace ECommerceApp.Pages
{
    [Authorize(Roles = "Client")]
    public class IndexModel : PageModel
    {
        private readonly Data.ECommerceAppContext _context;

        private readonly CartService _cartService;

        public IndexModel(Data.ECommerceAppContext context, CartService cartService)
        {
            _context = context;
            _cartService = cartService;
        }
        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; } = string.Empty;
        public IList<Product> Product { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Product = await _context.Product.ToListAsync();

            // Adjust quantities based on the session cart
            var cart = _cartService.GetCart();

            foreach (var item in Product)
            {
                // Check if the cart contains an item with the same product ID
                var cartItem = cart.FirstOrDefault(ci => ci.Product.Id == item.Id);
                if (cartItem != null)
                {
                    item.Quantity -= cartItem.Quantity; // Adjust quantity based on the cart
                }
            }
        }

        public IActionResult OnPostAddToCart(int id, int quantity)
        {
            var product = _context.Product.Find(id);
            if (product != null && product.Quantity >= quantity)
            {
                _cartService.AddToCart(product, quantity);
            }

            OnGetAsync().Wait();

            // Instead of redirecting to refresh the database, return the same page.
            return Page();
        }

        public async Task<IActionResult> OnPostSearchAsync()
        {
            if (string.IsNullOrEmpty(SearchTerm))
            {
                Product = await _context.Product.ToListAsync();
            }
            else
            {
                string normalizedSearchTerm = SearchTerm.Replace(" ", "").ToLower();

                Product = await _context.Product
                    .Where(p => p.Name.Replace(" ", "").ToLower().Contains(normalizedSearchTerm) ||
                                p.Description.Replace(" ", "").ToLower().Contains(normalizedSearchTerm) 
                                //||
                                //p.Price.ToString().Replace(" ", "").ToLower().Contains(normalizedSearchTerm)
                                )
                    .ToListAsync();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostLogout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToPage("/Admin/Index");
        }

    }
}
