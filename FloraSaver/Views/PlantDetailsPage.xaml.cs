using FloraSaver.ViewModels;
using FloraSaver.Services;
using FloraSaver.Models;

namespace FloraSaver;

public partial class PlantDetailsPage : ContentPage
{

	public PlantDetailsPage(PlantDetailsViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
    }

}

