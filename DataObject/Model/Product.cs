using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataObject.Model
{
    public class Product
    {
        public int ProductID { get; set; }
        [Display(Name = "Product Name")]
        public string ProductName { get; set; }
        public int SupplierID { get; set; }
        public int CategoryID { get; set; }
        [Display(Name = "Quantity Per Unit")]
        public string QuantityPerUnit { get; set; }
        [Display(Name ="Unit Price")]
        public decimal UnitPrice { get; set; }
        [Display(Name = "Product Image")]
        [ValidateNever]
        public string ProductImageURL { get; set; }
        [Display(Name = "Units In Stock")]
        public int UnitsInStock { get; set; }


        // Navigation properties
        [ValidateNever]
        public virtual Supplier Supplier { get; set; }
        [ValidateNever]
        public virtual Category Category { get; set; }
        [ValidateNever]
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }

}
