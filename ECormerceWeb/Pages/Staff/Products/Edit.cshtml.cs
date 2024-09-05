using DataAccess.Repository.IRepository;
using DataObject.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace PizzaManagement.Pages.Staff.Products
{
    public class EditModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public EditModel(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _hostingEnvironment = webHostEnvironment;
        }
        [BindProperty]
        public int id { get; set; }
        [BindProperty]
        public Product Product { get; set; }
        public IEnumerable<SelectListItem> SupplierList { get; set; }
        public IEnumerable<SelectListItem> CategoryList { get; set; }
        public async Task<IActionResult> OnGet(int id)
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

            if (id == null)
            {
                return NotFound();
            }
            Product = _unitOfWork.Product.GetFirstOrDefault(u => u.ProductID == id);

            this.id = id;
            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            string webRootPath = _hostingEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;

            var objFromDb = _unitOfWork.Product.Get(Product.ProductID);
            if (files.Count > 0)
            {
                string fileName = Guid.NewGuid().ToString();
                var uploads = Path.Combine(webRootPath, @"images\products");
                var extension_new = Path.GetExtension(files[0].FileName);

                var imagePath = Path.Combine(webRootPath, objFromDb.ProductImageURL.TrimStart('\\'));
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }

                using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension_new), FileMode.Create))
                {
                    files[0].CopyTo(fileStreams);
                }
                Product.ProductImageURL = @"\images\products\" + fileName + extension_new;
            }
            else
            {
                Product.ProductImageURL = objFromDb.ProductImageURL;
            }
            _unitOfWork.Product.Update(Product);
            _unitOfWork.Save();
            return RedirectToPage("Index");

        }
    }
}
