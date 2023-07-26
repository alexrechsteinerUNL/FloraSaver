using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FloraSaver.Models;
using FloraSaver.Services;
using Microsoft.Extensions.Logging.Abstractions;
using Plugin.LocalNotification;

namespace FloraSaver.ViewModels
{


    [QueryProperty(nameof(Plant), "Plant")]
    [QueryProperty(nameof(PlantGroup), "PlantGroup")]
    public partial class PlantDetailsViewModel : BaseViewModel, IQueryAttributable, INotifyPropertyChanged
    {
        // I moved the _databaseService to the base viewmodel because just about every page was going to use it.
        

        [ObservableProperty]
        public Plant initialPlant;

        private Random rand = new Random();

        private List<GroupColors> groupoColors = PickerService.GetSelectableColors();
        
        [ObservableProperty]
        public Plant alterPlant;

        [ObservableProperty]
        public List<ClipetSpeechBubble> speechBubbles;

        public ObservableCollection<PlantGroup> PlantGroups { get; set; } = new();

        public PlantDetailsViewModel(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
            wateringInterval = PickerService.GetWaterIntervals();
            mistingInterval = PickerService.GetWaterIntervals();
            sunInterval = PickerService.GetWaterIntervals();
            var bloop = PickerService.GetSelectableColors();
        }

        [RelayCommand]
        void Appearing()
        {
            IsInitialization = true;
            // extract to its own reusable method with reflection DRY!

            GroupPickerValue = initialPlant.PlantGroupName != null ? PlantGroups.FirstOrDefault(_ => _.GroupName == initialPlant.PlantGroupName) : PlantGroups.FirstOrDefault(_ => _.GroupName == "Ungrouped");

            WaterDaysFromNow = initialPlant.WaterInterval != null ? (int)initialPlant.WaterInterval : (initialPlant.DateOfNextWatering.Date - initialPlant.DateOfLastWatering.Date).Days;
            WaterIntervalPickerValue = WateringInterval.FirstOrDefault(x => x.DaysFromNow == WaterDaysFromNow);
            if (WaterIntervalPickerValue == null)
            {
                WaterIntervalPickerValue = WateringInterval.First(x => x.DaysFromNow == -1);
            }

            MistDaysFromNow = initialPlant.MistInterval != null ? (int)initialPlant.MistInterval : (initialPlant.DateOfNextMisting.Date - initialPlant.DateOfLastMisting.Date).Days;
            MistIntervalPickerValue = MistingInterval.FirstOrDefault(x => x.DaysFromNow == MistDaysFromNow);
            if (MistIntervalPickerValue == null)
            {
                MistIntervalPickerValue = MistingInterval.First(x => x.DaysFromNow == -1);
            }

            SunDaysFromNow = initialPlant.SunInterval != null ? (int)initialPlant.SunInterval : (initialPlant.DateOfNextMove.Date - initialPlant.DateOfLastMove.Date).Days;
            SunIntervalPickerValue = SunInterval.FirstOrDefault(x => x.DaysFromNow == SunDaysFromNow);
            if (SunIntervalPickerValue == null)
            {
                SunIntervalPickerValue = SunInterval.First(x => x.DaysFromNow == -1);
            }

            WaterGridText = InitialPlant.UseWatering ? "Do Not Use Watering" : "Use Watering";
            MistGridText = InitialPlant.UseMisting ? "Do Not Use Misting" : "Use Misting";
            SunGridText = InitialPlant.UseMoving ? "Do Not Use Sunlight Move" : "Use Sunlight Move";

            IsInitialization = false;
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            IsInitialization = true;
            InitialPlant = AlterPlant = query["Plant"] as Plant;
            OnPropertyChanged("Plant");
            var groups = query["PlantGroup"] as List<PlantGroup>;
            PlantGroups = new ObservableCollection<PlantGroup>(groups);
            OnPropertyChanged("PlantGroups");
            IsInitialization = false;
        }


        [ObservableProperty]
        bool isInitialization;


        [ObservableProperty]
        public bool addNewGroupGridVisible = false;
        [ObservableProperty]
        public string addGroupButtonText = "+";

        [ObservableProperty]
        public PlantGroup groupPickerValue;

        [ObservableProperty]
        List<Interval> wateringInterval;
        [ObservableProperty]
        List<Interval> mistingInterval;
        [ObservableProperty]
        List<Interval> sunInterval;

        [ObservableProperty]
        public bool customWaterIntervalGridVisible = false;
        [ObservableProperty]
        public bool customMistIntervalGridVisible = false;
        [ObservableProperty]
        public bool customSunIntervalGridVisible = false;

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
        public Interval mistIntervalPickerValue;
        [ObservableProperty]
        public Interval sunIntervalPickerValue;

        [ObservableProperty]
        public int waterDaysFromNow;
        [ObservableProperty]
        public int mistDaysFromNow;
        [ObservableProperty]
        public int sunDaysFromNow;

        partial void OnGroupPickerValueChanged(PlantGroup value)
        {
            if (!IsInitialization)
            {
                AlterPlant.PlantGroupName = value.GroupName;
                AlterPlant.GroupColorHexString = value.GroupColorHex;
                OnPropertyChanged("AlterPlant");
            }
         }

        partial void OnWaterDaysFromNowChanged(int value)
        {
            if (!IsInitialization)
            {
                AlterPlant.DateOfNextWatering = AlterPlant.DateOfLastWatering.AddDays(value);
            }
            
            AlterPlant.WaterInterval = value;
            OnPropertyChanged("AlterPlant");
        }

        partial void OnWaterIntervalPickerValueChanged(Interval value)
        {
            if (value.DaysFromNow == -1)
            {
                CustomWaterIntervalGridVisible = true;
            } else
            {
                CustomWaterIntervalGridVisible = false;
                WaterDaysFromNow = value.DaysFromNow;
            }
        }

