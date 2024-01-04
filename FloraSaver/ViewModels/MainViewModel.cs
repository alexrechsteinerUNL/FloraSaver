using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FloraSaver.Models;
using FloraSaver.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloraSaver.ViewModels
{
    public partial class MainViewModel : TableViewModel, INotifyPropertyChanged
    {
        [ObservableProperty]
        public Plant nextPlant;

        [ObservableProperty]
        public bool isNextPlantVisible = false;

        [ObservableProperty]
        public bool isNextWater = false;

        [ObservableProperty]
        public bool isNextMist = false;

        [ObservableProperty]
        public bool isNextSun = false;

        public MainViewModel(IDatabaseService databaseService, IPlantNotificationService plantNotificationService) : base(databaseService, plantNotificationService)
        {
            databaseService = _databaseService;
            plantNotificationService = _plantNotificationService;
        }

        [RelayCommand]
        private async Task AppearingHomeAsync()
        {
            if (ShouldUpdateCheckService.shouldGetNewPlantDataMain) { await GetPlantsAsync(); ShouldUpdateCheckService.shouldGetNewPlantDataMain = false; }
            PeriodicTimerUpdaterBackgroundAsync(() => CheatUpdateAllPlantProgress());
            SetNextPlant();
        }

        public void SetNextPlant()
        {
            NextPlant = DataPlants.OrderByDescending(_ => new[] { _.WaterPercent, _.MistPercent, _.SunPercent }.Max()).ThenByDescending(_ => new[] { _.UseWatering, _.UseMisting, _.UseMoving }.Max()).FirstOrDefault();
            if (NextPlant != null)
            {
                IsNextPlantVisible = true;
                var nextElement = new[] { NextPlant.WaterPercent, NextPlant.MistPercent, NextPlant.SunPercent }.Max();
                switch (nextElement)
                {
                    case var value when value == NextPlant.WaterPercent:
                        IsNextWater = true;
                        IsNextMist = false;
                        IsNextSun = false;
                        break;

                    case var value when value == NextPlant.MistPercent:
                        IsNextWater = false;
                        IsNextMist = true;
                        IsNextSun = false;
                        break;

                    case var value when value == NextPlant.SunPercent:
                        IsNextWater = false;
                        IsNextMist = false;
                        IsNextSun = true;
                        break;

                    default:
                        IsNextWater = false;
                        IsNextMist = false;
                        IsNextSun = false;
                        break;
                }
            }
        }

        protected override async Task ResetWateringAsync(Plant plant)
        {
            await base.ResetWateringAsync(plant);
            SetNextPlant();
        }

        protected override async Task ResetMistingAsync(Plant plant)
        {
            await base.ResetMistingAsync(plant);
            SetNextPlant();
        }

        protected override async Task ResetMovingAsync(Plant plant)
        {
            await base.ResetMovingAsync(plant);
            SetNextPlant();
        }
    }
}