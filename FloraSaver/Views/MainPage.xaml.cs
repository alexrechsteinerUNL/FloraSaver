﻿using FloraSaver.ViewModels;

namespace FloraSaver;

public partial class MainPage : ContentPage
{
    public MainPage(MainViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);
        ((MainViewModel)(this.BindingContext)).ReconfigureValuesForScreenSize(width, height);
        
        if (height < 600)
        {
            _FullMode.IsVisible = false;
            _CompactMode.IsVisible = true;
            
        } else
        {
            _FullMode.IsVisible = true;
            _CompactMode.IsVisible = false;
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
    }
}