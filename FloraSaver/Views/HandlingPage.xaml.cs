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

    private static void Entry_Completed(object sender, EventArgs e)
    {
        var entry = sender as Entry;
        entry.IsEnabled = false;
        entry.IsEnabled = true;
    }
}
