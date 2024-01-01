using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FloraSaver.Models;
using FloraSaver.Services;
using FloraSaver.Utilities;

namespace FloraSaver.ViewModels
{
    [QueryProperty(nameof(shouldGetNewData), "ShouldGetNewData")]
    [QueryProperty(nameof(shouldGetNewGroupData), "ShouldGetNewGroupData")]
    public partial class TableViewModel : BaseViewModel, INotifyPropertyChanged, IQueryAttributable
    {
        private readonly int percentageButtonSize = 105;
        protected readonly IPlantNotificationService _plantNotificationService;
        public ObservableCollection<Plant> DataPlants { get; set; } = new();
        public ObservableCollection<Plant> Plants { get; set; } = new();
        public ObservableCollection<PlantGroup> PlantGroups { get; set; } = new();
        protected bool shouldGetNewData { get; set; } = true;
        protected bool shouldGetNewGroupData { get; set; } = true;

        // I moved the _databaseService to the base viewmodel because just about every page was going to use it.
        public TableViewModel(IDatabaseService databaseService, IPlantNotificationService plantNotificationService)
        {
            Title = "Plant Saver";
            _plantNotificationService = plantNotificationService;
            _databaseService = databaseService;
            IsPlantTypeIncluded = true;
            WaterRectangle = new Rect(0, 20, 105, 105);
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.Count < 1)
            {
                return;
            }
            ShouldUpdateCheckService.shouldGetNewPlantDataTable = (bool)(query["ShouldGetNewData"] ?? false);
            ShouldUpdateCheckService.shouldGetNewGroupDataTable = (bool)(query["ShouldGetNewGroupData"] ?? false);
        }

        [RelayCommand]
        private async Task AppearingAsync()
        {
            timer = new PeriodicTimer(TimeSpan.FromSeconds(10));

            if (ShouldUpdateCheckService.shouldGetNewGroupDataTable) { ShouldUpdateCheckService.ForceToGetNewGroupData(); await GetPlantGroupsAsync(); ShouldUpdateCheckService.shouldGetNewGroupDataTable = false; }
            if (ShouldUpdateCheckService.shouldGetNewPlantDataTable) { ShouldUpdateCheckService.ForceToGetNewPlantData(); await GetPlantsAsync(); ShouldUpdateCheckService.shouldGetNewPlantDataTable = false; }
        }

        [RelayCommand]
        public void Disappearing()
        {
            timer.Dispose();
        }

        protected PeriodicTimer timer = new PeriodicTimer(TimeSpan.FromSeconds(10));

        [ObservableProperty]
        private List<string> orderByValues = PickerService.GetOrderByValues();

        [ObservableProperty]
        private string currentOrderByValue = "Next Action";

        [ObservableProperty]
        private bool isRefreshing;

        [ObservableProperty]
        private string searchQuery = string.Empty;

        [ObservableProperty]
        private Rect rectNeedsWatering;

        [ObservableProperty]
        private bool isPlantTypeIncluded;

        [ObservableProperty]
        private string toggleButtonTextColor;

        [ObservableProperty]
        private bool toggleButtonBackgroundColor;

        [ObservableProperty]
        private Rect waterRectangle;

        [ObservableProperty]
        private bool waterEnabled;

        [ObservableProperty]
        private bool mistEnabled;

        [ObservableProperty]
        private bool moveEnabled;

        [ObservableProperty]
        private int waterOpacity;

        [ObservableProperty]
        private int mistOpacity;

        [ObservableProperty]
        private int moveOpacity;

        partial void OnCurrentOrderByValueChanged(string value)
        {
            setPlantOrder(value);
        }

        protected void setPlantOrder(string order)
        {
            //this is gross. There has to be a better way.
            switch (order)
            {
                case "Next Action":
                    Plants = new ObservableCollection<Plant>(Plants.OrderByDescending(_ => new[] { _.WaterPercent, _.MistPercent, _.SunPercent }.Max()));
                    break;

                case "Next Watering":
                    Plants = new ObservableCollection<Plant>(Plants.OrderByDescending(_ => _.UseWatering).ThenByDescending(_ => _.WaterPercent));
                    break;

                case "Next Misting":
                    Plants = new ObservableCollection<Plant>(Plants.OrderByDescending(_ => _.UseMisting).ThenByDescending(_ => _.MistPercent));
                    break;

                case "Next Moving":
                    Plants = new ObservableCollection<Plant>(Plants.OrderByDescending(_ => _.UseMoving).ThenByDescending(_ => _.SunPercent));
                    break;

                case "Alphabetical":
                    Plants = new ObservableCollection<Plant>(Plants.OrderBy(_ => _.GivenName));
                    break;

                default:
                    Plants = new ObservableCollection<Plant>(Plants.OrderByDescending(_ => _.UseWatering).ThenByDescending(_ => _.WaterPercent));
                    break;
            }
            OnPropertyChanged("Plants");
        }

