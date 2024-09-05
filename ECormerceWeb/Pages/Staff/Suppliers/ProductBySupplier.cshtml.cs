using DataAccess.Repository.IRepository;
using DataObject.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PizzaManagement.Pages.Staff.Suppliers
{
    public class ProductBySupplierModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductBySupplierModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [BindProperty]
        public int supplierId { get; set; }
        public Supplier Supplier { get; set; }
        public IEnumerable<Product> Products { get; set; } 
        public void OnGet(int supplierId)
        {
            this.supplierId = supplierId;
            Supplier = _unitOfWork.Supplier.GetFirstOrDefault(u => u.SupplierID == supplierId);
            Products = _unitOfWork.Product.GetAll(u => u.SupplierID == supplierId, includeProperty: "Category");
        }
    }
}
