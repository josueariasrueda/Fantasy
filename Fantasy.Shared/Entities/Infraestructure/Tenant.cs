using Fantasy.Shared.Entities.Domain;
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
    // Un Tenant puede tener muchas empresas
    // Una empresa pertenece a un Tenant
    // En estos casos las empresas comparten tablas de cuentas, insumos, apus, etc.
    // Es necesario marcar las entidades que se comparten entre empresas
    // Para eso se utiliza la interfaz IMultiEnterprise
    // Pero cada enterprise tiene sus propios asientos contables, proyectos, presupuestos, etc.
    // Las empresas no pueden ser eliminadas, solo desactivadas
    // El usuario que pertenece a un Tenant puede ingresar a cualquiera de las Enterpices simpre y cuando tenga permisos
    // Un tenant tiene Suscripciones que van quedando en el tiempo y cuando se renueva se crea una nueva
    // Cada tenant tiene acceso a los modulos de la solucion de acuerdo a la suscripcion

    // Un tenant es fisicamente una base de datos independiente
    // Un tenant tiene un usuario root que es el que crea el tenant
    // Un tenant puede tener muchos usuarios
    // Un tenant puede tener muchas suscripciones
    // Un tenant comparte monedas
    // Un tenant comparte paises
    // Un tenant comparte cuentas contables
    // Un tenant comparte insumos
    // Un tenant comparte apus
    // Un tenant puede tener muchos proyectos
    // Un tenant puede tener muchos presupuestos

    [Key]
    public int TenantId { get; set; }

    [Display(Name = "TenantCode", ResourceType = typeof(Literals))]
    [MaxLength(6, ErrorMessageResourceName = "IUMaxLength", ErrorMessageResourceType = typeof(Literals))]
    public string Code { get; set; } = null!;

    [Display(Name = "TenantName", ResourceType = typeof(Literals))]
    [MaxLength(100, ErrorMessageResourceName = "IUMaxLength", ErrorMessageResourceType = typeof(Literals))]
    [Required(ErrorMessageResourceName = "IURequiredField", ErrorMessageResourceType = typeof(Literals))]
    public string Name { get; set; } = null!;

    [Display(Name = "TenantStoragePath", ResourceType = typeof(Literals))]
    [MaxLength(200, ErrorMessageResourceName = "IUMaxLength", ErrorMessageResourceType = typeof(Literals))]
    [Required(ErrorMessageResourceName = "IURequiredField", ErrorMessageResourceType = typeof(Literals))]
    public string? StoragePath { get; set; } = null!;

    [Display(Name = "TenantConnectionString", ResourceType = typeof(Literals))]
    [Required(ErrorMessageResourceName = "IURequiredField", ErrorMessageResourceType = typeof(Literals))]
    public string ConnectionString { get; set; } = null!;

    [Display(Name = "TenantIsActive", ResourceType = typeof(Literals))]
    public bool IsActive { get; set; } = true;

    public ICollection<EnterpriseTenant> TenantsEnterprises { get; set; } = new List<EnterpriseTenant>();
    public ICollection<UserTenantPermission> UsersTenantPermissions { get; set; } = new List<UserTenantPermission>();
    public ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
}