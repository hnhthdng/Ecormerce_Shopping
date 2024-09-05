using DataAccess.Repository.IRepository;
using DataObject.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace PizzaManagement.Pages.Staff.Products
{
    public class CreateModel : PageModel
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IUnitOfWork _unitOfWork;
        public CreateModel(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _hostingEnvironment = webHostEnvironment;

        }
        [BindProperty]
        public Product Product { get; set; }

        public IEnumerable<SelectListItem> SupplierList { get; set; }
        public IEnumerable<SelectListItem> CategoryList { get; set; }
        public void OnGet()
        {
            CategoryList = _unitOfWork.Category.GetAll().Select(i => new SelectListItem
            {
                Text = i.CategoryName,
                Value = i.CategoryID.ToString()
            });
            SupplierList = _unitOfWork.Supplier.GetAll().Select(i => new SelectListItem
            {
                Text = i.CompanyName,
                Value = i.SupplierID.ToString()
            });
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            string webRootPath = _hostingEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;
            string fileName = Guid.NewGuid().ToString();
            var uploads = Path.Combine(webRootPath, @"images\products");
            var extension = Path.GetExtension(files[0].FileName);

            using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
            {
                files[0].CopyTo(fileStreams);
            }
            Product.ProductImageURL = @"\images\products\" + fileName + extension;
            _unitOfWork.Product.Add(Product);
            _unitOfWork.Save();
            return RedirectToPage("./Index");
        }

    }
}
