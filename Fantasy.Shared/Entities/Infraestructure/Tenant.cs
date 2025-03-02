using Fantasy.Shared.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy.Shared.Entities.Infraestructure;

public class Tenant : IMustHaveTenant
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Display(Name = "TenantCode", ResourceType = typeof(Literals))]
    [MaxLength(6, ErrorMessageResourceName = "IUMaxLength", ErrorMessageResourceType = typeof(Literals))]
    public string Code { get; set; } = null!;

    [Display(Name = "TenantName", ResourceType = typeof(Literals))]
    [MaxLength(100, ErrorMessageResourceName = "IUMaxLength", ErrorMessageResourceType = typeof(Literals))]
    [Required(ErrorMessageResourceName = "IURequiredField", ErrorMessageResourceType = typeof(Literals))]
    public string Name { get; set; } = null!;

    [Display(Name = "ConnectionString", ResourceType = typeof(Literals))]
    [Required(ErrorMessageResourceName = "IURequiredField", ErrorMessageResourceType = typeof(Literals))]
    public string ConnectionString { get; set; } = null!;

    public Guid TenantId { get; set; }

    public ICollection<Enterprise> Enterprises { get; set; } = new List<Enterprise>();
    public ICollection<UserTenantPermission> UsersTenantPermissions { get; set; } = new List<UserTenantPermission>();
}