using System.ComponentModel.DataAnnotations;

namespace DevPlatform.ViewModels
{
    public class EditUserViewModel
    {
        [Required]
        [Display(Name = "Id")]
        public string Id { get; set; } = string.Empty;
        [Required(ErrorMessage = "Name is required!")]
        [Display(Name = "Name")]
        public string Name { get; set; } = string.Empty;
        [Required(ErrorMessage = "Name is required!")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;
    }
}
