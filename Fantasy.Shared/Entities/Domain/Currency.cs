using Fantasy.Shared.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fantasy.Shared.Entities.Domain;

public class Currency
{
    public int Id { get; set; }

    [Display(Name = "Currency", ResourceType = typeof(Literals))]
    [MaxLength(100, ErrorMessageResourceName = "IUMaxLength", ErrorMessageResourceType = typeof(Literals))]
    [Required(ErrorMessageResourceName = "IURequiredField", ErrorMessageResourceType = typeof(Literals))]
    public string Name { get; set; } = null!;

    [Display(Name = "CurrencyCode", ResourceType = typeof(Literals))]
    [MaxLength(5, ErrorMessageResourceName = "IUMaxLength", ErrorMessageResourceType = typeof(Literals))]
    public string? Code { get; set; }

    [Display(Name = "CurrencyValue", ResourceType = typeof(Literals))]
    [MaxLength(500, ErrorMessageResourceName = "IUMaxLength", ErrorMessageResourceType = typeof(Literals))]
    public string? Value { get; set; }

    [Display(Name = "CurrencyGuid", ResourceType = typeof(Literals))]
    [MaxLength(100, ErrorMessageResourceName = "IUMaxLength", ErrorMessageResourceType = typeof(Literals))]
    public Guid? GuidCode { get; set; }
}