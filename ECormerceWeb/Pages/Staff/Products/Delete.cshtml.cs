using DataAccess.Repository.IRepository;
using DataObject.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PizzaManagement.Pages.Staff.Products
{
    public class DeleteModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [BindProperty]
        public Product Product { get; set; }
        public void OnGet(int id)
        {
            Product = _unitOfWork.Product.GetFirstOrDefault(u => u.ProductID == id);

        }
        public async Task<IActionResult> OnPost()
        {
            var objFromDb = _unitOfWork.Product.GetFirstOrDefault(u => u.ProductID == Product.ProductID);
            if (objFromDb != null)
            {
                _unitOfWork.Product.Remove(objFromDb);
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
