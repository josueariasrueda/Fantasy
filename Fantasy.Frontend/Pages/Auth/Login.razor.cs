using Fantasy.Frontend.Repositories;
using Fantasy.Frontend.Services;
using Fantasy.Shared.DTOs;
using Fantasy.Shared.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;

namespace Fantasy.Frontend.Pages.Auth;

public partial class Login
{
    private LoginDTO loginDTO = new();
    private bool wasClose;

    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private IDialogService DialogService { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private IRepository Repository { get; set; } = null!;
    [Inject] private ILoginService LoginService { get; set; } = null!;
    [Inject] private IStringLocalizer<Literals> L { get; set; } = null!;

    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; } = null!;

    private void ShowModalRecoverPassword()
    {
        var closeOnEscapeKey = new DialogOptions() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.ExtraLarge };
        DialogService.Show<RecoverPassword>(L["PasswordRecovery"], closeOnEscapeKey);
    }

    private void ShowModalResendConfirmationEmail()
    {
        var closeOnEscapeKey = new DialogOptions() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.ExtraLarge };
        DialogService.Show<ResendConfirmationEmailToken>(L["MailForwarding"], closeOnEscapeKey);
    }

    private void CloseModal()
    {
        wasClose = true;
        MudDialog.Cancel();
    }

    private async Task LoginAsync()
    {
        if (wasClose)
        {
            NavigationManager.NavigateTo("/");
            return;
        }

        var responseHttp = await Repository.PostAsync<LoginDTO, TokenDTO>("/api/accounts/Login", loginDTO);
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(L[message!], Severity.Error);
            return;
        }

        await LoginService.LoginAsync(responseHttp.Response!.Token);
        NavigationManager.NavigateTo("/");
    }

    private async Task LLenarCredenciales()
    {
        loginDTO.Email = "bm.zulu@yopmail.com";
        loginDTO.Password = "123456";
    }
}