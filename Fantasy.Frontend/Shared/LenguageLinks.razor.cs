using Fantasy.Frontend.Pages.Auth;
using Fantasy.Shared.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Localization;
using MudBlazor;

namespace Fantasy.Frontend.Shared;

public partial class LenguageLinks
{
    private string? photoUser;
    private string? username;

    [Inject] private IStringLocalizer<Literals> L { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private IDialogService DialogService { get; set; } = null!;
    [Inject] private LanguageService LanguageService { get; set; } = null!;
    [CascadingParameter] private Task<AuthenticationState> AuthenticationStateTask { get; set; } = null!;

    protected override async Task OnParametersSetAsync()
    {
        var authenticationState = await AuthenticationStateTask;
        var claims = authenticationState.User.Claims.ToList();
        var photoClaim = claims.FirstOrDefault(x => x.Type == "Photo");
        var nameClaim = claims.FirstOrDefault(x => x.Type == "UserName");
        if (photoClaim is not null)
        {
            photoUser = photoClaim.Value;
        }

        if (nameClaim is not null)
        {
            username = claims.FirstOrDefault(x => x.Type == "UserName").Value;
        }
    }

    private void EditAction()
    {
        NavigationManager.NavigateTo("/EditUser");
    }

    private void ShowModalLogIn()
    {
        var closeOnEscapeKey = new DialogOptions() { CloseOnEscapeKey = true };
        DialogService.Show<Login>(L["UserLogin"], closeOnEscapeKey);
    }

    private void ShowModalLogOut()
    {
        var closeOnEscapeKey = new DialogOptions() { CloseOnEscapeKey = true };
        DialogService.Show<Logout>(L["UserLogout"], closeOnEscapeKey);
    }

    private void ChangeLanguage(string language)
    {
        LanguageService.SetLanguage(language);
        NavigationManager.NavigateTo(NavigationManager.Uri, forceLoad: true);
    }
}