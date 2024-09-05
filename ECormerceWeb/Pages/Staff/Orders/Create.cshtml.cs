using DataAccess.Repository.IRepository;
using DataObject.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace PizzaManagement.Pages.Staff.Orders
{
    public class CreateModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        public CreateModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

        }
        [BindProperty]
        public Order Order { get; set; }

        [BindProperty]
        public Customer Customer { get; set; }

        public IEnumerable<SelectListItem> AccountsList { get; set; }
        public void OnGet()
        {
            AccountsList = _unitOfWork.Account.GetAll().Select(i => new SelectListItem
            {
                Text = i.Email,
                Value = i.Id
            });
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            try
            {
                var customer = new Customer
                {
                    AccountId = claim.Value,
                    CustomerID = Customer.CustomerID,
                    Password = "00000",
                    ContactName = Customer.ContactName,
                    Address = Customer.Address,
                    Phone = Customer.Phone
                };
                _unitOfWork.Customer.Add(customer);
                _unitOfWork.Save();

                //Add Order for the account
                var order = new Order
                {
                    AccountId = Order.AccountId,
                    CustomerID = customer.CustomerID,
                    OrderDate = DateTime.Now,
                    RequiredDate = Order.RequiredDate,
                    ShippedDate = null,
                    Freight = 0,
                    ShipAddress = customer.Address
                };
                _unitOfWork.Order.Add(order);
                _unitOfWork.Save();

                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
