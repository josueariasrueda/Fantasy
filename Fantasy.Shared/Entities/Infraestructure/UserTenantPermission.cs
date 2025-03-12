using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy.Shared.Entities.Infraestructure;

public class UserTenantPermission
{
    public string UserId { get; set; } = null!;
    public User User { get; set; } = null!;
    public int TenantId { get; set; }
    public Tenant Tenant { get; set; } = null!;
    public int ModuleId { get; set; }
    public Module Module { get; set; } = null!;
    public bool CanRead { get; set; }
    public bool CanWrite { get; set; }
    public bool CanEdit { get; set; }
    public bool CanDelete { get; set; }
    public bool OnlyMine { get; set; }
}