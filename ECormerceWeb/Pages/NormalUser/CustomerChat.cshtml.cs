using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ECormerceWeb.Pages.NormalUser
{
    [Authorize(Roles = "NormalUser")]

    public class CustomerChatModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
