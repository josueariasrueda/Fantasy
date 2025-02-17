﻿using Fantasy.Shared.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;

namespace Fantasy.Frontend.Layout;

public partial class MainLayout
{
    private bool _drawerOpen = true;
    private string _icon = Icons.Material.Filled.DarkMode;

    [Inject] private IStringLocalizer<Literals> L { get; set; } = null!;
    private bool _darkMode { get; set; } = true;

    private void DrawerToggle()
    {
        _drawerOpen = !_drawerOpen;
    }

    private void DarkModeToggle()
    {
        _darkMode = !_darkMode;
        _icon = _darkMode ? Icons.Material.Filled.LightMode : Icons.Material.Filled.DarkMode;
    }
}