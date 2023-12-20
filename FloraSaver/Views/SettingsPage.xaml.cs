using FloraSaver.Models;
using FloraSaver.ViewModels;

namespace FloraSaver;

public partial class SettingsPage : ContentPage
{
    public SettingsPage(SettingsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    private void ColorIntervalPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        var picker = (Picker)sender;
        int selectedIndex = picker.SelectedIndex;

        if (selectedIndex != -1)
        {
            var pickerColor = (GroupColors)picker.ItemsSource[selectedIndex];
            hiddenSpacerForPickerUpdate.IsEnabled = hiddenSpacerForPickerUpdate.IsEnabled ? false : true;
            ((SettingsViewModel)(this.BindingContext)).GroupColorEdit();
        }
    }

    private void Entry_Completed(object sender, EventArgs e)
    {
        var entry = sender as Entry;
        entry.IsEnabled = false;
        entry.IsEnabled = true;
        ((SettingsViewModel)(this.BindingContext)).GroupNameEdit();
    }
}