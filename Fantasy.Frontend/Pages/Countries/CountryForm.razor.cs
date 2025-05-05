using CurrieTechnologies.Razor.SweetAlert2;
using Fantasy.Frontend.Repositories;
using Fantasy.Shared.Entities.Domain;
using Fantasy.Shared.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.Extensions.Localization;

namespace Fantasy.Frontend.Pages.Countries;

public partial class CountryForm
{
    private EditContext editContext = null!;
    private Currency? selectedCurrency = new();
    private List<Currency>? Currencies;

    [EditorRequired, Parameter] public CountryDTO CountryDTO { get; set; } = null!;
    [EditorRequired, Parameter] public EventCallback OnValidSubmit { get; set; }
    [EditorRequired, Parameter] public EventCallback ReturnAction { get; set; }

    public bool FormPostedSuccessfully { get; set; } = false;

    [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;
    [Inject] private IStringLocalizer<Literals> L { get; set; } = null!;
    [Inject] private IRepository Repository { get; set; } = null!;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        editContext = new(CountryDTO);
    }

    protected override async Task OnInitializedAsync()
    {
        await LoadCurrrenciesAsync();
    }

    private async Task<IEnumerable<Currency>> SearchCurrency(string searchText, CancellationToken cancellationToken)
    {
        await Task.Delay(5);
        if (string.IsNullOrWhiteSpace(searchText))
        {
            return Currencies!;
        }

        return Currencies!
            .Where(x => x.Name.Contains(searchText, StringComparison.InvariantCultureIgnoreCase))
            .ToList();
    }

    private void CurrencyChanged(Currency currency)
    {
        selectedCurrency = currency;
        CountryDTO.DefaultCurrencyId = currency.CurrencyId;
    }

    private async Task OnBeforeInternalNavigation(LocationChangingContext context)
    {
        var formWasEdited = editContext.IsModified();

        if (!formWasEdited || FormPostedSuccessfully)
        {
            return;
        }

        var result = await SweetAlertService.FireAsync(new SweetAlertOptions
        {
            Title = L["IUConfirmation"],
            Text = L["IULeaveAndLoseChanges"],
            Icon = SweetAlertIcon.Warning,
            ShowCancelButton = true,
            CancelButtonText = L["IUCancel"],
        });

        var confirm = !string.IsNullOrEmpty(result.Value);
        if (confirm)
        {
            return;
        }

        context.PreventNavigation();
    }

    private async Task LoadCurrrenciesAsync()
    {
        var responseHttp = await Repository.GetAsync<List<Currency>>("/api/currencies/combo");
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
            return;
        }

        Currencies = responseHttp.Response;
    }
}