        partial void OnMistDaysFromNowChanged(int value)
        {
            if (!IsInitialization)
            {
                AlterPlant.DateOfNextMisting = AlterPlant.DateOfLastMisting.AddDays(value);
            }
            
            AlterPlant.MistInterval = value;
            OnPropertyChanged("AlterPlant");
        }

        partial void OnMistIntervalPickerValueChanged(Interval value)
        {
            if (value.DaysFromNow == -1)
            {
                CustomMistIntervalGridVisible = true;
            }
            else
            {
                CustomMistIntervalGridVisible = false;
                MistDaysFromNow = value.DaysFromNow;
            }
        }

        partial void OnSunDaysFromNowChanged(int value)
        {
            if (!IsInitialization)
            {
                AlterPlant.DateOfNextMove = AlterPlant.DateOfLastMove.AddDays(value);
            }
            
            AlterPlant.SunInterval = value;
            OnPropertyChanged("AlterPlant");
        }

        partial void OnSunIntervalPickerValueChanged(Interval value)
        {
            if (value.DaysFromNow == -1)
            {
                CustomSunIntervalGridVisible = true;
            }
            else
            {
                CustomSunIntervalGridVisible = false;
                SunDaysFromNow = value.DaysFromNow;
            }
        }


        public Plant SetPlantValues(Plant plant)
        {
            if (WaterDaysFromNow != -1)
            {
                plant.WaterInterval = WaterDaysFromNow;
            }

            return plant;
        }

        [RelayCommand]
        void AddGroupShowPressed()
        {
            AddNewGroupGridVisible = !AddNewGroupGridVisible;
            AddGroupButtonText = AddNewGroupGridVisible ? "-" : "+";
        }

        [RelayCommand]
        async Task AddNewGroupAsync(string newPlantGroupName)
        {
            var newPlantGroup = new PlantGroup()
            {
                GroupId = PlantGroups.Any() ? PlantGroups.Max(x => x.GroupId) + 1 : 0,
                GroupName = newPlantGroupName,
                GroupColorHex = $"#{rand.Next(0x1000000):X6}",
            };
            var result = await AddUpdateGroupAsync(newPlantGroup);
            if (result)
            {
                PlantGroups.Add(newPlantGroup);
                OnPropertyChanged("PlantGroups");
                AddGroupShowPressed();
                GroupPickerValue = newPlantGroup;
            }
            
        }


        [RelayCommand]
        void UseWateringPressed(bool value)
        {
            AlterPlant.UseWatering = !AlterPlant.UseWatering;
            WaterGridText = AlterPlant.UseWatering ? "Do Not Use Watering" : "Use Watering";
            OnPropertyChanged("AlterPlant");
        }

        [RelayCommand]
        void UseMistingPressed(bool value)
        {
            AlterPlant.UseMisting = !AlterPlant.UseMisting;
            MistGridText = AlterPlant.UseMisting ? "Do Not Use Misting" : "Use Misting";
            OnPropertyChanged("AlterPlant");
        }

        [RelayCommand]
        void UseSunPressed(bool value)
        {
            AlterPlant.UseMoving = !AlterPlant.UseMoving;
            SunGridText = AlterPlant.UseMoving ? "Do Not Use Sunlight Move" : "Use Sunlight Move";
            OnPropertyChanged("AlterPlant");
        }

        [RelayCommand]
        async Task<bool> AddUpdateGroupAsync(PlantGroup plantGroup)
        {
            var result = false;
            if (IsBusy)
                return result;

            try
            {
                //plant = SetPlantValues(plant);

                IsBusy = true;
                await _databaseService.AddUpdateNewPlantGroupAsync(plantGroup);
                result = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unable to add or update plant group: {ex.Message}");
                await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");
                result = false;
            }
            finally
            {
                IsBusy = false;
                FriendlyLabel = _databaseService.StatusMessage;
                
            }
            return result;
        }

        [RelayCommand]
        async Task AddUpdateAsync(Plant plant)
        {
            if (IsBusy)
                return;

            try
            {
                //plant = SetPlantValues(plant);

                IsBusy = true;
                await _databaseService.AddUpdateNewPlantAsync(plant);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unable to add or update plants: {ex.Message}");
                await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
                FriendlyLabel = _databaseService.StatusMessage;
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
                await _databaseService.DeletePlantAsync(plant);
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

        [RelayCommand]
        private void SetToDefaultMorningTime(string timeValue)
        {
            alterPlant.GetType().GetProperty(timeValue).SetValue(alterPlant, DateTime.FromBinary(Preferences.Default.Get("morning_time_date", new DateTime(1, 1, 1, 8, 0, 0).ToBinary())).TimeOfDay);
            OnPropertyChanged("AlterPlant");
        }

        [RelayCommand]
        private void SetToDefaultMiddayTime(string timeValue)
        {
            alterPlant.GetType().GetProperty(timeValue).SetValue(alterPlant, DateTime.FromBinary(Preferences.Default.Get("midday_time_date", new DateTime(1, 1, 1, 12, 0, 0).ToBinary())).TimeOfDay);
            OnPropertyChanged("AlterPlant");
        }

        [RelayCommand]
        private void SetToDefaultNightTime(string timeValue)
        {
            alterPlant.GetType().GetProperty(timeValue).SetValue(alterPlant, DateTime.FromBinary(Preferences.Default.Get("night_time_date", new DateTime(1, 1, 1, 16, 0, 0).ToBinary())).TimeOfDay);
            OnPropertyChanged("AlterPlant");
        }

        [ObservableProperty]
        Plant plant;
    }
}
