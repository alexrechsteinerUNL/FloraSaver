﻿using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FloraSaver.Models;
using FloraSaver.Services;

namespace FloraSaver.ViewModels
{


    [QueryProperty(nameof(Plant), "Plant")]
    public partial class PlantDetailsViewModel : BaseViewModel, IQueryAttributable
    {
        PlantService plantService;
        PickerService pickerService;

        public Plant InitialPlant { get; set; }

        public PlantDetailsViewModel(PlantService PlantService, PickerService PickerService)
        {
            this.plantService = PlantService;
            this.pickerService = PickerService;
            wateringInterval = pickerService.GetIntervals();

        }
        [ObservableProperty]
        List<Interval> wateringInterval;


        // I think the code below is equivalent to
        // [ObservableProperty]
        // string friendlyLabel;
        string friendlyLabel;
        public string FriendlyLabel {
            get => friendlyLabel;
            set
            {
                friendlyLabel = value;
                OnPropertyChanged();
            }
        }

        [ObservableProperty]
        public bool customWaterInteravlGridVisible = false;

        [ObservableProperty]
        public bool refreshGridVisible = false;

        [ObservableProperty]
        public string refreshGridText = "Use Refreshing";

        [ObservableProperty]
        public Interval waterIntervalPickerValue;

        partial void OnWaterIntervalPickerValueChanged(Interval value)
        {
            if (value.DaysFromNow == -1)
            {
                CustomWaterInteravlGridVisible = true;
                value.DaysFromNow = 0;
            } else
            {
                CustomWaterInteravlGridVisible = false;
            }
        }




        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            InitialPlant = query["Plant"] as Plant;
            OnPropertyChanged("Plant");
        }

        



        [RelayCommand]
        void UseRefreshingPressed(bool value)
        {
            RefreshGridVisible = value ? false : true;
            RefreshGridText = value ? "Needs Refreshing" : "Remove Refreshing";
        }

        [RelayCommand]
        async Task AddUpdateAsync(Plant plant)
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;
                await plantService.AddUpdateNewPlantAsync(plant);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unable to add or update plants: {ex.Message}");
                await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
                FriendlyLabel = plantService.StatusMessage;
            }
        }

        [RelayCommand]
        async Task DeleteAsync(Plant plant)
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;
                await plantService.DeletePlantAsync(plant);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unable to add or update plants: {ex.Message}");
                await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        async Task GoToTableAsync()
        {
            await Shell.Current.GoToAsync("..");
        }



        [ObservableProperty]
        Plant plant;
    }
}
