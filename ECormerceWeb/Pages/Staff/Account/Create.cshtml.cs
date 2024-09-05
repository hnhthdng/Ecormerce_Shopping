using DataAccess.Repository.IRepository;
using DataObject.Model;
using DataObject.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PizzaManagement.Pages.Staff.Account
{
    public class CreateModel : PageModel
    {
        private readonly UserManager<Accounts> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        public CreateModel(IUnitOfWork unitOfWork, UserManager<Accounts> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;

        }
        [BindProperty]
        public RegisterVM Input { get; set; }

        public void OnGet()
        {
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = new Accounts {
                    Email = Input.Email,
                    FullName = Input.FullName,
                    PhoneNumber = Input.PhoneNumber,
                    Type = Input.Type
                };
                string role = Request.Form["rdUserRole"].ToString();
                user.Type = int.Parse(role);
                user.UserName = Input.Email;
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    if (role == "1")
                    {
                        await _userManager.AddToRoleAsync(user, "Staff");

                    }
                    else
                    {
                        await _userManager.AddToRoleAsync(user, "NormalUser");
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return RedirectToPage("./Index");
            }

            return Page();
        }

    }
}
