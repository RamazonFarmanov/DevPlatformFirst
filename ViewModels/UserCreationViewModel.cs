using System.ComponentModel.DataAnnotations;

namespace DevPlatform.ViewModels
{
    public class UserCreationViewModel
    {
        [Required(ErrorMessage = "Name is required!")]
        [Display(Name = "Name")]
        public string Name { get; set; } = string.Empty;
        [Required(ErrorMessage = "Email is required!")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;
        [Required(ErrorMessage = "Password is required!")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;
    }
}
