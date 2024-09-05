using DataAccess.Repository.IRepository;
using DataObject.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PizzaManagement.Pages.Staff.Categories
{
    public class ProductByCategoryModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductByCategoryModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [BindProperty]
        public int categoryId { get; set; }
        public Category Category { get; set; }
        public IEnumerable<Product> Products { get; set; }
        public void OnGet(int categoryId)
        {
            this.categoryId = categoryId;
            Category = _unitOfWork.Category.GetFirstOrDefault(u => u.CategoryID == categoryId);
            Products = _unitOfWork.Product.GetAll(u => u.CategoryID == categoryId, includeProperty:"Supplier");
        }
    }
}
