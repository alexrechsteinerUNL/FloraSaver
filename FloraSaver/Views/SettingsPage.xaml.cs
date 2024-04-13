using FloraSaver.Models;
using FloraSaver.ViewModels;
using Microsoft.Maui.Controls.Internals;
using FloraSaver.Utilities;

namespace FloraSaver;

public partial class SettingsPage : ContentPage, IAndroidBackButtonHandlerUtility
{
    public SettingsPage(SettingsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    private void ColorIntervalPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        var picker = (Picker)sender;
        var pickerContext = (PlantGroup)picker.BindingContext;
        int selectedIndex = picker.SelectedIndex;

        if (selectedIndex != -1)
        {
            var pickerColor = (GroupColors)picker.ItemsSource[selectedIndex];
            hiddenSpacerForPickerUpdate.IsEnabled = hiddenSpacerForPickerUpdate.IsEnabled ? false : true;
            ((SettingsViewModel)(this.BindingContext)).GroupColorEdit();

            plantGroupDeck.ScrollTo(pickerContext, animate: false);
        }
    }

    public async Task<bool> HandleBackButtonAsync()
    {
        return await ((BaseViewModel)(this.BindingContext)).BackButtonWarnLeavingApplicationAsync() ? false : true;
    }

    private void Validate(object sender, EventArgs e)
    {
        _ = ((SettingsViewModel)(this.BindingContext)).ValidateGroupAsync();
    }

    private void Entry_Completed(object sender, EventArgs e)
    {
        var entry = sender as Entry;
        entry.IsEnabled = false;
        entry.IsEnabled = true;
        ((SettingsViewModel)(this.BindingContext)).GroupNameEdit();
        var entryContext = (PlantGroup)entry.BindingContext;
        plantGroupDeck.ScrollTo(entryContext, animate: false);
    }

    private void ScrollTo_ImageButton_Clicked(object sender, EventArgs e)
    {
        var result = (ImageButton)sender;
        var clickContext = (PlantGroup)result.BindingContext;
        plantGroupDeck.ScrollTo(clickContext, animate: false);
    }

    private void ScrollTo_Button_Clicked(object sender, EventArgs e)
    {
        var result = (Button)sender;
        var clickContext = (PlantGroup)result.BindingContext;
        plantGroupDeck.ScrollTo(clickContext, animate: false);
    }
}