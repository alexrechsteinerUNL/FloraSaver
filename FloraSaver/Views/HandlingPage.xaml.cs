﻿using FloraSaver.ViewModels;
using FloraSaver.Services;
using FloraSaver.Models;

namespace FloraSaver;

public partial class HandlingPage : ContentPage
{
    public HandlingPage(HandlingViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);
        ((TableViewModel)(this.BindingContext)).ReconfigureSpanForScreenSize(width, height);
        if (_RefreshViewSpace.Height < _RefreshViewSpace.Width && _RefreshViewSpace.Width / _RefreshViewSpace.Height > 1.333333333 && _RefreshViewSpace.Height < 500)
        {
            _bigButtonSpace.IsVisible = false;
            _littleButtonSpace.IsVisible = true;
        } else
        {
            _bigButtonSpace.IsVisible = true;
            _littleButtonSpace.IsVisible = false;
        }
    }

    private static void Entry_Completed(object sender, EventArgs e)
    {
        var entry = sender as Entry;
        entry.IsEnabled = false;
        entry.IsEnabled = true;
    }
}