using DataAccess.Repository.IRepository;
using DataObject.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PizzaManagement.Pages.Staff.Suppliers
{
    public class CreateModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        public CreateModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

        }
        [BindProperty]
        public Supplier Supplier { get; set; }

        public void OnGet()
        {
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var supplier = new Supplier
                {
                    CompanyName = Supplier.CompanyName,
                    Address = Supplier.Address,
                    Phone = Supplier.Phone,
                };

                if (_unitOfWork.Supplier.GetAll().Any(c => c.CompanyName == Supplier.CompanyName))
                {
                    ModelState.AddModelError("", "Supplier already exists");
                    return Page();
                }

                _unitOfWork.Supplier.Add(supplier);
                _unitOfWork.Save();

                return RedirectToPage("./Index");
            }

            return Page();
        }

    }
}
