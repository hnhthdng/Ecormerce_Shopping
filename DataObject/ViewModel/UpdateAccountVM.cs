using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataObject.ViewModel
{
    public class UpdateAccountVM
    {
        [Required]  
        [EmailAddress]
        public string Email { get; set; }
        [Display(Name = "Full Name")]
        public string FullName { get; set; }
        [Phone]
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }
        public int Type { get; set; }
    }
}
