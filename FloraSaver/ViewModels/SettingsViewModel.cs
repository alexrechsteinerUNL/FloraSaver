using FloraSaver.Services;
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
        [ObservableProperty]
        protected bool isBeingUndone = false;
        [ObservableProperty]
        protected bool nameEntryUndoButtonVisible = false;
        [RelayCommand]
        public void GroupNameEdit()
        {
            if (!IsInitialization && !IsBeingUndone)
            {
                foreach (var plant in PickerPlantGroups.Where(_ => !initialPlantGroups.Select(_ => _.GroupName).Contains(_.GroupName)))
                {
                    plant.isNameEdited = true;
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
            SetItem();
            OnPropertyChanged(nameof(VisiblePlantGroups));
            IsBeingUndone = false;
            group.isColorEdited = false;
            NameEntryUndoButtonVisible = false;
        }













        private List<PlantGroup> initialPlantGroups = new();

        private ObservableCollection<PlantGroup> visiblePlantGroups = new();

        public ObservableCollection<PlantGroup> VisiblePlantGroups
        {
            get { return visiblePlantGroups; }
            set
            {
                SetObservableProperty(ref visiblePlantGroups, value);
            }
        }

        private List<PlantGroup> pickerPlantGroups;

        public List<PlantGroup> PickerPlantGroups
        {
            get { return pickerPlantGroups; }
            set
            {
                SetObservableProperty(ref pickerPlantGroups, value);
            }
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
            databaseService = _databaseService;
            plantNotificationService = _plantNotificationService;

            DateTime morningDate = DateTime.FromBinary(Preferences.Default.Get("morning_time_date", new DateTime(1, 1, 1, 8, 0, 0).ToBinary()));
            morningTime = morningDate.TimeOfDay;
            var middayDate = DateTime.FromBinary(Preferences.Default.Get("midday_time_date", new DateTime(1, 1, 1, 12, 0, 0).ToBinary()));
            middayTime = middayDate.TimeOfDay;
            var nightDate = DateTime.FromBinary(Preferences.Default.Get("night_time_date", new DateTime(1, 1, 1, 16, 0, 0).ToBinary()));
            nightTime = nightDate.TimeOfDay;
        }

        [ObservableProperty]
        private bool isInitialization;

        [RelayCommand]
        private async Task AppearingSettingsAsync()
        {
            IsInitialization = true;
            PickerPlantGroups = await _databaseService.GetAllPlantGroupAsync();
            foreach (var group in PickerPlantGroups)
            {
                initialPlantGroups.Add(new PlantGroup(group));
            }
            await GetVisiblePlantGroupsAsync();
            IsInitialization = false;
        }

        //public void OnPlantGroupChanged()
        //{
        //    OnPropertyChanged(nameof(VisiblePlantGroups));
        //}

        [ObservableProperty]
        private TimeSpan morningTime;

        [ObservableProperty]
        private TimeSpan middayTime;

        [ObservableProperty]
        private TimeSpan nightTime;

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
        private async Task GoToDatabaseExportAsync()
        {
            await Shell.Current.GoToAsync($"{nameof(DatabaseExportPage)}", true);
        }

        [RelayCommand]
        private async Task GoToDatabaseImportAsync()
        {
            await Shell.Current.GoToAsync($"{nameof(DatabaseImportPage)}", true);
        }

        [RelayCommand]
        public void UpdateColor(GroupColors value)
        {
            Console.WriteLine("bloop");
        }

        [RelayCommand]
        public void SetColor(GroupColors value)
        {
            Console.WriteLine("bloop");
        }


        [RelayCommand]
        protected void VisiblePlantAttentionGetter()
        {
            VisiblePlantGroups = new ObservableCollection<PlantGroup>(VisiblePlantGroups);
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
            await _databaseService.AddUpdateNewPlantGroupAsync(plantGroup);
            initialPlantGroups.FirstOrDefault(_ => _.GroupId == plantGroup.GroupId).GroupName = plantGroup.GroupName;
            initialPlantGroups.FirstOrDefault(_ => _.GroupId == plantGroup.GroupId).GroupColorHex = plantGroup.GroupColorHex;
            PickerPlantGroups.FirstOrDefault(_ => _.Equals(plantGroup)).isNameEdited = false;
            PickerPlantGroups.FirstOrDefault(_ => _.Equals(plantGroup)).isColorEdited = false;
            SetItem();
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
            }
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
            }
        }
    }
}