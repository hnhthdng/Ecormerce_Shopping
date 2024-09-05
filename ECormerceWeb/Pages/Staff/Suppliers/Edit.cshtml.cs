using DataAccess.Repository.IRepository;
using DataObject.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PizzaManagement.Pages.Staff.Suppliers
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
        public Supplier Supplier { get; set; }

        public async Task<IActionResult> OnGet(int id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Supplier = _unitOfWork.Supplier.GetFirstOrDefault(u => u.SupplierID == id);

            this.id = id;
            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            var supplier = _unitOfWork.Supplier.Get(id);
            if (supplier == null)
            {
                return NotFound();
            }
            supplier.CompanyName = Supplier.CompanyName;
            supplier.Address = Supplier.Address;
            supplier.Phone = Supplier.Phone;

            _unitOfWork.Supplier.Update(supplier);
            _unitOfWork.Save();

            return RedirectToPage("./Index");
        }

    }
}
