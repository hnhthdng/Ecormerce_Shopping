using DataAccess.Repository;
using DataAccess.Repository.IRepository;
using DataObject.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PizzaManagement.Pages.Staff.Orders
{
    public class IndexModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        public IndexModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [BindProperty(SupportsGet = true)]
        public DateOnly? StartDate { get; set; }
        [BindProperty(SupportsGet = true)]
        public DateOnly? EndDate { get; set; }


        public IEnumerable<Order> Orders { get; set; }

        public void OnGet()
        {
            Orders = _unitOfWork.Order.GetAll(orderBy: u => u.OrderBy(c => c.OrderDate), includeProperty: "Accounts,Customer");

            if (StartDate.HasValue)
            {
                Orders = Orders.Where(o => DateOnly.FromDateTime(o.OrderDate) >= StartDate.Value).ToList();
            }

            if (EndDate.HasValue)
            {
                Orders = Orders.Where(o => DateOnly.FromDateTime(o.OrderDate) <= EndDate.Value).ToList();
            }
        }
    }
}
