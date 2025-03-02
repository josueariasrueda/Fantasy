using System.ComponentModel.DataAnnotations;
using Fantasy.Shared.Entities.Domain;
using Fantasy.Shared.Enums;
using Fantasy.Shared.Resources;
using Microsoft.AspNetCore.Identity;
using static System.Net.Mime.MediaTypeNames;

namespace Fantasy.Shared.Entities.Infraestructure;

public class User : IdentityUser
{
    [Display(Name = "UserFirstName", ResourceType = typeof(Literals))]
    [MaxLength(50, ErrorMessageResourceName = "IUMaxLength", ErrorMessageResourceType = typeof(Literals))]
    [Required(ErrorMessageResourceName = "IURequiredField", ErrorMessageResourceType = typeof(Literals))]
    public string FirstName { get; set; } = null!;

    [Display(Name = "UserLastName", ResourceType = typeof(Literals))]
    [MaxLength(50, ErrorMessageResourceName = "IUMaxLength", ErrorMessageResourceType = typeof(Literals))]
    [Required(ErrorMessageResourceName = "IURequiredField", ErrorMessageResourceType = typeof(Literals))]
    public string LastName { get; set; } = null!;

    [Display(Name = "IUImage", ResourceType = typeof(Literals))]
    public string? Photo { get; set; }

    [Display(Name = "UserType", ResourceType = typeof(Literals))]
    public UserType UserType { get; set; }

    [Required(ErrorMessageResourceName = "IURequiredField", ErrorMessageResourceType = typeof(Literals))]
    public Country Country { get; set; } = null!;

    [Display(Name = "AppCountry", ResourceType = typeof(Literals))]
    [Range(1, int.MaxValue, ErrorMessageResourceName = "IURequiredField", ErrorMessageResourceType = typeof(Literals))]
    public int CountryId { get; set; }

    [Display(Name = "User", ResourceType = typeof(Literals))]
    public string FullName => $"{FirstName} {LastName}";

    //public ICollection<Group>? GroupsManaged { get; set; }

    //public ICollection<UserGroup>? GroupsBelong { get; set; }

    //public ICollection<Prediction>? Predictions { get; set; }

    //public int PredictionsCount => Predictions == null ? 0 : Predictions.Count;

    public string PhotoFull => string.IsNullOrEmpty(Photo) ? "/images/NoImage.png" : Photo;

    public ICollection<UserTenantPermission> UsersTenantPermissions { get; set; } = new List<UserTenantPermission>();
    public ICollection<UserSubscription> UsersSubscriptions { get; set; } = new List<UserSubscription>();
}