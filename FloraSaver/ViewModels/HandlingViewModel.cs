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
        public HandlingViewModel(IDatabaseService databaseService) : base (databaseService)
        {
            databaseService = _databaseService;
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
                PlantSelection(plant);
            }
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
