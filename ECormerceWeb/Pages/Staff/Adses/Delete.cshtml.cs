using DataAccess.Repository.IRepository;
using DataObject.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ECormerceWeb.Pages.Staff.Adses
{
    public class DeleteModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [BindProperty]
        public Ads Ads { get; set; }

        public void OnGet(int id)
        {
            Ads = _unitOfWork.Ads.GetFirstOrDefault(u => u.Id == id);
        }
        public async Task<IActionResult> OnPost()
        {
            var objFromDb = _unitOfWork.Ads.GetFirstOrDefault(u => u.Id == Ads.Id);
            if (objFromDb != null)
            {
                _unitOfWork.Ads.Remove(objFromDb);
                _unitOfWork.Save();
                RedirectToPage("Index");
            }
            else
            {
                return Page();
            }
            return RedirectToPage("Index");
        }
    }
}
