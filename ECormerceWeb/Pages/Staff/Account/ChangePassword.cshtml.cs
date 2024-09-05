using DataObject.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PizzaManagement.Pages.Staff.Account
{
    public class ChangePasswordModel : PageModel
    {
        private readonly UserManager<Accounts> _userManager;

        public ChangePasswordModel(UserManager<Accounts> userManager)
        {
            _userManager = userManager;
        }
        [BindProperty]
        public string id { get; set; }
        [BindProperty]
        public string NewPassword { get; set; }
        public void OnGet(string id)
        {
            this.id = id;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, resetToken, NewPassword);

            if (result.Succeeded)
            {
                return RedirectToPage("/Index", new { message = "Password reset successfully" });
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return Page();
        }
    }
}
