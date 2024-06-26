﻿using FloraSaver.ViewModels;

namespace FloraSaver;

public partial class AddGroupPage : ContentPage
{
    public AddGroupPage(PlantDetailsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
    }

    // This is a workaround to resolve a .NET MAUI bug regarding keyboards not disappearing on completion
    private void Entry_Completed(object sender, EventArgs e)
    {
        var entry = sender as Entry;
        entry.IsEnabled = false;
        entry.IsEnabled = true;
    }

    private void addNewGroupButton_Pressed(object sender, EventArgs e)
    {
        _groupAdd.IsEnabled = false;
        _groupAdd.IsEnabled = true;
    }
}