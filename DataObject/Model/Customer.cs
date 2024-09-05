using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataObject.Model
{
    public class Customer
    {
        public int CustomerID { get; set; }
        public string AccountId { get; set; }   
        public string? Password { get; set; }
        [Display(Name = "Contact Name")]
        public string ContactName { get; set; }
        public string Address { get; set; }
        [Phone]
        public string Phone { get; set; }

        // Navigation properties
        [ValidateNever]
        public virtual ICollection<Order> Orders { get; set; }
        [ValidateNever]
        public virtual Accounts Accounts { get; set; }
    }

}
