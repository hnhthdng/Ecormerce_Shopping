using DataAccess.Repository.IRepository;
using DataObject.Model;
using DataObject.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Identity.Client;

namespace PizzaManagement.Pages.Staff.Account
{
    
    public class EditModel : PageModel
    {
        private readonly UserManager<Accounts> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        public EditModel(IUnitOfWork unitOfWork, UserManager<Accounts> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;

        }
        [BindProperty]
        public string id { get; set; }
        [BindProperty]
        public UpdateAccountVM Input { get; set; }
        public async Task<IActionResult> OnGet(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Accounts account = _unitOfWork.Account.GetFirstOrDefault(u => u.Id == id);
            Input = new UpdateAccountVM
            {
                Email = account.Email,
                FullName = account.FullName,
                PhoneNumber = account.PhoneNumber,
                Type = account.Type
            };
            this.id = id;
            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            var isExistEmail = _unitOfWork.Account.GetFirstOrDefault(u => u.Email == Input.Email && u.Id != id);
            if (isExistEmail != null)
            {
                ModelState.AddModelError(string.Empty, "Email already exist");
                return Page();
            }
            user.Email = Input.Email;
            user.FullName = Input.FullName;
            user.PhoneNumber = Input.PhoneNumber;

            var roleValue = Input.Type == 1 ? "Staff" : "NormalUser";
            user.Type = Input.Type;

            // Remove all current roles
            var currentRoles = await _userManager.GetRolesAsync(user);
            var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
            if (!removeResult.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Failed to remove user roles.");
                return Page();
            }

            // Add the new role
            var addRoleResult = await _userManager.AddToRoleAsync(user, roleValue);
            if (!addRoleResult.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Failed to add user to the new role.");
                return Page();
            }

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return RedirectToPage("Index"); // Redirect to a list or details page after successful update
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return Page();
        }
    }
}
