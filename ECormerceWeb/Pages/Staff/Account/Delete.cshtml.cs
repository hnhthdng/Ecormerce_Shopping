using DataAccess.Repository.IRepository;
using DataObject.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PizzaManagement.Pages.Staff.Account
{
    [BindProperties]
    public class DeleteModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<Accounts> _userManager;
        public DeleteModel(IUnitOfWork unitOfWork, UserManager<Accounts> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }
        public Accounts Account { get; set; }

        public void OnGet(string id)
        {
            Account = _unitOfWork.Account.GetFirstOrDefault(u => u.Id == id);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.FindByIdAsync(Account.Id);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                return RedirectToPage("Index"); // Redirect to a list or confirmation page after successful deletion
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return Page();
        }

    }
}
