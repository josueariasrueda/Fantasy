using Fantasy.Shared.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace Fantasy.Frontend;

public partial class App
{
    [Inject] private IStringLocalizer<Literals> Localizer { get; set; } = null!;
    [Inject] private LanguageService LanguageService { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        await LanguageService.InitializeLanguageAsync();
    }
}