        //please break these bad boys out into a class of their own so you don't have to rewirte this fun logic ever.
        [RelayCommand]
        protected virtual async Task ResetWateringAsync(Plant plant)
        {
            if (plant.UseWatering)
            {
                if (plant.WaterInterval != 0)
                {
                    do
                    {
                        plant.DateOfNextWatering = plant.DateOfNextWatering.AddDays(plant.WaterInterval != null ? (int)plant.WaterInterval :
(plant.DateOfNextWatering.Date - plant.DateOfLastWatering.Date).Days);
                    } while (plant.DateOfNextWatering < DateTime.Now);
                }
                else
                {
                    plant.UseWatering = false;
                }

                plant.DateOfLastWatering = DateTime.Now;
                plant.TimeOfLastWatering = DateTime.Now.TimeOfDay;
                _plantNotificationService.PlantNotificationEnder(plant, "water");
                await _databaseService.AddUpdateNewPlantAsync(plant);
                OnPropertyChanged("Plant");
                await GetPlantsAsync();
            }
            ShouldUpdateCheckService.ForceToGetNewPlantData();
        }

        [RelayCommand]
        protected virtual async Task ResetMistingAsync(Plant plant)
        {
            if (plant.UseMisting)
            {
                if (plant.MistInterval != 0)
                {
                    do
                    {
                        plant.DateOfNextMisting = plant.DateOfNextMisting.AddDays(plant.MistInterval != null ? (int)plant.MistInterval :
    (plant.DateOfNextMisting.Date - plant.DateOfLastMisting.Date).Days);
                    } while (plant.DateOfNextMisting < DateTime.Now);
                }
                else
                {
                    plant.UseMisting = false;
                }

                plant.DateOfLastMisting = DateTime.Now;
                plant.TimeOfLastMisting = DateTime.Now.TimeOfDay;

                _plantNotificationService.PlantNotificationEnder(plant, "mist");
                await _databaseService.AddUpdateNewPlantAsync(plant);
                OnPropertyChanged("Plant");
                await GetPlantsAsync();
            }
            ShouldUpdateCheckService.ForceToGetNewPlantData();
        }

        [RelayCommand]
        protected virtual async Task ResetMovingAsync(Plant plant)
        {
            //This method is probably causing a glitch
            if (plant.UseMoving)
            {
                if (plant.SunInterval != 0)
                {
                    do
                    {
                        plant.DateOfNextMove = plant.DateOfNextMove.AddDays(plant.SunInterval != null ? (int)plant.SunInterval :
                    (plant.DateOfNextMove.Date - plant.DateOfLastMove.Date).Days);
                    }
                    while (plant.DateOfNextMove < DateTime.Now);
                }
                else
                {
                    plant.UseMoving = false;
                }

                plant.DateOfLastMove = DateTime.Now;
                plant.TimeOfLastMove = DateTime.Now.TimeOfDay;
                _plantNotificationService.PlantNotificationEnder(plant, "move");
                await _databaseService.AddUpdateNewPlantAsync(plant);
                OnPropertyChanged("Plant");
                await GetPlantsAsync();
            }
            ShouldUpdateCheckService.ForceToGetNewPlantData();
        }

        [RelayCommand]
        public async Task SearchPlantsAsync(string inputString)
        {
            try
            {
                SearchQuery = inputString;
                Plants.Replace(DataPlants);
                foreach (var group in PlantGroups)
                {
                    await ShowHidePlantGroupsAsync(group, false);
                }
                if (!string.IsNullOrWhiteSpace(SearchQuery))
                {
                    for (int i = Plants.Count - 1; i >= 0; i--)
                    {
                        if (Plants[i].GivenName.IndexOf(SearchQuery) < 0)
                        {
                            Plants.RemoveAt(i);
                        }
                    }
                }
                setPlantOrder(CurrentOrderByValue);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unable to search plants: {ex.Message}");
                await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");
            }
        }

