using Fantasy.Frontend.Pages.Auth;
using Fantasy.Frontend.Repositories;
using Fantasy.Shared.DTOs.Generic;
using Fantasy.Shared.Entities;
using Fantasy.Shared.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Localization;
using MudBlazor;
using System.Net.Http;
using System.Net.Http.Json;

namespace Fantasy.Frontend.Shared;

public partial class AuthLinks
{
    private string? photoUser;
    private string? username;

    [Inject] private IStringLocalizer<Literals> L { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private IDialogService DialogService { get; set; } = null!;
    [Inject] private HttpClient HttpClient { get; set; } = null!;
    [CascadingParameter] private Task<AuthenticationState> AuthenticationStateTask { get; set; } = null!;

    protected override async Task OnParametersSetAsync()
    {
        var authenticationState = await AuthenticationStateTask;
        var claims = authenticationState.User.Claims.ToList();
        var photoClaim = claims.FirstOrDefault(x => x.Type == "Photo");
        var nameClaim = claims.FirstOrDefault(x => x.Type == "UserName");
        if (photoClaim is not null)
        {
            var encodedPhotoName = Uri.EscapeDataString(photoClaim.Value);
            var responseHttp = await HttpClient.GetAsync($"api/files/downloaduserphoto/{encodedPhotoName}");

            if (responseHttp.IsSuccessStatusCode)
            {
                var imageBytes = await responseHttp.Content.ReadAsByteArrayAsync();
                photoUser = $"data:image/jpeg;base64,{Convert.ToBase64String(imageBytes)}";
            }
        }

        if (nameClaim is not null)
        {
            username = nameClaim.Value;
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
}