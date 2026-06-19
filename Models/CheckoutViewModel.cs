using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class CheckoutViewModel
    {
        [Required(ErrorMessage = "Name is required")]
        [Display(Name = "Full Name")]
        public string CustomerName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Enter a valid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Shipping address is required")]
        [Display(Name = "Shipping Address")]
        public string ShippingAddress { get; set; }
    }
}
