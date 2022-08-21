using FloraSaver.Models;
using FloraSaver.ViewModels;

namespace FloraSaver;

public partial class TablePage : ContentPage
{

    public TablePage(PlantsViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
        getPlants.Command.Execute("GetPlantsCommand");
    }


}

