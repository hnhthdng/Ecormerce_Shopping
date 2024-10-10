using DataAccess.Repository.IRepository;
using DataObject.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ECormerceWeb.Pages
{
    public class ProductDetailModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductDetailModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [BindProperty]
        public int OrderID { get; set; }
        public Order Order { get; set; }
        public IEnumerable<OrderDetail> OrderDetail { get; set; }
        public void OnGet(int id)
        {
            this.OrderID = id;
            Order = _unitOfWork.Order.GetFirstOrDefault(u => u.OrderID == OrderID);
            OrderDetail = _unitOfWork.OrderDetail.GetAll(u => u.OrderID == OrderID, includeProperty: "Product");

        }
    }
}
