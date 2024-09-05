using DataAccess.Repository.IRepository;
using DataObject.Model;
using DataObject.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PizzaManagement.Pages.Staff.Categories
{
    public class CreateModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        public CreateModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

        }
        [BindProperty]
        public Category Category { get; set; }

        public void OnGet()
        {
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var cate = new Category
                {
                    CategoryName = Category.CategoryName,
                    Description = Category.Description
                };  
                
                if(_unitOfWork.Category.GetAll().Any(c => c.CategoryName == Category.CategoryName))
                {
                    ModelState.AddModelError("","Category already exists");
                    return Page();
                }

                _unitOfWork.Category.Add(cate);
                _unitOfWork.Save();
                
                return RedirectToPage("./Index");
            }

            return Page();
        }

    }
}
