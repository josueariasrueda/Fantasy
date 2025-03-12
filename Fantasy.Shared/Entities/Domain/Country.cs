using Fantasy.Shared.Resources;
using System.ComponentModel.DataAnnotations;

namespace Fantasy.Shared.Entities.Domain;

public class Country
{
    public int Id { get; set; }

    [Display(Name = "Country", ResourceType = typeof(Literals))]
    [MaxLength(100, ErrorMessageResourceName = "IUMaxLength", ErrorMessageResourceType = typeof(Literals))]
    [Required(ErrorMessageResourceName = "IURequiredField", ErrorMessageResourceType = typeof(Literals))]
    public string Name { get; set; } = null!;

    [Display(Name = "CountryCode", ResourceType = typeof(Literals))]
    [MaxLength(5, ErrorMessageResourceName = "IUMaxLength", ErrorMessageResourceType = typeof(Literals))]
    public string? Code { get; set; }

    [Display(Name = "CountryISOname", ResourceType = typeof(Literals))]
    [MaxLength(100, ErrorMessageResourceName = "IUMaxLength", ErrorMessageResourceType = typeof(Literals))]
    public string? ISOname { get; set; }

    [Display(Name = "CountryAlfa2Code", ResourceType = typeof(Literals))]
    [MaxLength(5, ErrorMessageResourceName = "IUMaxLength", ErrorMessageResourceType = typeof(Literals))]
    public string? Alfa2Code { get; set; }

    [Display(Name = "CountryAlfa3Code", ResourceType = typeof(Literals))]
    [MaxLength(5, ErrorMessageResourceName = "IUMaxLength", ErrorMessageResourceType = typeof(Literals))]
    public string? Alfa3Code { get; set; }

    [Display(Name = "CountryDefaultCurrencyCode", ResourceType = typeof(Literals))]
    public int DefaultCurrencyId { get; set; }

    public Currency DefaultCurrency { get; set; } = null!;

    [Display(Name = "CountryCallingCode", ResourceType = typeof(Literals))]
    [MaxLength(5, ErrorMessageResourceName = "IUMaxLength", ErrorMessageResourceType = typeof(Literals))]
    public string? CallingCode { get; set; }

    [Display(Name = "CountryGuid", ResourceType = typeof(Literals))]
    [MaxLength(100, ErrorMessageResourceName = "IUMaxLength", ErrorMessageResourceType = typeof(Literals))]
    public Guid? GuidCode { get; set; }
}