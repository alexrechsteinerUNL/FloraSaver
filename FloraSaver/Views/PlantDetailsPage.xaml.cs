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
        var intervals = PickerService.GetIntervals();
        // Required to use index here due to *possible* bug in MAUI that won't allow assigning by interval
        var waterIndex = intervals.Select(x => x.DaysFromNow).ToList().IndexOf(Int32.Parse(_CustomInterval.Text));
        waterIntervalPicker.SelectedIndex = waterIndex != -1 ? waterIndex : intervals.Count;

    }

    // This is a workaround to resolve a .NET MAUI bug regarding keyboards not disappearing on completion
    private void Entry_Completed(object sender, EventArgs e)
    {
        var entry = sender as Entry;
        entry.IsEnabled = false;
        entry.IsEnabled = true;
    }
}

