using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataObject.Model
{
    public class Supplier
    {
        public int SupplierID { get; set; }
        [Display(Name = "Company Name")]
        public string CompanyName { get; set; }
        public string Address { get; set; }
        [Phone]
        public string Phone { get; set; }

        // Navigation properties
        [ValidateNever]
        public virtual ICollection<Product> Products { get; set; }
    }

}
