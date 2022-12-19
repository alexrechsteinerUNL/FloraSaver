using FloraSaver.ViewModels;

namespace FloraSaver;

public partial class SettingsPage : ContentPage
{
	public SettingsPage(SettingsViewModel viewModel)
	{
        InitializeComponent();
        BindingContext = viewModel;
    }
}

