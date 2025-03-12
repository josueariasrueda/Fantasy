using Fantasy.Frontend.Pages.Currencies;
using Fantasy.Frontend.Repositories;
using Fantasy.Shared.DTOs;
using Fantasy.Shared.Entities.Domain;
using Fantasy.Shared.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;

namespace Fantasy.Frontend.Pages.Currencies;

public partial class CurrencyCreate
{
    private CurrencyForm? currencyForm;
    private CurrencyDTO currencyDTO = new();

    [Inject] private IRepository Repository { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private IStringLocalizer<Literals> L { get; set; } = null!;

    private async Task CreateAsync()
    {
        var responseHttp = await Repository.PostAsync("/api/currencies", currencyDTO);
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(L[message!], Severity.Error);
            return;
        }

        Return();
        Snackbar.Add(L["IURecordCreatedOk"], Severity.Success);
    }

    private void Return()
    {
        currencyForm!.FormPostedSuccessfully = true;
        NavigationManager.NavigateTo("/currencies");
    }
}