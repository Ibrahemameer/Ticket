using System;
using System.Globalization;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace RhinoTicketingSystem.Components
{
    public partial class CulturePicker
    {
        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        protected string culture;

        protected override void OnInitialized()
        {
            // Initialize culture based on current thread's culture
            culture = CultureInfo.CurrentCulture.Name;
        }

        protected void ChangeCulture()
        {
            // Log current culture before changing
            Console.WriteLine($"Changing culture to: {culture}");

            // Use a relative path for redirectUri
            var redirectUri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri).PathAndQuery; // Get just the path and query
            NavigationManager.NavigateTo($"/Culture/SetCulture?culture={Uri.EscapeDataString(culture)}&redirectUri={Uri.EscapeDataString(redirectUri)}", forceLoad: true);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                // Set direction based on selected culture
                await SetDirection(culture);
            }
        }

        private async Task SetDirection(string culture)
        {
            Console.WriteLine($"Setting direction for culture: {culture}"); // Debug output

            if (culture == "ar-SA")
            {
                await JSRuntime.InvokeVoidAsync("setDirection", "rtl");
                Console.WriteLine("Setting direction to RTL"); // Debug output
            }
            else
            {
                await JSRuntime.InvokeVoidAsync("setDirection", "ltr");
                Console.WriteLine("Setting direction to LTR"); // Debug output
            }
        }
    }
}