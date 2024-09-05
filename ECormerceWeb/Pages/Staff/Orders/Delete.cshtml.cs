using DataAccess.Repository.IRepository;
using DataObject.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PizzaManagement.Pages.Staff.Orders
{
    public class DeleteModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [BindProperty]
        public Order Order { get; set; }
        public void OnGet(int id)
        {
            Order = _unitOfWork.Order.GetFirstOrDefault(u => u.OrderID == id, includeProperties: "Accounts,Customer");

        }
        public async Task<IActionResult> OnPost()
        {
            var objFromDb = _unitOfWork.Order.GetFirstOrDefault(u => u.OrderID == Order.OrderID);
            if (objFromDb != null)
            {
                _unitOfWork.Order.Remove(objFromDb);
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
