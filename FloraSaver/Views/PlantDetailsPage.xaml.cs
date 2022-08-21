using FloraSaver.ViewModels;
using FloraSaver.Services;

namespace FloraSaver;

public partial class PlantDetailsPage : ContentPage
{

	public PlantDetailsPage(PlantDetailsViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
    }


}

