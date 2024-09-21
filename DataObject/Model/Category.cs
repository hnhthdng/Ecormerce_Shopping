

using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace DataObject.Model
{
    public class Category
    {
        public int CategoryID { get; set; }
        [Display(Name = "Category Name")]
        public string CategoryName { get; set; }
        public string? Description { get; set; }

        // Navigation properties
        [ValidateNever]
        public  virtual ICollection<Product> Products { get; set; }
    }

}
