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

    private static void Entry_Completed(object sender, EventArgs e)
    {
        var entry = sender as Entry;
        entry.IsEnabled = false;
        entry.IsEnabled = true;
    }
}

