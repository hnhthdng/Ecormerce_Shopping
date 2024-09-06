using DataAccess.Repository;
using DataAccess.Repository.IRepository;
using DataAccess.Service;
using DataAccess.Service.IService;
using DataObject.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PizzaManagement.Pages.Staff.Orders
{
    public class IndexModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOrderExportService _exportService;

        public IndexModel(IUnitOfWork unitOfWork, IOrderExportService exportService)
        {
            _unitOfWork = unitOfWork;
            _exportService = exportService;
        }

        [BindProperty(SupportsGet = true)]
        public DateOnly? StartDate { get; set; }
        [BindProperty(SupportsGet = true)]
        public DateOnly? EndDate { get; set; }
        [BindProperty(SupportsGet = true)]
        public string? Account { get; set; }
        [BindProperty(SupportsGet = true)]
        public int? OrderId { get; set; }
        [BindProperty]
        public IEnumerable<Order> Orders { get; set; }

        public async Task OnGetAsync()
        {
            Orders = await GetFilteredOrdersAsync(OrderId, Account, StartDate, EndDate);
        }

        public async Task<IActionResult> OnGetExportAsync()
        {
            Orders = await GetFilteredOrdersAsync(OrderId, Account, StartDate, EndDate);
            var orders = Orders; // Replace with actual data fetching logic

            // Call the export service to generate the Excel file
            var fileContent = await _exportService.ExportToExcel(orders);

            // Return the file for download
            string fileName = $"OrderList-{System.DateTime.Now:yyyyMMddHHmmss}.xlsx";
            return File(fileContent, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        

        private async Task<List<Order>> GetFilteredOrdersAsync(int? orderId, string account, DateOnly? startDate, DateOnly? endDate)
        {
            // Get all orders from the unit of work
            var orders = _unitOfWork.Order.GetAll(orderBy: u => u.OrderBy(c => c.OrderDate), includeProperty: "Accounts,Customer").ToList();

            // Apply filters
            if (orderId.HasValue)
            {
                orders = orders.Where(o => o.OrderID == orderId.Value).ToList();
            }
            if (!string.IsNullOrEmpty(account))
            {
                orders = orders.Where(o => o.Accounts.UserName.ToLower().Contains(account.ToLower())).ToList();
            }
            if (startDate.HasValue)
            {
                orders = orders.Where(o => DateOnly.FromDateTime(o.OrderDate) >= startDate.Value).ToList();
            }
            if (endDate.HasValue)
            {
                orders = orders.Where(o => DateOnly.FromDateTime(o.OrderDate) <= endDate.Value).ToList();
            }

            return await Task.FromResult(orders);
        }
    }
}
