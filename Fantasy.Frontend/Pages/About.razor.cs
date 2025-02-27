using Fantasy.Shared.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace Fantasy.Frontend.Pages;

public partial class About
{
    [Inject] private IStringLocalizer<Literals> L { get; set; } = null!;
}