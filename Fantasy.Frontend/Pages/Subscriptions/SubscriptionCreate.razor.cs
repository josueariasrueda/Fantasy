using Fantasy.Frontend.Repositories;
using Fantasy.Shared.Entities.Infraestructure;
using Fantasy.Shared.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;

namespace Fantasy.Frontend.Pages.Subscriptions;

public partial class SubscriptionCreate
{
    private SubscriptionForm? subscriptionForm;
    private Subscription subscription = new();

    [Inject] private IRepository Repository { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private IStringLocalizer<Literals> L { get; set; } = null!;

    private async Task CreateAsync()
    {
        var responseHttp = await Repository.PostAsync("/api/subscriptions", subscription);
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
        subscriptionForm!.FormPostedSuccessfully = true;
        NavigationManager.NavigateTo("/subscriptions");
    }
}