using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataObject.Model
{
    public class Accounts: IdentityUser
    {

        public string FullName { get; set; }
        public int Type { get; set; } // 1: Staff, 2: Normal User

        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<Customer> Customers{ get; set; }
    }

}
