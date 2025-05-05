using Fantasy.Shared.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy.Shared.Entities.Infraestructure;

public class Module
{
    [Key]
    public int ModuleId { get; set; }

    [Display(Name = "SubscriptionName", ResourceType = typeof(Literals))]
    [MaxLength(100, ErrorMessageResourceName = "IUMaxLength", ErrorMessageResourceType = typeof(Literals))]
    [Required(ErrorMessageResourceName = "IURequiredField", ErrorMessageResourceType = typeof(Literals))]
    public string Name { get; set; }

    public bool Active { get; set; } = true; // Por defecto, los módulos estarán activos
    public ICollection<UserTenantPermission> UsersTenantPermissions { get; set; }
}