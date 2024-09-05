using DataAccess.Repository.IRepository;
using DataObject.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace PizzaManagement.Pages
{
    [Authorize]
    public class OrderHistoryModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderHistoryModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<Order> Orders { get; set; }

        public void OnGet()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            Orders = _unitOfWork.Order.GetAll(o => o.AccountId == claim.Value, includeProperty: "Customer");
        }
    }
}
