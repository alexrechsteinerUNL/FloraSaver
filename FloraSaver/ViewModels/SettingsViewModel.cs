﻿using FloraSaver.Services;
using System.Diagnostics;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using FloraSaver.Models;
using SQLite;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FloraSaver.ViewModels
{
    public partial class SettingsViewModel : TableViewModel, INotifyPropertyChanged
    {
        private ObservableCollection<PlantGroup> visiblePlantGroups = new();

        public ObservableCollection<PlantGroup> VisiblePlantGroups
        {
            get { return visiblePlantGroups; }
            set
            {
                SetObservableProperty(ref visiblePlantGroups, value);
            }
        }

        private List<PlantGroup> initialPlantGroups = new();

        private List<PlantGroup> pickerPlantGroups;

        public List<PlantGroup> PickerPlantGroups
        {
            get { return pickerPlantGroups; }
            set
            {
                SetObservableProperty(ref pickerPlantGroups, value);
            }
        }

        public string ShowTutorialButtonText => ShowTutorial ? "Hide Tutorials" : "Show Tutorials";

        [ObservableProperty]
        protected List<Interval> overduePlantsInterval;
        [ObservableProperty]
        public Interval overduePlantsPickerValue;

        [ObservableProperty]
        protected List<Interval> overduePlantsMultiInterval;
        [ObservableProperty]
        public Interval overduePlantsMultiPickerValue;
        [ObservableProperty]
        protected bool areNoGroups = true;

        [ObservableProperty]
        protected bool isBeingUndone = false;
        [ObservableProperty]
        protected bool nameEntryUndoButtonVisible = false;
        
        [RelayCommand]
        public void GroupNameEdit()
        {
            if (!IsInitialization && !IsBeingUndone && PickerPlantGroups is not null)
            {
                //foreach (var group in PickerPlantGroups.Where(_ => !initialPlantGroups.Select(_ => _.GroupName).Contains(_.GroupName)))
                foreach (var group in PickerPlantGroups.Where(_ => initialPlantGroups.FirstOrDefault(x => x.GroupId == _.GroupId).GroupName != _.GroupName))
                {
                    group.isNameEdited = true;
                    group.Validate(initialPlantGroups.Select(_ => _.GroupName).ToList());
                }
                NameEntryUndoButtonVisible = true;
                SetItem();
                OnPropertyChanged(nameof(VisiblePlantGroups));
            }
            return;
        }
        [RelayCommand]
        protected void NameSectionUndo(PlantGroup group)
        {
            IsBeingUndone = true;
            PickerPlantGroups.FirstOrDefault(_ => _.GroupId == group.GroupId).GroupName = initialPlantGroups?.FirstOrDefault(_ => _.GroupId == group.GroupId).GroupName ?? PickerPlantGroups.FirstOrDefault(_ => _.GroupId == group.GroupId).GroupName;
            IsBeingUndone = false;
            group.isNameEdited = false;
            NameEntryUndoButtonVisible = false;
            group.Validate(initialPlantGroups.Where(_ => _.GroupId != group.GroupId).Select(_ => _.GroupName).ToList());
            SetItem();
            OnPropertyChanged(nameof(VisiblePlantGroups));
        }

        [ObservableProperty]
        protected bool colorEntryUndoButtonVisible = false;
        [RelayCommand]
        public void GroupColorEdit()
        {
            if (!IsInitialization && !IsBeingUndone)
            {
                foreach (var group in PickerPlantGroups.Where(_ => !initialPlantGroups.Select(_ => _.GroupColorHex).Contains(_.GroupColorHex)))
                {
                    group.isColorEdited = true;
                    group.Validate(initialPlantGroups.Select(_ => _.GroupName).ToList());
                }
                ColorEntryUndoButtonVisible = true;
                SetItem();
                OnPropertyChanged(nameof(VisiblePlantGroups));
            }
            return;
        }
        [RelayCommand]
        protected void ColorSectionUndo(PlantGroup group)
        {
            IsBeingUndone = true;
            PickerPlantGroups.FirstOrDefault(_ => _.GroupId == group.GroupId).GroupColorHex = initialPlantGroups?.FirstOrDefault(_ => _.GroupId == group.GroupId).GroupColorHex ?? PickerPlantGroups.FirstOrDefault(_ => _.GroupId == group.GroupId).GroupColorHex;
            IsBeingUndone = false;
            group.isColorEdited = false;
            group.Validate(initialPlantGroups.Where(_ => _.GroupId != group.GroupId).Select(_ => _.GroupName).ToList());
            ColorEntryUndoButtonVisible = false;
            SetItem();
            OnPropertyChanged(nameof(VisiblePlantGroups));
        }

        

        [ObservableProperty]
        public Plant alterPlant;

        [RelayCommand]
        public void SetItem()
        {
            if (PickerPlantGroups is not null)
            {
                VisiblePlantGroups = new ObservableCollection<PlantGroup>(PickerPlantGroups);
            }
        }

        protected void SetObservableProperty<T>(ref T field, T value,
        [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return;
            field = value;
            OnPropertyChanged(propertyName);
        }

        public SettingsViewModel(IDatabaseService databaseService, IPlantNotificationService plantNotificationService) : base(databaseService, plantNotificationService)
        {
            IsInitialization = true;
            databaseService = _databaseService;
            plantNotificationService = _plantNotificationService;
            
            OverduePlantsInterval = PickerService.GetCooldownBeforePlantActionsOverdueNotification();
            OverduePlantsMultiInterval = PickerService.GetInActionBeforeMultiOverdueNotification();
            HumidityIntervals = PickerService.GetHumidityPercent();
            TemperatureIntervalsF = PickerService.GetTemperatureF();
            TemperatureIntervalsC = PickerService.GetTemperatureC();
            MorningTime = new TimeSpan(0, 0, 0);
            MiddayTime = new TimeSpan(0, 0, 0);
            NightTime = new TimeSpan(0, 0, 0);

            DateTime morningDate = DateTime.FromBinary(Preferences.Default.Get("morning_time_date", new DateTime(1, 1, 1, 8, 0, 0).ToBinary()));
            MorningTime = morningDate.TimeOfDay;
            var middayDate = DateTime.FromBinary(Preferences.Default.Get("midday_time_date", new DateTime(1, 1, 1, 12, 0, 0).ToBinary()));
            MiddayTime = middayDate.TimeOfDay;
            var nightDate = DateTime.FromBinary(Preferences.Default.Get("night_time_date", new DateTime(1, 1, 1, 16, 0, 0).ToBinary()));
            NightTime = nightDate.TimeOfDay;

            OverduePlantsPickerValue = OverduePlantsInterval.FirstOrDefault(_ => _.NumFromNow == double.Parse(Preferences.Default.Get("overdue_plants_time_to", "1"))) ?? new Interval() { IntervalText = "1 Hour", NumFromNow = 1 };
            OverduePlantsMultiPickerValue = OverduePlantsMultiInterval.FirstOrDefault(_ => _.NumFromNow == double.Parse(Preferences.Default.Get("overdue_plants_multi_time_to", "24"))) ?? new Interval() { IntervalText = "1 Day", NumFromNow = 24 };
            
            HumidityIntervalPickerValue = HumidityIntervals.FirstOrDefault(_ => _.HumidityLevel == Preferences.Default.Get("humidity_level", 44)) ?? new HumidityInterval() { HumidityLevel = 30, IntervalText = "Normal Indoor Humidity" };
            
            IsCelsius = Preferences.Default.Get("is_Celsius", false);
            TemperatureIntervalPickerValueC = TemperatureIntervalsC.FirstOrDefault(_ => _.TemperatureLevel == Preferences.Default.Get("temperature_level", 60)) ?? new TemperatureInterval() { TemperatureLevel = 60, IntervalText = "Normal Indoor Temperatures", IsCelsius = true };
            TemperatureIntervalPickerValueF = TemperatureIntervalsF.FirstOrDefault(_ => _.TemperatureLevel == Preferences.Default.Get("temperature_level", 60)) ?? new TemperatureInterval() { TemperatureLevel = 60, IntervalText = "Normal Indoor Temperatures"};
            IsInitialization = false;
        }

        [ObservableProperty]
        private bool isInitialization;
        [RelayCommand]
        private async Task AppearingSettingsAsync()
        {
            IsInitialization = true;
            PickerPlantGroups = await _databaseService.GetAllPlantGroupAsync();
            PickerPlantGroups = PickerPlantGroups.Where(_ => !string.Equals(_.GroupName, "Ungrouped")).ToList();
            foreach (var group in PickerPlantGroups)
            {
                if (!string.Equals(group.GroupName, "Ungrouped"))
                {
                    initialPlantGroups.Add(new PlantGroup(group));
                }

            }
            await GetVisiblePlantGroupsAsync(); ShouldUpdateCheckService.shouldGetNewGroupDataSettings = false; 
            if (initialPlantGroups.Count > 0)
            {
                AreNoGroups = false;
            } else
            {
                AreNoGroups = true;
            }
            // This is to resolve the fact that TimePicker cannot have its time centered which would cut off the correct values
            MorningTime = new TimeSpan(0, 0, 0);
            MiddayTime = new TimeSpan(0, 0, 0);
            NightTime = new TimeSpan(0, 0, 0);
            DateTime morningDate = DateTime.FromBinary(Preferences.Default.Get("morning_time_date", new DateTime(1, 1, 1, 8, 0, 0).ToBinary()));
            MorningTime = morningDate.TimeOfDay;
            var middayDate = DateTime.FromBinary(Preferences.Default.Get("midday_time_date", new DateTime(1, 1, 1, 12, 0, 0).ToBinary()));
            MiddayTime = middayDate.TimeOfDay;
            var nightDate = DateTime.FromBinary(Preferences.Default.Get("night_time_date", new DateTime(1, 1, 1, 16, 0, 0).ToBinary()));
            NightTime = nightDate.TimeOfDay;
            ShowTutorial = Preferences.Default.Get("show_tutorial", true);
            IsInitialization = false;
        }

        [ObservableProperty]
        private TimeSpan morningTime;

        [ObservableProperty]
        private TimeSpan middayTime;

        [ObservableProperty]
        private TimeSpan nightTime;

        [RelayCommand]
        private void ToggleTutorials()
        {
            ShowTutorial = !ShowTutorial;
            Preferences.Default.Set("show_tutorial", ShowTutorial);
            OnPropertyChanged(nameof(ShowTutorialButtonText)); 
        }

        [RelayCommand]
        public async Task ValidateGroupAsync(int includedGroupId = -1)
        {
            foreach(var group in PickerPlantGroups)
            {
                if (includedGroupId != -1)
                {
                    group.Validate(initialPlantGroups.Where(_ => _.GroupId != includedGroupId).Select(_ => _.GroupName).ToList());
                } else
                {
                    group.Validate(new());
                }
                
            }
        }

        [RelayCommand]
        partial void OnMorningTimeChanged(TimeSpan value)
        {
            if (!IsInitialization)
            {
                var morningTimeDate = new DateTime().Add(value).ToBinary();
                Preferences.Default.Set("morning_time_date", morningTimeDate);
            }
            
        }

        [RelayCommand]
        partial void OnMiddayTimeChanged(TimeSpan value)
        {
            if (!IsInitialization)
            {
                var middayTimeDate = new DateTime().Add(value).ToBinary();
                Preferences.Default.Set("midday_time_date", middayTimeDate);
            }
            
        }

        [RelayCommand]
        partial void OnNightTimeChanged(TimeSpan value)
        {
            if (!IsInitialization)
            {
                var nightTimeDate = new DateTime().Add(value).ToBinary();
                Preferences.Default.Set("night_time_date", nightTimeDate);
            }

        }

        [RelayCommand]
        partial void OnOverduePlantsPickerValueChanged(Interval value)
        {
            Preferences.Default.Set("overdue_plants_time_to", value.NumFromNow);
            OverduePlantsPickerValue = value;
        }

        [RelayCommand]
        partial void OnOverduePlantsMultiPickerValueChanged(Interval value)
        {
            Preferences.Default.Set("overdue_plants_multi_time_to", value.NumFromNow);
            OverduePlantsMultiPickerValue = value;
        }

        

        [RelayCommand]
        public void ChangeTemperatureMetrics()
        {
            if (IsCelsius)
            {
                 IsCelsius = false;

            } else
            {
                IsCelsius = true;
            }
            TemperatureIntervalPickerValueC = TemperatureIntervalPickerValueC;
            TemperatureIntervalPickerValueF = TemperatureIntervalPickerValueF;
            Preferences.Default.Set("is_Celsius", IsCelsius);
            OnPropertyChanged(nameof(IsCelsius));
            OnPropertyChanged(nameof(IsFahrenheit));
            OnPropertyChanged(nameof(TemperatureIntervalPickerValueF));
            OnPropertyChanged(nameof(TemperatureIntervalPickerValueC));

        }

        [RelayCommand]
        protected void VisiblePlantAttentionGetter()
        {
            VisiblePlantGroups = new ObservableCollection<PlantGroup>(VisiblePlantGroups);
        }

        [RelayCommand]
        public async Task CheckForAutofillUpdatesAsync()
        {
            await _databaseService.DeepUpdateAutofillPlantsAsync();
            FriendlyLabel = _databaseService.StatusMessage;
            await FriendlyLabelToastAsync();
        }

        [RelayCommand]
        public async Task CheckForClipetUpdatesAsync()
        {
            await _databaseService.DeepUpdateClipetDialogAsync();
            FriendlyLabel = _databaseService.StatusMessage;
            await FriendlyLabelToastAsync();
        }

        [RelayCommand]
        protected async Task GetVisiblePlantGroupsAsync()
        {
            IsBusy = true;
            IsRefreshing = true;
            await GetPlantGroupsAsync();
            if (IsInitialization)
            {
                VisiblePlantGroups = new ObservableCollection<PlantGroup>(PickerPlantGroups);
            }
            //this redundant call is due to a Bug in maui that makes observable collections not realized they've been altered
            VisiblePlantGroups = new ObservableCollection<PlantGroup>(VisiblePlantGroups);
            OnPropertyChanged(nameof(VisiblePlantGroups));
            IsBusy = false;
            IsRefreshing = false;
        }

        [RelayCommand]
        private async Task SaveGroupChangeAsync(PlantGroup plantGroup)
        {
            plantGroup.Validate(initialPlantGroups.Where(_ => _.GroupId != plantGroup.GroupId).Select(_ => _.GroupName).ToList());
            if (plantGroup.Validation.IsSuccessful)
            {
                await _databaseService.AddUpdateNewPlantGroupAsync(plantGroup, true, initialPlantGroups.FirstOrDefault(_ => _.GroupId == plantGroup.GroupId).GroupName);
                initialPlantGroups.FirstOrDefault(_ => _.GroupId == plantGroup.GroupId).GroupName = plantGroup.GroupName;
                initialPlantGroups.FirstOrDefault(_ => _.GroupId == plantGroup.GroupId).GroupColorHex = plantGroup.GroupColorHex;
                PickerPlantGroups.FirstOrDefault(_ => _.Equals(plantGroup)).isNameEdited = false;
                PickerPlantGroups.FirstOrDefault(_ => _.Equals(plantGroup)).isColorEdited = false;
                SetItem();
                ShouldUpdateCheckService.ForceToGetNewGroupData();
                ShouldUpdateCheckService.ForceToGetNewPlantData();
            } else
            {
                await Application.Current.MainPage.DisplayAlert("OH HOLD ON!", plantGroup.Validation.Message, "Gotcha");
            }
            
        }

        [RelayCommand]
        private async Task DeletePlantGroupAsync(PlantGroup plantGroup)
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;
                bool reallyDelete = await Application.Current.MainPage.DisplayAlert("OH HOLD ON!", $"Are you sure you want to delete your plant group: '{plantGroup.GroupName}'?", "Delete It", "Please Don't");
                if (reallyDelete)
                {
                    await _databaseService.DeletePlantGroupAsync(plantGroup);
                    PickerPlantGroups.Remove(plantGroup);
                    IsBusy = false;
                    SetItem();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unable to delete plants: {ex.Message}");
                await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
                OnPropertyChanged("PlantGroup");
                ShouldUpdateCheckService.ForceToGetNewGroupData();
                ShouldUpdateCheckService.ForceToGetNewPlantData();
            }
        }

        [RelayCommand]
        private async Task ShowOpenNoticeAsync()
        {
            await Application.Current.MainPage.DisplayAlert("The following 3rd Party NuGet Packages Made This App Possible", "• Plugin.LocalNotification\n• sqlite-net-pcl\n• SQLitePCLRaw.provider.dynamic_cdecl", "Nice");
        }

        [RelayCommand]
        private async Task ShowCreditsAsync()
        {
            await Application.Current.MainPage.DisplayAlert("", "This App was made with love by CrackpotOpus", "Sweet");
        }

        [RelayCommand]
        private async Task ClearAllPlantGroupDataAsync()
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
                ShouldUpdateCheckService.ForceToGetNewGroupData();
                ShouldUpdateCheckService.ForceToGetNewPlantData();
            }
        }

        [RelayCommand]
        private async Task ClearAllPlantDataAsync()
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
                ShouldUpdateCheckService.ForceToGetNewPlantData();
            }
        }
    }
}