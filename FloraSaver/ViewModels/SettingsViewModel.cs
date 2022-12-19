﻿using FloraSaver.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using CommunityToolkit.Mvvm.Input;

namespace FloraSaver.ViewModels
{
    public partial class SettingsViewModel : BaseViewModel
    {
        PlantService plantService;
        public SettingsViewModel(PlantService plantService)
        {
            this.plantService = plantService;
        }

        [RelayCommand]
        async Task ClearAllPlantDataAsync()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;
                bool reallyDelete = await Application.Current.MainPage.DisplayAlert("OH HOLD ON!", "Are you sure you want to delete all of your plants?", "Yes", "No");
                if (reallyDelete)
                {
                    await plantService.DeleteAllAsync();
                }
                
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unable to delete all plants: {ex.Message}");
                await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
