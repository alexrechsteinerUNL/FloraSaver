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

    private void SearchBar_Completed(object sender, EventArgs e)
    {
        var searchElement = sender as SearchBar;

        searchElement.Unfocus();
        searchElement.IsEnabled = false;
        searchElement.IsEnabled = true;
    }
}

