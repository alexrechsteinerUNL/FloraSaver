﻿using FloraSaver.ViewModels;
using FloraSaver.Services;
using FloraSaver.Models;
using System.Linq;

namespace FloraSaver;

public partial class PlantDetailsSetupPage : ContentPage
{
    public PlantDetailsSetupPage(PlantDetailsSetupViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);
        //((MainViewModel)(this.BindingContext)).ReconfigureValuesForScreenSize(width, height);

        if (height < 700)
        {
            clipet.IsVisible = false;
            speechSpace.IsVisible = false;

        }
        else
        {
            clipet.IsVisible = true;
            speechSpace.IsVisible = true;
        }
    }

    private void Validate(object sender, EventArgs e)
    {
        ((PlantDetailsSetupViewModel)(this.BindingContext)).ValidateAlterPlantAsync();
    }

    // This is a workaround to resolve a .NET MAUI bug regarding keyboards not disappearing on completion
    private void Entry_Completed(object sender, EventArgs e)
    {
        var entry = sender as Entry;
        entry.IsEnabled = false;
        entry.IsEnabled = true;
    }
}