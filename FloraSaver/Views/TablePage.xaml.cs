using FloraSaver.Models;
using FloraSaver.ViewModels;
using static System.Net.Mime.MediaTypeNames;

namespace FloraSaver;

public partial class TablePage : ContentPage
{
    public TablePage(TableViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
	}

    //protected override void OnAppearing()
    //{
    //    base.OnAppearing();
    //    getPlants.Command.Execute("GetPlantsCommand");
    //}
}

