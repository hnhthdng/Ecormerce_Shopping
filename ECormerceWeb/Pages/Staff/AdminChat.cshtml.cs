using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ECormerceWeb.Pages.Staff
{
    [Authorize(Roles = "Staff")]
    public class AdminChatModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
