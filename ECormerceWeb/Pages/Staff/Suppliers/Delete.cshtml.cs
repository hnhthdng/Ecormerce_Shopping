using DataAccess.Repository.IRepository;
using DataObject.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PizzaManagement.Pages.Staff.Suppliers
{
    public class DeleteModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [BindProperty]
        public Supplier Supplier { get; set; }
        public void OnGet(int id)
        {
            Supplier = _unitOfWork.Supplier.GetFirstOrDefault(u => u.SupplierID == id);

        }
        public async Task<IActionResult> OnPost()
        {
            var objFromDb = _unitOfWork.Supplier.GetFirstOrDefault(u => u.SupplierID == Supplier.SupplierID);
            if (objFromDb != null)
            {
                _unitOfWork.Supplier.Remove(objFromDb);
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
