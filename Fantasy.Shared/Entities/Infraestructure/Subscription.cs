using Fantasy.Shared.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy.Shared.Entities.Infraestructure;

public class Subscription
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Display(Name = "SubscriptionName", ResourceType = typeof(Literals))]
    [MaxLength(100, ErrorMessageResourceName = "IUMaxLength", ErrorMessageResourceType = typeof(Literals))]
    [Required(ErrorMessageResourceName = "IURequiredField", ErrorMessageResourceType = typeof(Literals))]
    public string Name { get; set; } = null!;

    [Display(Name = "ExpirationDate", ResourceType = typeof(Literals))]
    public DateTime ExpirationDate { get; set; }

    [Display(Name = "MaxUsers", ResourceType = typeof(Literals))]
    public int MaxUsers { get; set; }

    [Display(Name = "MaxEnterprises", ResourceType = typeof(Literals))]
    public int MaxEnterprises { get; set; }

    public ICollection<UserSubscription> UsersSubscriptions { get; set; } = new List<UserSubscription>();
}