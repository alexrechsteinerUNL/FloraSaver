using FloraSaver.ViewModels;

namespace FloraSaver;

public partial class MainPage : ContentPage
{
	public MainPage(TableViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
    }
}

