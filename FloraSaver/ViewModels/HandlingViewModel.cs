﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks.Dataflow;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FloraSaver.Models;
using FloraSaver.Services;

namespace FloraSaver.ViewModels
{
    public partial class HandlingViewModel : TableViewModel, INotifyPropertyChanged
    {
        public HandlingViewModel(IDatabaseService databaseService, IPlantNotificationService plantNotificationService) : base(databaseService, plantNotificationService)
        {
            databaseService = _databaseService;
            plantNotificationService = _plantNotificationService;
        }

        [RelayCommand]
        private async Task AppearingHandlingAsync()
        {
            timer = new PeriodicTimer(TimeSpan.FromSeconds(10));

            if (ShouldUpdateCheckService.shouldGetNewGroupDataHandling) { await GetPlantGroupsAsync(); ShouldUpdateCheckService.shouldGetNewGroupDataHandling = false; }
            if (ShouldUpdateCheckService.shouldGetNewPlantDataHandling) { await GetPlantsAsync(); ShouldUpdateCheckService.shouldGetNewPlantDataHandling = false; }
            _ = PeriodicTimerUpdaterBackgroundAsync(() => CheatUpdateAllPlantProgress());
            ShowTutorial = Preferences.Default.Get("show_tutorial", true);
            //await StandardActionsHandlingAsync(SearchQuery);
        }

        [RelayCommand]
        public async Task StandardActionsHandlingAsync(string searchText = "")
        {
            if (searchText != SearchQuery)
            {
                SearchQuery = searchText;

            }

            if (ShowSearchSuggestionsBox)
            {
                QueryAutofillPlantAsyncFromSearch(SearchQuery);
            }

            BackendPlantList = DataPlants.ToList();
            var temporaryBackendPlantList = new List<Plant>(BackendPlantList);
            foreach (var plant in temporaryBackendPlantList)
            {
                var isSavedFromGroups = await ShowHideSingualrPlantGroupsAsync(plant);
                var isSavedFromSearch = await SearchSingularPlantAsync(plant);
                if (!isSavedFromGroups || !isSavedFromSearch)
                {
                    BackendPlantList.Remove(plant);
                }
                else if (BackendPlantList.FirstOrDefault(_ => _.GivenName == plant.GivenName) is null)
                {
                    BackendPlantList.Add(plant);
                }

            }
            setPlantOrder(CurrentOrderByValue);
        }

        [RelayCommand]
        public void DisappearingHandling()
        {
            timer.Dispose();
        }

        [RelayCommand]
        private void PlantSelection(Plant plant)
        {
            // You need to take into account that we could be selecting from something other than this "plants" file here.
            var specificPlant = Plants.FirstOrDefault(_ => _.Id == plant.Id);
            specificPlant.IsEnabled = plant.IsEnabled ? false : true;
            OnPropertyChanged("Plants");
        }

        [RelayCommand]
        private void AllPlantSelection()
        {
            foreach (var plant in Plants)
            {
                plant.IsEnabled = true;
            }
            OnPropertyChanged("Plants");
        }

        [RelayCommand]
        private void AllPlantUnselection()
        {
            foreach (var plant in Plants)
            {
                plant.IsEnabled = false;
            }
            OnPropertyChanged("Plants");
        }

        [RelayCommand]
        private async Task CheckAllSelectedWaterAsync()
        {
            var changingPlants = Plants.ToList();
            foreach (var plant in changingPlants.Where(_ => _.IsEnabled && _.UseWatering))
            {
                await ResetWateringAsync(plant);
            }
        }

        [RelayCommand]
        private async Task CheckAllSelectedMistAsync()
        {
            var changingPlants = Plants.ToList();
            foreach (var plant in changingPlants.Where(_ => _.IsEnabled && _.UseMisting))
            {
                await ResetMistingAsync(plant);
            }
        }

        [RelayCommand]
        private async Task CheckAllSelectedMoveAsync()
        {
            var changingPlants = Plants.ToList();
            foreach (var plant in changingPlants.Where(_ => _.IsEnabled && _.UseMoving))
            {
                await ResetMovingAsync(plant);
            }
        }

        //This also exists in many pages you might want to genericize it
        [RelayCommand]
        private async Task DeletePlantAsync(Plant plant)
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;
                bool reallyDelete = await Application.Current.MainPage.DisplayAlert("OH HOLD ON!", $"Are you sure you want to delete your plant {plant.GivenName}?", "Delete It", "Please Don't");
                if (reallyDelete)
                {
                    await _databaseService.DeletePlantAsync(plant);
                    IsBusy = false;
                    await GetPlantsAsync();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unable to add or update plants: {ex.Message}");
                await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
                OnPropertyChanged("Plants");
            }
        }

        [RelayCommand]
        public async Task RefreshPlantsAsync()
        {
            await GetPlantsAsync(false);
            await StandardActionsHandlingAsync(SearchQuery);
        }
    }
}