using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy.Shared.Entities.Infraestructure;

public class EnterprisesTenant
{
    public int EnterpriseId { get; set; }
    public Enterprise Enterprise { get; set; } = null!;
    public int TenantId { get; set; }
    public Tenant Tenant { get; set; } = null!;
}