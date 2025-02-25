using System.ComponentModel.DataAnnotations;
using Fantasy.Shared.Resources;

namespace Fantasy.Shared.DTOs;

public class EmailDTO
{
    [Display(Name = "UserEmail", ResourceType = typeof(Literals))]
    [Required(ErrorMessageResourceName = "IURequiredField", ErrorMessageResourceType = typeof(Literals))]
    [EmailAddress(ErrorMessageResourceName = "UserValidEmail", ErrorMessageResourceType = typeof(Literals))]
    public string Email { get; set; } = null!;

    public string Language { get; set; } = null!;
}