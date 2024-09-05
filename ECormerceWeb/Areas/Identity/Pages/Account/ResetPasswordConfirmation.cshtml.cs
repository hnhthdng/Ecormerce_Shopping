using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PizzaManagement.Areas.Identity.Pages.Account
{
    public class ResetPasswordConfirmationModel : PageModel
    {
        [AllowAnonymous]
        public void OnGet()
        {
        }
    }
}
