using System.ComponentModel.DataAnnotations;
using Fantasy.Shared.Resources;

namespace Fantasy.Shared.DTOs;

public class LoginDTO
{
    [Display(Name = "UserEmail", ResourceType = typeof(Literals))]
    [Required(ErrorMessageResourceName = "IURequiredField", ErrorMessageResourceType = typeof(Literals))]
    [EmailAddress(ErrorMessageResourceName = "UserValidEmail", ErrorMessageResourceType = typeof(Literals))]
    public string Email { get; set; } = null!;

    [Display(Name = "UserPassword", ResourceType = typeof(Literals))]
    [Required(ErrorMessageResourceName = "IURequiredField", ErrorMessageResourceType = typeof(Literals))]
    [MinLength(6, ErrorMessageResourceName = "IUMinLength", ErrorMessageResourceType = typeof(Literals))]
    public string Password { get; set; } = null!;
}