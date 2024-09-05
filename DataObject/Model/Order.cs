using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataObject.Model
{
    public class Order
    {
        public int OrderID { get; set; }
        public int CustomerID { get; set; }
        public string AccountId { get; set; }
        [Display(Name = "Order Date")]
        public DateTime OrderDate { get; set; }
        [Display(Name = "Required Date")]
        public DateTime RequiredDate { get; set; }
        [Display(Name = "Shipped Date")]
        public DateTime? ShippedDate { get; set; }
        public decimal Freight { get; set; }
        [Display(Name = "Ship Name")]
        public string ShipAddress { get; set; }

        // Navigation properties
        [ValidateNever]
        public virtual Customer Customer { get; set; }
        [ValidateNever]
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        [ValidateNever]
        public virtual Accounts Accounts { get; set; }
    }

}
