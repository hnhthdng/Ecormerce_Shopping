using DataAccess.Repository.IRepository;
using DataObject.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ECormerceWeb.Pages.Staff.Adses
{
    public class EditModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        public EditModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [BindProperty]
        public int id { get; set; }
        [BindProperty]
        public Ads Ads { get; set; }
        public async Task<IActionResult> OnGet(int id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Ads = _unitOfWork.Ads.GetFirstOrDefault(u => u.Id == id);

            this.id = id;
            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            if(Ads.StartDate > Ads.EndDate)
            {
                ModelState.AddModelError("Ads.EndDate", "End Date must be greater than Start Date");
                return Page();
            }
            _unitOfWork.Ads.Update(Ads);
            _unitOfWork.Save();
            return RedirectToPage("./Index");
        }
    }
}
