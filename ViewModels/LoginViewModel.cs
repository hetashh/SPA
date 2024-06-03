using System.ComponentModel.DataAnnotations;

namespace SPA.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        [StringLength(50)]
        public string UserName { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 4)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
