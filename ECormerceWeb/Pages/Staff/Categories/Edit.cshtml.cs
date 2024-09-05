using DataAccess.Repository.IRepository;
using DataObject.Model;
using DataObject.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PizzaManagement.Pages.Staff.Categories
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
        public Category Category { get; set; }

        public async Task<IActionResult> OnGet(int id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Category = _unitOfWork.Category.GetFirstOrDefault(u => u.CategoryID == id);
            
            this.id = id;
            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            var category = _unitOfWork.Category.Get(id);
            if (category == null)
            {
                return NotFound();
            }
            category.CategoryName = Category.CategoryName;
            category.Description = Category.Description;

            _unitOfWork.Category.Update(category);
            _unitOfWork.Save();

            return RedirectToPage("./Index");
        }

    }
}
