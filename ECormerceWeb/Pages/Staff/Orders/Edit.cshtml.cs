using DataAccess.Repository.IRepository;
using DataObject.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace PizzaManagement.Pages.Staff.Orders
{
    public class EditModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        public EditModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

        }
        [BindProperty]
        public Order Order { get; set; }

        public void OnGet(int id)
        {
            Order = _unitOfWork.Order.GetFirstOrDefault(u => u.OrderID == id, includeProperties: "Accounts,Customer");
        }

        public IActionResult OnPost()
        {
            Order orderFromDb = _unitOfWork.Order.Get(Order.OrderID);
            orderFromDb.RequiredDate = Order.RequiredDate;
            orderFromDb.ShippedDate = Order.ShippedDate;
            orderFromDb.Freight = Order.Freight;
            orderFromDb.ShipAddress = Order.ShipAddress;
            _unitOfWork.Order.Update(orderFromDb);
            _unitOfWork.Save();
            return RedirectToPage("./Index");
        }
    }
}
