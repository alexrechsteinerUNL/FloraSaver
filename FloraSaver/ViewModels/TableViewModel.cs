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
    public partial class TableViewModel : BaseViewModel, INotifyPropertyChanged
    {
        private readonly int percentageButtonSize = 105;
        public ObservableCollection<Plant> DataPlants { get; set; } = new();
        public ObservableCollection<Plant> Plants { get; set; } = new();
        public ObservableCollection<PlantGroup> PlantGroups { get; set; } = new();

        // I moved the _databaseService to the base viewmodel because just about every page was going to use it.
        public TableViewModel(IDatabaseService databaseService)
        {
            Title = "Plant Saver";
            _databaseService = databaseService;
            IsPlantTypeIncluded = true;
            WaterRectangle = new Rect(0,20,105,105);
        }

        [RelayCommand]
        async Task AppearingAsync()
        {
            await GetPlantGroupsAsync();
            await GetPlantsAsync();
        }


        [ObservableProperty]
        List<string> orderByValues = PickerService.GetOrderByValues();
        [ObservableProperty]
        string currentOrderByValue = "Next Action";
        [ObservableProperty]
        bool isRefreshing;
        [ObservableProperty]
        string searchQuery = string.Empty;
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

        partial void OnCurrentOrderByValueChanged(string value)
        {
            setPlantOrder(value);
        }

        private void setPlantOrder(string order)
        {
            //this is gross. There has to be a better way.
            switch (order)
            {
                case "Next Action":
                    Plants = new ObservableCollection<Plant>(Plants.OrderByDescending(_ => new[] { _.UseWatering, _.UseMisting, _.UseMoving }.Max()).ThenByDescending(_ => new[] { _.WaterPercent, _.MistPercent, _.SunPercent }.Max()));
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


        [RelayCommand]
        protected async Task ResetWateringAsync(Plant plant)
        {
            if (plant.UseWatering)
            {
                if (plant.WaterInterval != 0)
                {
                    plant.DateOfNextWatering = plant.DateOfNextWatering.AddDays(plant.WaterInterval != null ? (int)plant.WaterInterval :
(plant.DateOfNextWatering.Date - plant.DateOfLastWatering.Date).Days);
                } else
                {
                    plant.UseWatering = false;
                }

                plant.DateOfLastWatering = DateTime.Now;
                plant.TimeOfLastWatering = DateTime.Now.TimeOfDay;
                _databaseService.PlantNotificationEnder(plant, "water");
                await _databaseService.AddUpdateNewPlantAsync(plant);
                OnPropertyChanged("Plant");
                await GetPlantsAsync();
            }
        }

        [RelayCommand]
        protected async Task ResetMistingAsync(Plant plant)
        {
            if (plant.UseMisting)
            {
                if (plant.MistInterval != 0)
                {
                    plant.DateOfNextMisting = plant.DateOfNextMisting.AddDays(plant.MistInterval != null ? (int)plant.MistInterval :
    (plant.DateOfNextMisting.Date - plant.DateOfLastMisting.Date).Days);
                }
                else
                {
                    plant.UseMisting = false;
                }

                plant.DateOfLastMisting = DateTime.Now;
                plant.TimeOfLastMisting = DateTime.Now.TimeOfDay;

                _databaseService.PlantNotificationEnder(plant, "mist");
                await _databaseService.AddUpdateNewPlantAsync(plant);
                OnPropertyChanged("Plant");
                await GetPlantsAsync();
            }
        }

        [RelayCommand]
        protected async Task ResetMovingAsync(Plant plant)
        {
            if (plant.UseMoving)
            {
                if (plant.SunInterval != 0)
                {
                    plant.DateOfNextMove = plant.DateOfNextMove.AddDays(plant.SunInterval != null ? (int)plant.SunInterval :
                    (plant.DateOfNextMove.Date - plant.DateOfLastMove.Date).Days);
                }
                else
                {
                    plant.UseMoving = false;
                }

                plant.DateOfLastMove = DateTime.Now;
                plant.TimeOfLastMove = DateTime.Now.TimeOfDay;
                _databaseService.PlantNotificationEnder(plant, "move");
                await _databaseService.AddUpdateNewPlantAsync(plant);
                OnPropertyChanged("Plant");
                await GetPlantsAsync();
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
        async Task GetPlantsAsync()
        {
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

        [RelayCommand]
        async Task GetPlantGroupsAsync()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;
                var plantGroups = await _databaseService.GetAllPlantGroupAsync();

                // checking if there are really 0 plantGroups or if an issue occured.
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
        async Task GroupSelectionAsync(PlantGroup plantGroup)
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
                    },
                    {"PlantGroup", await _databaseService.GetAllPlantGroupAsync() }
                });
            }
            else
            {
                CurrentPlantNeeds(plant);
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
