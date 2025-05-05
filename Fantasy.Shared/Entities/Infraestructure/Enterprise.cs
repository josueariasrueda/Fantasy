using Fantasy.Shared.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy.Shared.Entities.Infraestructure;

public class Enterprise : IMustHaveTenant
{
    [Key]
    public int EnterpriseId { get; set; }

    [Display(Name = "EnterpriseName", ResourceType = typeof(Literals))]
    [MaxLength(100, ErrorMessageResourceName = "IUMaxLength", ErrorMessageResourceType = typeof(Literals))]
    [Required(ErrorMessageResourceName = "IURequiredField", ErrorMessageResourceType = typeof(Literals))]
    public string Name { get; set; }

    public bool Active { get; set; } = true; // Por defecto, las empresas estarán activas
    public ICollection<EnterpriseTenant> EnterprisesTenants { get; set; } = new List<EnterpriseTenant>();
}