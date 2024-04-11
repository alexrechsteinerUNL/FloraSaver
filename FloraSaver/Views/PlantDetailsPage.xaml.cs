using FloraSaver.ViewModels;
using FloraSaver.Services;
using FloraSaver.Models;
using System.Linq;
using FloraSaver.Utilities;

namespace FloraSaver;
public partial class PlantDetailsPage : ContentPage, IAndroidBackButtonHandlerUtility
{
    
    public PlantDetailsPage(PlantDetailsViewModel viewModel)
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


    // This is a workaround to resolve a .NET MAUI bug regarding keyboards not disappearing on completion
    private void Entry_Completed(object sender, EventArgs e)
    {
        var entry = sender as Entry;
        entry.IsEnabled = false;
        entry.IsEnabled = true;
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
}