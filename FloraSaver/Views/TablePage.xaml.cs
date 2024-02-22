using FloraSaver.ViewModels;
using FloraSaver.Services;
using FloraSaver.Models;

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

    private void deadSpaceButtonHideSuggestion_Clicked(object sender, EventArgs e)
    {
        searchBar.Unfocus();
        searchBar.IsEnabled = false;
        searchBar.IsEnabled = true;
    }

    //This could be causing issues in release mode 
    private void hiddenSpacerForAppearingScrollTo_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        var result = sender as Label;
        int tryGetInt = new();
        var shouldScroll = Int32.TryParse(result.Text, out tryGetInt);
        if (shouldScroll && tryGetInt > 0)
        {
            plantDeck.ScrollTo(tryGetInt);
        }
    }
}