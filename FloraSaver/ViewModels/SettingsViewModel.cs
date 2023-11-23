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
            
            DateTime morningDate = DateTime.FromBinary(Preferences.Default.Get("morning_time_date", new DateTime(1,1,1,8,0,0).ToBinary()));
            morningTime = morningDate.TimeOfDay;
            var middayDate = DateTime.FromBinary(Preferences.Default.Get("midday_time_date", new DateTime(1, 1, 1, 12, 0, 0).ToBinary()));
            middayTime = middayDate.TimeOfDay;
            var nightDate = DateTime.FromBinary(Preferences.Default.Get("night_time_date", new DateTime(1, 1, 1, 16, 0, 0).ToBinary()));
            nightTime = nightDate.TimeOfDay;
        }

        [ObservableProperty]
        bool isInitialization;

        [RelayCommand]
        async Task AppearingSettingsAsync()
        {
            IsInitialization = true;
            PickerPlantGroups = await _databaseService.GetAllPlantGroupAsync();
            await GetVisiblePlantGroupsAsync();

            IsInitialization = false;
        }

        //public void OnPlantGroupChanged()
        //{
        //    OnPropertyChanged(nameof(VisiblePlantGroups));
        //}

        [ObservableProperty]
        TimeSpan morningTime;
        [ObservableProperty]
        TimeSpan middayTime;
        [ObservableProperty]
        TimeSpan nightTime;


        //private GroupColors groupSelection;
        //public GroupColors GroupSelection
        //{
        //    get => groupSelection;
        //    set
        //    {
        //        SetProperty(ref groupSelection, value);
        //        VisiblePlantAttentionGetter();
        //    }
        //}

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
        async Task GoToDatabaseExportAsync()
        {
            await Shell.Current.GoToAsync($"{nameof(DatabaseExportPage)}", true);
        }

        [RelayCommand]
        async Task GoToDatabaseImportAsync()
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
        public void GroupNameEdit(PlantGroup plantGroup)
        {
            if (plantGroup is not null)
            {
                PickerPlantGroups.FirstOrDefault(_ => _.Equals(plantGroup)).isEdited = true;
                SetItem();
                OnPropertyChanged(nameof(VisiblePlantGroups));
            }
            
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
            if (isInitialization)
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
        async Task ResetGroupChangeAsync(PlantGroup plantGroup)
        {
            //not working... must figure out why
            PickerPlantGroups[PickerPlantGroups.IndexOf(PickerPlantGroups.FirstOrDefault(_ => _.GroupId == plantGroup.GroupId))] = VisiblePlantGroups.FirstOrDefault(_ => _.GroupId == plantGroup.GroupId);
            PickerPlantGroups.FirstOrDefault(_ => _.Equals(plantGroup)).isEdited = false;
            SetItem();

        }

        [RelayCommand]
        async Task SaveGroupChangeAsync(PlantGroup plantGroup)
        {
            await _databaseService.AddUpdateNewPlantGroupAsync(plantGroup);
            PickerPlantGroups.FirstOrDefault(_ => _.Equals(plantGroup)).isEdited = false;
            SetItem();
        }

        [RelayCommand]
        async Task DeletePlantGroupAsync(PlantGroup plantGroup)
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
        async Task ClearAllPlantGroupDataAsync()
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
        async Task ClearAllPlantDataAsync()
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
