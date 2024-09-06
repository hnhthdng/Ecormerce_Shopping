using DataAccess.Repository.IRepository;
using DataObject.Model;
using DataObject.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using Utility;

namespace PizzaManagement.Pages
{
    public class CheckoutModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CheckoutModel(IHttpContextAccessor httpContextAccessor, IUnitOfWork unitOfWork)
        {
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork = unitOfWork;
        }

        public List<CartItem> CartItems { get; private set; }
        public IEnumerable<Customer> Customers { get; set; }
        [BindProperty]
        public decimal CartTotal { get; private set; }
        [BindProperty]
        public int? SelectedCustomerId { get; set; }
        [BindProperty]
        public Customer NewCustomer { get; set; }

        public async Task OnGet()
        {
            LoadCartData();
        }

        public async Task<IActionResult> OnPost()
        {
            LoadCartData();
            //Create new order for this user
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (SelectedCustomerId.HasValue)
            {
                Customer customer = _unitOfWork.Customer.Get(SelectedCustomerId.Value);
                Order order = new Order()
                {
                    CustomerID = SelectedCustomerId.Value,
                    AccountId = claim.Value,
                    OrderDate = DateTime.Now,
                    RequiredDate = DateTime.Now.AddDays(3),
                    ShippedDate = null,
                    Freight = CartTotal,
                    ShipAddress = customer.Address
                };
                _unitOfWork.Order.Add(order);
                _unitOfWork.Save();

                foreach (var item in CartItems)
                {
                    OrderDetail orderDetail = new OrderDetail
                    {
                        OrderID = order.OrderID,
                        ProductID = item.ProductID,
                        UnitPrice = item.UnitPrice,
                        Quantity = item.Quantity
                    };
                    _unitOfWork.OrderDetail.Add(orderDetail);
                    _unitOfWork.Save();
                }
            }
            else if (NewCustomer != null)
            {
                NewCustomer.AccountId = claim.Value;
                _unitOfWork.Customer.Add(NewCustomer);
                _unitOfWork.Save();
                Order order = new Order()
                {
                    CustomerID = NewCustomer.CustomerID,
                    AccountId = claim.Value,
                    OrderDate = DateTime.Now,
                    RequiredDate = DateTime.Now.AddDays(3),
                    Freight = CartTotal,
                    ShippedDate = null,
                    ShipAddress = NewCustomer.Address
                };
                _unitOfWork.Order.Add(order);
                _unitOfWork.Save();

                foreach (var item in CartItems)
                {
                    OrderDetail orderDetail = new OrderDetail()
                    {
                        OrderID = order.OrderID,
                        ProductID = item.ProductID,
                        UnitPrice = item.UnitPrice,
                        Quantity = item.Quantity
                    };
                    _unitOfWork.OrderDetail.Add(orderDetail);
                    _unitOfWork.Save();
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Please select or create a customer.");
                return Page();
            }
            foreach (var item in CartItems)
            {
                Product product = _unitOfWork.Product.GetFirstOrDefault(u => u.ProductID == item.ProductID);
                product.UnitsInStock -= item.Quantity;
                _unitOfWork.Product.Update(product);
                _unitOfWork.Save();
            }
            _httpContextAccessor.HttpContext.Session.SetCart(new List<CartItem>());
            // Redirect to a confirmation page or order summary
            return RedirectToPage("/OrderConfirmation");
        }

        public IActionResult OnPostRemoveFromCart(int id)
        {
            CartItems = _httpContextAccessor.HttpContext.Session.GetCart();
            // Find the cart item by ProductID
            var cartItem = CartItems.Where(item => item.ProductID == id).FirstOrDefault();

            if (cartItem != null)
            {
                // Remove the item from the cart
                CartItems.Remove(cartItem);
                _httpContextAccessor.HttpContext.Session.SetCart(CartItems);
            }
            return RedirectToPage();
        }


        private async Task LoadCartData()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            Customers = _unitOfWork.Customer.GetAll(u => u.AccountId == claim.Value);
            CartItems = _httpContextAccessor.HttpContext.Session.GetCart();
            CartTotal = CartItems.Sum(item => item.UnitPrice * item.Quantity);
        }

        public async Task<IActionResult> OnPostUpdateQuantity([FromBody] CartUpdateViewModel cartUpdate)
        {
            // Get the current cart from the session
            var cartItems = _httpContextAccessor.HttpContext.Session.GetCart();

            // Find the cart item by ProductID
            var cartItem = cartItems.FirstOrDefault(item => item.ProductID == cartUpdate.ProductId);

            if (cartItem != null)
            {
                // Update the quantity of the cart item
                cartItem.Quantity = cartUpdate.Quantity;
                _httpContextAccessor.HttpContext.Session.SetCart(cartItems);

                // Recalculate the total
                CartTotal = cartItems.Sum(item => item.UnitPrice * item.Quantity);

                return new JsonResult(new { success = true, newTotal = CartTotal.ToString("C") });
            }
            return new JsonResult(new { success = false });
        }

        public class CartUpdateViewModel
        {
            public int ProductId { get; set; }
            public int Quantity { get; set; }
        }
    }

}