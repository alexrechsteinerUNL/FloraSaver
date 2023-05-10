﻿using System;
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
    public partial class TableViewModel : BaseViewModel, INotifyPropertyChanged
    {
        private readonly int percentageButtonSize = 105;
        public ObservableCollection<Plant> DataPlants { get; set; } = new();
        public ObservableCollection<Plant> Plants { get; set; } = new();

        // I moved the plantService to the base viewmodel because just about every page was going to use it.
        public TableViewModel(PlantService plantService)
        {
            Title = "Plant Saver";
            this.plantService = plantService;
            IsPlantTypeIncluded = true;
            WaterRectangle = new Rect(0,20,105,105);
        }

        [RelayCommand]
        void Appearing()
        {

        }

        [ObservableProperty]
        bool isRefreshing;

        [ObservableProperty]
        Rect rectNeedsWatering;
        [ObservableProperty]
        double percentageToNeedsRefreshing;
        [ObservableProperty]
        double percentageToNeedsSunning;

        [ObservableProperty]
        bool isPlantTypeIncluded;
        [ObservableProperty]
        string toggleButtonTextColor;
        [ObservableProperty]
        bool toggleButtonBackgroundColor;

        [ObservableProperty]
        Rect waterRectangle;

        [ObservableProperty]
        bool waterEnabled;
        [ObservableProperty]
        bool mistEnabled;
        [ObservableProperty]
        bool moveEnabled;

        [ObservableProperty]
        int waterOpacity;
        [ObservableProperty]
        int mistOpacity;
        [ObservableProperty]
        int moveOpacity;

        [RelayCommand]
        async Task ResetWateringAsync(Plant plant)
        {
            if (plant.UseWatering)
            {
                plant.DateOfNextWatering = plant.DateOfNextWatering.AddDays(plant.WaterInterval != null ? (int)plant.WaterInterval :
                (plant.DateOfNextWatering.Date - plant.DateOfLastWatering.Date).Days);
                plant.DateOfLastWatering = DateTime.Now;
                plantService.PlantNotificationEnder(plant, "water");
                await plantService.AddUpdateNewPlantAsync(plant);
                OnPropertyChanged("Plant");
            }
        }

        [RelayCommand]
        async Task ResetMistingAsync(Plant plant)
        {
            if (plant.UseMisting)
            {
                plant.DateOfLastMisting = DateTime.Now;
                plant.DateOfNextMisting = plant.DateOfNextMisting.AddDays(plant.MistInterval != null ? (int)plant.MistInterval :
                    (plant.DateOfNextMisting.Date - plant.DateOfLastMisting.Date).Days);
                plantService.PlantNotificationEnder(plant, "mist");
                await plantService.AddUpdateNewPlantAsync(plant);
                OnPropertyChanged("Plant");
            }
        }

        [RelayCommand]
        async Task ResetMovingAsync(Plant plant)
        {
            if (plant.UseMoving)
            {
                plant.DateOfLastMove = DateTime.Now;
                plant.DateOfNextMove = plant.DateOfNextMove.AddDays(plant.SunInterval != null ? (int)plant.SunInterval :
                    (plant.DateOfNextMove.Date - plant.DateOfLastMove.Date).Days);
                plantService.PlantNotificationEnder(plant, "move");
                await plantService.AddUpdateNewPlantAsync(plant);
                OnPropertyChanged("Plant");
            }
        }

        [RelayCommand]
        void CurrentPlantNeeds(Plant plant)
        {
            //Eh this needs a little work. Gotta account for times
            var percentageToNeedsWatering = (DateTime.Now - plant.DateOfLastWatering).TotalSeconds /
                (plant.DateOfNextWatering - plant.DateOfLastWatering).TotalSeconds * 105;
            RectNeedsWatering = new Rect(0, percentageToNeedsWatering, 105, 105);
        
        }

        [RelayCommand]
        async Task SearchPlantsAsync(string inputString)
        {
            try
            {
                Plants.Replace(DataPlants);
                if (!string.IsNullOrWhiteSpace(inputString))
                {
                    for (int i = Plants.Count - 1; i >= 0; i--)
                    {
                        if (Plants[i].GivenName.IndexOf(inputString) < 0)
                        {
                            Plants.RemoveAt(i);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unable to search plants: {ex.Message}");
                await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");
            }
        }

        [RelayCommand]
        async Task GetPlantsAsync()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;
                var plants = await plantService.GetAllPlantAsync();

                if (Plants.Count != 0)
                {
                    Plants.Clear();
                } 
                foreach (var plant in plants)
                {
                    Plants.Add(plant);
                    OnPropertyChanged(nameof(Plants));
                }
                DataPlants = new ObservableCollection<Plant>(Plants);

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unable to get plants: {ex.Message}");
                await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
                IsRefreshing = false;
            }
            return;
        }

        [RelayCommand]
        async Task GoToDetailsAsync(Plant plant)
        {
            if (plant == null)
            {
                await Shell.Current.GoToAsync(nameof(PlantDetailsPage), true, new Dictionary<string, object>
                {
                    {"Plant", new Plant {
                        Id = DataPlants.Any() ? DataPlants.Max(x => x.Id) + 1 : 0,
                        DateOfBirth = DateTime.Now,
                        DateOfLastWatering = DateTime.Now,
                        DateOfNextWatering = DateTime.Now,
                        DateOfLastMisting = DateTime.Now,
                        DateOfNextMisting = DateTime.Now,
                        DateOfLastMove = DateTime.Now,
                        DateOfNextMove = DateTime.Now
                        }
                    }
                });
            }
            else
            {
                CurrentPlantNeeds(plant);
                await Shell.Current.GoToAsync(nameof(PlantDetailsPage), true, new Dictionary<string, object>
                {
                    {"Plant", plant }
                });
            }
            return;
        }
    }
}
