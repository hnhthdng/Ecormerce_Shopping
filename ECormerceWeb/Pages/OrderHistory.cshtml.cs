using DataAccess.Repository.IRepository;
using DataObject.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace PizzaManagement.Pages
{
    [Authorize]
    public class OrderHistoryModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailSender _emailSender;

        public OrderHistoryModel(IUnitOfWork unitOfWork, IEmailSender emailSender)
        {
            _unitOfWork = unitOfWork;
            _emailSender = emailSender;
        }

        public IEnumerable<Order> Orders { get; set; }

        public void OnGet()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            Orders = _unitOfWork.Order.GetAll(o => o.AccountId == claim.Value, includeProperty: "Customer");
        }

        public IActionResult OnPostReceiveOrder(int id)
        {
            var order = _unitOfWork.Order.GetFirstOrDefault(o => o.OrderID == id);

            if (order == null)
            {
                return NotFound();
            }

            // Update the ShippedDate to the current date
            order.ShippedDate = DateTime.Now;

            _unitOfWork.Order.Update(order);
            _unitOfWork.Save();
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.Email);

            _emailSender.SendEmailAsync(claim.Value, $"Order {order.OrderID} Shipped", $"Your order has been shipped. Wish you have the best time on shopping on our website !!!");
            // Redirect back to the OrderHistory page
            return RedirectToPage();
        }
    }
}
