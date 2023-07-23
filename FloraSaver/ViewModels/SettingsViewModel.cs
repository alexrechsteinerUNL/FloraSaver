﻿using FloraSaver.Services;
using System.Diagnostics;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using FloraSaver.Models;

namespace FloraSaver.ViewModels
{
    public partial class SettingsViewModel : TableViewModel
    {
        public ObservableCollection<PlantGroup> VisiblePlantGroups { get; set; } = new();

        public SettingsViewModel(IDatabaseService databaseService, IPlantNotificationService plantNotificationService) : base(databaseService, plantNotificationService)
        {
            databaseService = _databaseService;
            plantNotificationService = _plantNotificationService;
            DateTime morningDate = DateTime.FromBinary(Preferences.Default.Get("morning_time_date", new DateTime(1,1,1,8,0,0).ToBinary()));
            morningTime = morningDate.TimeOfDay;
            var middayDate = DateTime.FromBinary(Preferences.Default.Get("midday_time_date", new DateTime(1, 1, 1, 12, 0, 0).ToBinary()));
            middayTime = middayDate.TimeOfDay;
            var nightDate = DateTime.FromBinary(Preferences.Default.Get("night_time_date", new DateTime(1, 1, 1, 16, 0, 0).ToBinary()));
            nightTime = nightDate.TimeOfDay;
        }

        [ObservableProperty]
        TimeSpan morningTime;
        [ObservableProperty]
        TimeSpan middayTime;
        [ObservableProperty]
        TimeSpan nightTime;

        [RelayCommand]
        partial void OnMorningTimeChanged(TimeSpan value)
        {
            var morningTimeDate = new DateTime().Add(value).ToBinary();
            Preferences.Default.Set("morning_time_date", morningTimeDate);
        }

        [RelayCommand]
        partial void OnMiddayTimeChanged(TimeSpan value)
        {
            var middayTimeDate = new DateTime().Add(value).ToBinary();
            Preferences.Default.Set("midday_time_date", middayTimeDate);
        }

        [RelayCommand]
        partial void OnNightTimeChanged(TimeSpan value)
        {
            var nightTimeDate = new DateTime().Add(value).ToBinary();
            Preferences.Default.Set("night_time_date", nightTimeDate);
            NightTime = DateTime.FromBinary(Preferences.Default.Get("night_time_date", new DateTime(1, 1, 1, 16, 0, 0).ToBinary())).TimeOfDay;
        }

        [RelayCommand]
        protected async Task GetVisiblePlantGroupsAsync()
        {
            await GetPlantGroupsAsync();
            VisiblePlantGroups = new ObservableCollection<PlantGroup>(PlantGroups);
            OnPropertyChanged(nameof(VisiblePlantGroups));
        }

        [RelayCommand]
        async Task ClearAllPlantGroupDataAsync()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;
                bool reallyDelete = await Application.Current.MainPage.DisplayAlert("OH HOLD ON!", "Are you sure you want to delete all of your plant groups?", "Delete Them", "Please Don't");
                if (reallyDelete)
                {
                    await _databaseService.DeleteAllPlantGroupsAsync();
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


        [RelayCommand]
        async Task ClearAllPlantDataAsync()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;
                bool reallyDelete = await Application.Current.MainPage.DisplayAlert("OH HOLD ON!", "Are you sure you want to delete all of your plants?", "Delete Them", "Please Don't");
                if (reallyDelete)
                {
                    await _databaseService.DeleteAllAsync();
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
