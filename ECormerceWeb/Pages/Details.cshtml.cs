using DataAccess.Repository.IRepository;
using DataObject.Model;
using DataObject.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Utility;

namespace PizzaManagement.Pages
{
    [Authorize]
    public class DetailsModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISession _session;
        public DetailsModel(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _session = httpContextAccessor.HttpContext.Session;
        }
        [BindProperty]
        public Product Product { get; set; }
        public async Task<IActionResult> OnGetAsync(int id)
        {
            //    var claimsIdentity = (ClaimsIdentity)User.Identity;
            //    var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            Product = _unitOfWork.Product.GetFirstOrDefault(u => u.ProductID == id, includeProperties:"Category,Supplier");
            if (Product == null)
            {
                return NotFound();
            }

            return Page();
        }

        public IActionResult OnPostAddToCart(int id)
        {
            var cart = _session.GetCart();
            var cartItem = cart.FirstOrDefault(c => c.ProductID == id);

            if (cartItem != null)
            {
                cartItem.Quantity++;
            }
            else
            {
                var product = _unitOfWork.Product.Get(id);
                if (product != null)
                {
                    cart.Add(new CartItem
                    {
                        ProductID = product.ProductID,
                        ProductName = product.ProductName,
                        UnitPrice = product.UnitPrice,
                        Quantity = 1,
                        imageURL = product.ProductImageURL
                    });
                }
            }

            _session.SetCart(cart);

            return RedirectToPage("/Details", new { id });
        }
    }
}
