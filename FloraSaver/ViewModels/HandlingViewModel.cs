using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FloraSaver.Models;
using FloraSaver.Services;

namespace FloraSaver.ViewModels
{
    public partial class HandlingViewModel : TableViewModel, INotifyPropertyChanged
    {

        public HandlingViewModel(IDatabaseService databaseService, IPlantNotificationService plantNotificationService) : base (databaseService, plantNotificationService)
        {
            databaseService = _databaseService;
            plantNotificationService = _plantNotificationService;
        }

        [RelayCommand]
        async Task AppearingHandlingAsync()
        {
            timer = new PeriodicTimer(TimeSpan.FromSeconds(10));
            await GetPlantGroupsAsync();
            await GetPlantsAsync();
            PeriodicTimerUpdaterBackgroundAsync(() => CheatUpdateAllPlantProgress());

        }

        [RelayCommand]
        public void DisappearingHandling()
        {
            timer.Dispose();
        }

        [RelayCommand]
        void PlantSelection(Plant plant)
        {
            var specificPlant = Plants.FirstOrDefault(_ => _.Id == plant.Id);
            specificPlant.IsEnabled = plant.IsEnabled ? false : true;
            OnPropertyChanged("Plants");
        }

        [RelayCommand]
        void AllPlantSelection()
        {
            foreach (var plant in Plants)
            {
                plant.IsEnabled = true;
            }
            OnPropertyChanged("Plants");
        }

        [RelayCommand]
        void AllPlantUnselection()
        {
            foreach (var plant in Plants)
            {
                plant.IsEnabled = false;
            }
            OnPropertyChanged("Plants");
        }

        [RelayCommand]
        async Task CheckAllSelectedWaterAsync()
        {
            var changingPlants = Plants.ToList();
            foreach (var plant in changingPlants.Where(_ => _.IsEnabled && _.UseWatering)) {
                await ResetWateringAsync(plant);
            }
        }

        [RelayCommand]
        async Task CheckAllSelectedMistAsync()
        {
            var changingPlants = Plants.ToList();
            foreach (var plant in changingPlants.Where(_ => _.IsEnabled && _.UseMisting))
            {
                await ResetMistingAsync(plant);
            }
        }

        [RelayCommand]
        async Task CheckAllSelectedMoveAsync()
        {
            var changingPlants = Plants.ToList();
            foreach (var plant in changingPlants.Where(_ => _.IsEnabled && _.UseMoving))
            {
                await ResetMovingAsync(plant);
            }
        }
    }
}
