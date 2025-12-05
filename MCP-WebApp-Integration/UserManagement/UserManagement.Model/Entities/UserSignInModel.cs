using System.ComponentModel.DataAnnotations;

namespace UserManagement.Model.Entities
{
    public class UserSignInModel
    {
        
        [Required(ErrorMessage = "EmailAddress is required")]
        [EmailAddress(ErrorMessage = "Invalid email address format")]
        [Display(Name = "Email Address")]
        public  string EmailAddress { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
        public  string Password { get; set; }

    }
}