        [RelayCommand]
        protected async Task GetPlantsAsync()
        {
            //This is a bad place for it, but this is testing for the Utility that separates elements into textboxes. This would be covered by something real software engineers call a UNIT TEST!
            //this is testing data for loading from a file.
            //await LoadClipetTextFileAsync("WelcomeMessage.txt");
            PeriodicTimerUpdaterBackgroundAsync(() => CheatUpdateAllPlantProgress());
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;
                var plants = await _databaseService.GetAllPlantAsync();

                // checking if there are really 0 plants or if an issue occured.
                if (plants.Count == 0)
                {
                    plants = await _databaseService.GetAllPlantAsync();
                    // small buffer for plant count
                    for (var i = 0; i < 2; i++)
                    {
                        if (plants.Count != 0)
                        {
                            break;
                        }
                    }
                }

                if (Plants.Count != 0)
                {
                    Plants.Clear();
                }
                foreach (var plant in plants)
                {
                    plant.PlantImageSource = plant.ImageLocation is not null ? Base64ImageConverterService.Base64ToImage(plant.ImageLocation) : null;
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
                foreach (var group in PlantGroups)
                {
                    await ShowHidePlantGroupsAsync(group);
                }
                setPlantOrder(CurrentOrderByValue);
            }
            return;
        }

        protected void CheatUpdateAllPlantProgress()
        {
            for (var i = 0; i < Plants.Count; i++)
            {
                Plants[i].WaterPercent = Plants[i].WaterPercent;
                Plants[i].MistPercent = Plants[i].MistPercent;
                Plants[i].SunPercent = Plants[i].SunPercent;
                OnPropertyChanged("Plant");
            }
        }

        protected async Task PeriodicTimerUpdaterBackgroundAsync(Action action)
        {
            while (await timer.WaitForNextTickAsync())
            {
                action();
            }
        }

        [RelayCommand]
        protected async Task GetPlantGroupsAsync()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;
                var plantGroups = await _databaseService.GetAllPlantGroupAsync();

                // checking if there are really 0 plantGroups or if an issue occurred.
                if (plantGroups.Count == 0)
                {
                    plantGroups = await _databaseService.GetAllPlantGroupAsync();
                    for (var i = 0; i < 2; i++)
                    {
                        if (plantGroups.Count != 0)
                        {
                            break;
                        }
                    }
                }

                if (PlantGroups.Count != 0)
                {
                    PlantGroups.Clear();
                }
                foreach (var plantGroup in plantGroups)
                {
                    PlantGroups.Add(plantGroup);
                    OnPropertyChanged(nameof(PlantGroups));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unable to get plant groups: {ex.Message}");
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
        private async Task GroupSelectionAsync(PlantGroup plantGroup)
        {
            var specificGroup = PlantGroups.FirstOrDefault(_ => _.GroupId == plantGroup.GroupId);
            specificGroup.IsEnabled = plantGroup.IsEnabled ? false : true;
            OnPropertyChanged("PlantGroups");
            await ShowHidePlantGroupsAsync(specificGroup);
            setPlantOrder(CurrentOrderByValue);
        }

        public virtual async Task ShowHidePlantGroupsAsync(PlantGroup specificGroup, bool awaitSearch = true)
        {
            foreach (var plant in DataPlants.Where(_ => _.PlantGroupName == specificGroup.GroupName))
            {
                if (!Plants.Contains(plant) && specificGroup.IsEnabled)
                {
                    Plants.Add(plant);
                }
                else if (Plants.Contains(plant) && !specificGroup.IsEnabled)
                {
                    Plants.Remove(plant);
                }
            }
            if (awaitSearch)
            {
                await SearchPlantsAsync(SearchQuery);
            }
        }

        [RelayCommand]
        private async Task GoToDetailsAsync(Plant plant)
        {
            if (plant == null)
            {
                await Shell.Current.GoToAsync(nameof(PlantDetailsSetupPage), true, new Dictionary<string, object>
                {
                    {"Plant", new Plant {
                        Id = DataPlants.Any() ? DataPlants.Max(x => x.Id) + 1 : 0,
                        DateOfBirth = DateTime.Now,
                        DateOfLastWatering = DateTime.Now,
                        TimeOfLastWatering = DateTime.Now.TimeOfDay,
                        DateOfNextWatering = DateTime.Now,
                        TimeOfNextWatering = DateTime.Now.TimeOfDay,
                        DateOfLastMisting = DateTime.Now,
                        TimeOfLastMisting = DateTime.Now.TimeOfDay,
                        DateOfNextMisting = DateTime.Now,
                        TimeOfNextMisting = DateTime.Now.TimeOfDay,
                        DateOfLastMove = DateTime.Now,
                        TimeOfLastMove = DateTime.Now.TimeOfDay,
                        DateOfNextMove = DateTime.Now,
                        TimeOfNextMove = DateTime.Now.TimeOfDay
                        }
                    },
                    {"PlantGroup", await _databaseService.GetAllPlantGroupAsync() }
                });
            }
            else
            {
                await Shell.Current.GoToAsync(nameof(PlantDetailsPage), true, new Dictionary<string, object>
                {
                    {"Plant", plant },
                    {"PlantGroup", await _databaseService.GetAllPlantGroupAsync() }
                });
            }
            return;
        }
    }
}