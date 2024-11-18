using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace ECommerceApp.Pages.Login
{
    public class IndexModel : PageModel
    {

        public string ErrorMessage { get; set; }
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPost(string username, string password, string returnUrl = null)
        {
            string role = null;
            if (username == "admin" && password == "ilisi") // Admin credentials
            {
                role = "Admin";
            }
            else if (username == "client" && password == "ilisi") // Client credentials
            {
                role = "Client";
            }

            if (role != null)
            {
                // Create claims with a role
                var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role)
            };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                // Redirect based on role
                if (role == "Admin")
                {
                    return RedirectToPage("/Admin/Index");
                }
                else if (role == "Client")
                {
                    return RedirectToPage("/Index");
                }
            }

            // Invalid login attempt
            ErrorMessage = "Login ou Mot de passe incorrect.";
            return Page();
        }


        public async Task<IActionResult> OnPostLogout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToPage("/Admin/Index");
        }
    }
}
