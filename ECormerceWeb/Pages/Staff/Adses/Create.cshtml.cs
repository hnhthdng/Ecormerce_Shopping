using DataAccess.Repository.IRepository;
using DataObject.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ECormerceWeb.Pages.Staff.Adses
{
    public class CreateModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        public CreateModel(IUnitOfWork unitOfWork) {
            _unitOfWork = unitOfWork;
        }
        [BindProperty]
        public Ads Ads { get; set; }

        public void OnGet()
        {
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            if (Ads.StartDate > Ads.EndDate)
            {
                ModelState.AddModelError("Ads.EndDate", "End Date must be greater than Start Date");
                return Page();
            }

            _unitOfWork.Ads.Add(Ads);
            _unitOfWork.Save();
            return RedirectToPage("./Index");
        }
    }
}
