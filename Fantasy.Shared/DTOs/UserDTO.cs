using System.ComponentModel.DataAnnotations;
using Fantasy.Shared.Entities;
using Fantasy.Shared.Resources;

namespace Fantasy.Shared.DTOs;

public class UserDTO : User
{
    [DataType(DataType.Password)]
    [Display(Name = "UserPassword", ResourceType = typeof(Literals))]
    [Required(ErrorMessageResourceName = "IURequiredField", ErrorMessageResourceType = typeof(Literals))]
    [StringLength(20, MinimumLength = 6, ErrorMessageResourceName = "IULengthField", ErrorMessageResourceType = typeof(Literals))]
    public string Password { get; set; } = null!;

    [Compare("UserPassword", ErrorMessageResourceName = "UserPasswordAndConfirmationDifferent", ErrorMessageResourceType = typeof(Literals))]
    [Display(Name = "UserPasswordConfirm", ResourceType = typeof(Literals))]
    [DataType(DataType.Password)]
    [Required(ErrorMessageResourceName = "IURequiredField", ErrorMessageResourceType = typeof(Literals))]
    [StringLength(20, MinimumLength = 6, ErrorMessageResourceName = "IULengthField", ErrorMessageResourceType = typeof(Literals))]
    public string PasswordConfirm { get; set; } = null!;

    public string Language { get; set; } = null!;

    public string LocalPathPhoto { get; set; } = null!;
}