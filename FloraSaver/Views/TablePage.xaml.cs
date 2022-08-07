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
}

