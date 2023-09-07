using FloraSaver.ViewModels;
using FloraSaver.Services;
using FloraSaver.Models;
using System.Linq;

namespace FloraSaver;

public partial class PlantDetailsSetupPage : ContentPage
{

	public PlantDetailsSetupPage(PlantDetailsSetupViewModel viewModel)
	{
        InitializeComponent();
        BindingContext = viewModel;
    }

    // This is a workaround to resolve a .NET MAUI bug regarding keyboards not disappearing on completion
    private void Entry_Completed(object sender, EventArgs e)
    {
        var entry = sender as Entry;
        entry.IsEnabled = false;
        entry.IsEnabled = true;
    }
}

