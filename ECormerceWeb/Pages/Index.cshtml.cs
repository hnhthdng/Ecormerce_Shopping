using DataAccess.Repository.IRepository;
using DataObject.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PizzaManagement.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        public IndexModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [BindProperty(SupportsGet = true)]
        public string SearchItem { get; set; }
        [BindProperty(SupportsGet = true)]
        public decimal? MinPrice { get; set; }
        [BindProperty(SupportsGet = true)]
        public decimal? MaxPrice { get; set; }
        [BindProperty(SupportsGet = true)]
        public int? CategoryId { get; set; }
        [BindProperty(SupportsGet = true)]
        public int? SupplierId { get; set; }

        public IEnumerable<Product> ProductList { get; set; }
        public IEnumerable<Category> CategoryList { get; set; }
        public IEnumerable<Supplier> SupplierList { get; set; }

        public void OnGet()
        {
            ProductList = _unitOfWork.Product.GetAll(includeProperty: "Category,Supplier");
            CategoryList = _unitOfWork.Category.GetAll(orderBy: u => u.OrderBy(c => c.CategoryName));
            SupplierList = _unitOfWork.Supplier.GetAll(orderBy: u => u.OrderBy(s => s.CompanyName));

            // Apply filters if any
            if (!string.IsNullOrEmpty(SearchItem))
            {
                ProductList = ProductList.Where(p => p.ProductName.Contains(SearchItem, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (MinPrice.HasValue)
            {
                ProductList = ProductList.Where(p => p.UnitPrice >= MinPrice.Value).ToList();
            }

            if (MaxPrice.HasValue)
            {
                ProductList = ProductList.Where(p => p.UnitPrice <= MaxPrice.Value).ToList();
            }

            if (CategoryId.HasValue)
            {
                ProductList = ProductList.Where(p => p.CategoryID == CategoryId.Value).ToList();
            }

            if (SupplierId.HasValue)
            {
                ProductList = ProductList.Where(p => p.SupplierID == SupplierId.Value).ToList();
            }
        }

    }
}
