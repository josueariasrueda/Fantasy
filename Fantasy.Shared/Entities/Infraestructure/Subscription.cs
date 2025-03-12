using Fantasy.Shared.Resources;
using System.ComponentModel.DataAnnotations;

namespace Fantasy.Shared.Entities.Infraestructure;

public class Subscription
{
    public int Id { get; set; }

    [Display(Name = "SubscriptionName", ResourceType = typeof(Literals))]
    [MaxLength(100, ErrorMessageResourceName = "IUMaxLength", ErrorMessageResourceType = typeof(Literals))]
    [Required(ErrorMessageResourceName = "IURequiredField", ErrorMessageResourceType = typeof(Literals))]
    public string Name { get; set; } = null!;

    [Display(Name = "SubscriptionTenant", ResourceType = typeof(Literals))]
    [MaxLength(6, ErrorMessageResourceName = "IUMaxLength", ErrorMessageResourceType = typeof(Literals))]
    public string Tenant { get; set; } = null!;

    [Display(Name = "SubscriptionConnectionString", ResourceType = typeof(Literals))]
    [Required(ErrorMessageResourceName = "IURequiredField", ErrorMessageResourceType = typeof(Literals))]
    public string ConnectionString { get; set; } = null!;

    [DataType(DataType.Date)]
    [Display(Name = "SubscriptionExpirationDate", ResourceType = typeof(Literals))]
    public DateTime? ExpirationDate { get; set; } = DateTime.Now;

    [Display(Name = "SubscriptionMaxUsers", ResourceType = typeof(Literals))]
    public int MaxUsers { get; set; }

    [Display(Name = "SubscriptionMaxEnterprises", ResourceType = typeof(Literals))]
    public int MaxEnterprises { get; set; }

    [Display(Name = "SubscriptionMaxElectronicsDocs", ResourceType = typeof(Literals))]
    public int MaxElectronicsDocs { get; set; }

    [Display(Name = "SubscriptionMaxSpace", ResourceType = typeof(Literals))]
    public int MaxSpace { get; set; }

    [Display(Name = "SubscriptionDiskSpace", ResourceType = typeof(Literals))]
    public int DiskSpace { get; set; }

    public string UserId { get; set; } = null!;
    public User User { get; set; } = null!;
}