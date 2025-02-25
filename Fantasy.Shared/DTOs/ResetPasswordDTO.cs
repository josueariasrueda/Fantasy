using System.ComponentModel.DataAnnotations;
using Fantasy.Shared.Resources;

namespace Fantasy.Shared.DTOs;

public class ResetPasswordDTO
{
    [Display(Name = "UserEmail", ResourceType = typeof(Literals))]
    [Required(ErrorMessageResourceName = "IURequiredField", ErrorMessageResourceType = typeof(Literals))]
    [EmailAddress(ErrorMessageResourceName = "UserValidEmail", ErrorMessageResourceType = typeof(Literals))]
    public string Email { get; set; } = null!;

    [DataType(DataType.Password)]
    [Display(Name = "UserNewPassword", ResourceType = typeof(Literals))]
    [Required(ErrorMessageResourceName = "IURequiredField", ErrorMessageResourceType = typeof(Literals))]
    [StringLength(20, MinimumLength = 6, ErrorMessageResourceName = "IULengthField", ErrorMessageResourceType = typeof(Literals))]
    public string NewPassword { get; set; } = null!;

    [Compare("UserNewPassword", ErrorMessageResourceName = "UserPasswordAndConfirmationDifferent", ErrorMessageResourceType = typeof(Literals))]
    [Display(Name = "UserPasswordConfirm", ResourceType = typeof(Literals))]
    [Required(ErrorMessageResourceName = "IURequiredField", ErrorMessageResourceType = typeof(Literals))]
    [StringLength(20, MinimumLength = 6, ErrorMessageResourceName = "IULengthField", ErrorMessageResourceType = typeof(Literals))]
    public string ConfirmPassword { get; set; } = null!;

    public string Token { get; set; } = null!;
}