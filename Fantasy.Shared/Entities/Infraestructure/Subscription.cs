using Fantasy.Shared.Resources;
using System.ComponentModel.DataAnnotations;

namespace Fantasy.Shared.Entities.Infraestructure;

public class Subscription : AuditableEntity
{
    [Key]
    public int SubscriptionId { get; set; }

    [Display(Name = "SubscriptionName", ResourceType = typeof(Literals))]
    [MaxLength(100, ErrorMessageResourceName = "IUMaxLength", ErrorMessageResourceType = typeof(Literals))]
    [Required(ErrorMessageResourceName = "IURequiredField", ErrorMessageResourceType = typeof(Literals))]
    public string Name { get; set; } = null!;

    [Display(Name = "SubscriptionTenant", ResourceType = typeof(Literals))]
    [Required(ErrorMessageResourceName = "IURequiredField", ErrorMessageResourceType = typeof(Literals))]
    public Tenant Tenant { get; set; } = null!;

    [Display(Name = "SubscriptionTenant", ResourceType = typeof(Literals))]
    public int TenantId { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "SubscriptionCreationDate", ResourceType = typeof(Literals))]
    public DateTime? CreationDate { get; set; } = DateTime.UtcNow.AddYears(1);

    [DataType(DataType.Date)]
    [Display(Name = "SubscriptionExpirationDate", ResourceType = typeof(Literals))]
    public DateTime? ExpirationDate { get; set; } = DateTime.UtcNow.AddYears(1);

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

    [Display(Name = "SubscriptionIsTrial", ResourceType = typeof(Literals))]
    public bool IsTrial { get; set; } = false;

    [Display(Name = "SubscriptionIsActive", ResourceType = typeof(Literals))]
    public bool Active { get; set; } = true;
}