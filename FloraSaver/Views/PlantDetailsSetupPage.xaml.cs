using FloraSaver.ViewModels;
using FloraSaver.Services;
using FloraSaver.Models;
using System.Linq;
using FloraSaver.Utilities;
using System.Linq.Expressions;

namespace FloraSaver;


public partial class PlantDetailsSetupPage : ContentPage, IAndroidBackButtonHandlerUtility
{
    public PlantDetailsSetupPage(PlantDetailsSetupViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        _ChangeImagePreview.IsVisible = false;
        base.OnAppearing();
        addUpdate.Text = string.IsNullOrWhiteSpace(_GivenName.Text) ? "Add" : "Update";
        deletePlants.IsVisible = string.IsNullOrWhiteSpace(_GivenName.Text) ? false : true;
        _ChangeImagePreview.IsVisible = true;
    }

    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);

        if (height < 700)
        {
            clipet.IsVisible = false;
            speechSpace.IsVisible = false;
            if (width > height)
            {
                HorizontalSelect.IsVisible = true;
                VerticalSelect.IsVisible = false;
            }
        }
        else
        {
            HorizontalSelect.IsVisible = false;
            VerticalSelect.IsVisible = true;
            clipet.IsVisible = true;
            speechSpace.IsVisible = true;
            
        }
    }

    public async Task<bool> HandleBackButtonAsync()
    {
        await ((PlantDetailsViewModel)(this.BindingContext)).GoToTableAsync();
        return true;
    }

    private void deadSpaceButtonHideSuggestion_Clicked(object sender, EventArgs e)
    {
        _PlantSpecies.Unfocus();
        _PlantSpecies.IsEnabled = false;
        _PlantSpecies.IsEnabled = true;
    }

    // This is a workaround to resolve a .NET MAUI bug regarding keyboards not disappearing on completion
    private void Entry_Completed(object sender, EventArgs e)
    {
        var entry = sender as Entry;
        entry.IsEnabled = false;
        entry.IsEnabled = true;
    }
}