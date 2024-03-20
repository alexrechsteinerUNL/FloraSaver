using FloraSaver.ViewModels;
using FloraSaver.Services;
using FloraSaver.Models;
using System.Linq;

namespace FloraSaver;

public partial class PlantDetailsPage : ContentPage
{
    public PlantDetailsPage(PlantDetailsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        addUpdate.Text = string.IsNullOrWhiteSpace(_GivenName.Text) ? "Add" : "Update";
        deletePlants.IsVisible = string.IsNullOrWhiteSpace(_GivenName.Text) ? false : true;
    }

    // This is a workaround to resolve a .NET MAUI bug regarding keyboards not disappearing on completion
    private void Entry_Completed(object sender, EventArgs e)
    {
        var entry = sender as Entry;
        entry.IsEnabled = false;
        entry.IsEnabled = true;
    }

    private void deadSpaceButtonHideSuggestion_Clicked(object sender, EventArgs e)
    {
        _PlantSpecies.Unfocus();
        _PlantSpecies.IsEnabled = false;
        _PlantSpecies.IsEnabled = true;
    }

    private void Validate(object sender, EventArgs e)
    {
        ((PlantDetailsViewModel)(this.BindingContext)).ValidateAlterPlantAsync();
    }
    //Alter this so that if the frame's width and height are smaller than 600 a "compact mode" is engaged. Where the inside layout is 4* instead of 10*
    private void Frame_SizeChanged(object sender, EventArgs e)
    {
        //var frameWidth = Frame.Width;
        //var frameHeight = Frame.Height;

        //if (frameWidth < 600 && frameHeight < 600)
        //{

        //}
    }
}