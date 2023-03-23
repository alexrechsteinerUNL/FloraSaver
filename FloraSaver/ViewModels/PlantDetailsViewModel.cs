using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FloraSaver.Models;
using FloraSaver.Services;
using Plugin.LocalNotification;

namespace FloraSaver.ViewModels
{


    [QueryProperty(nameof(Plant), "Plant")]
    public partial class PlantDetailsViewModel : BaseViewModel, IQueryAttributable
    {
        PlantService plantService;
        NotificationService notificationService;

        public Plant InitialPlant { get; set; }

        public PlantDetailsViewModel(PlantService PlantService, NotificationService NotificationService)
        {
            plantService = PlantService;
            notificationService = NotificationService;
            wateringInterval = PickerService.GetIntervals();

        }
        [ObservableProperty]
        List<Interval> wateringInterval;

        [ObservableProperty]
        public bool customWaterIntervalGridVisible = false;

        [ObservableProperty]
        public bool waterGridVisible = false;

        [ObservableProperty]
        public string waterGridText = "Use Watering";

        [ObservableProperty]
        public bool mistGridVisible = false;

        [ObservableProperty]
        public string mistGridText = "Use Misting";

        [ObservableProperty]
        public bool sunGridVisible = false;

        [ObservableProperty]
        public string sunGridText = "Use Sunlight Move";

        [ObservableProperty]
        public Interval waterIntervalPickerValue;

        [ObservableProperty]
        public TimeSpan lastWaterTime;
        [ObservableProperty]
        public TimeSpan nextWaterTime;
        [ObservableProperty]
        public TimeSpan lastMistTime;
        [ObservableProperty]
        public TimeSpan nextMistTime;
        [ObservableProperty]
        public TimeSpan lastSunTime;
        [ObservableProperty]
        public TimeSpan nextSunTime;
        [ObservableProperty]
        private int daysFromNow = 0;

        partial void OnWaterIntervalPickerValueChanged(Interval value)
        {
            if (value.DaysFromNow == -1)
            {
                CustomWaterIntervalGridVisible = true;
            } else
            {
                CustomWaterIntervalGridVisible = false;
            }
            DaysFromNow = value.DaysFromNow;
        }




        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            InitialPlant = query["Plant"] as Plant;
            OnPropertyChanged("Plant");
        }

        public Plant SetPlantValues(Plant plant)
        {
            plant.DateOfLastWatering = plant.DateOfLastWatering.Date + plant.TimeOfLastWatering;
            plant.DateOfNextWatering = plant.DateOfNextWatering.Date + plant.TimeOfNextWatering;
            plant.DateOfLastMisting = plant.DateOfLastMisting.Date + plant.TimeOfLastMisting;
            plant.DateOfNextMisting = plant.DateOfNextMisting.Date + plant.TimeOfNextMisting;
            if (DaysFromNow != -1)
            {
                plant.WaterInterval = DaysFromNow;
            }
            return plant;
        }


        [RelayCommand]
        void UseWateringPressed(bool value)
        {
            WaterGridVisible = value ? false : true;
            WaterGridText = value ? "Use Watering" : "Do Not Use Watering";
        }

        [RelayCommand]
        void UseMistingPressed(bool value)
        {
            MistGridVisible = value ? false : true;
            MistGridText = value ? "Use Misting" : "Do Not Use Misting";
        }

        [RelayCommand]
        void UseSunPressed(bool value)
        {
            SunGridVisible = value ? false : true;
            SunGridText = value ? "Use Sunlight Move" : "Do Not Use Sunlight Move";
        }

        [RelayCommand]
        async Task AddUpdateAsync(Plant plant)
        {
            if (IsBusy)
                return;

            try
            {
                plant = SetPlantValues(plant);

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
