using DataAccess.Repository.IRepository;
using DataObject.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ECormerceWeb.Pages.Staff.OrderDetails
{
    public class IndexModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        public IndexModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

        }
        [BindProperty]
        public IEnumerable<OrderDetail> OrderDetail { get; set; }
        public void OnGet(int id)
        {
            OrderDetail = _unitOfWork.OrderDetail.GetAll(u => u.OrderID == id, includeProperty:"Product").ToList();
        }
    }
}
