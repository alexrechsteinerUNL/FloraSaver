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
        public List<AutoFillPlant> PlantSuggestions { get; set; } = new();

        public ObservableCollection<SearchedPlants> TopTenAutoFillPlants { get; set; } = new();

        public List<Plant> BackendPlantList { get; set; } = new();
        bool IsInitialization { get; set; } = true;
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
            IsInitialization = true;
            PlantSuggestions = PlantSuggestions.Count > 0 ? PlantSuggestions : new(await _databaseService.GetAllAutofillPlantAsync());
            timer = new PeriodicTimer(TimeSpan.FromSeconds(10));
            PeriodicTimerUpdaterBackgroundAsync(() => CheatUpdateAllPlantProgress());
            if (ShouldUpdateCheckService.shouldGetNewGroupDataTable) { ShouldUpdateCheckService.ForceToGetNewGroupData(); await GetPlantGroupsAsync(); ShouldUpdateCheckService.shouldGetNewGroupDataTable = false; }
            if (ShouldUpdateCheckService.shouldGetNewPlantDataTable) { ShouldUpdateCheckService.ForceToGetNewPlantData(); await GetPlantsAsync(); ShouldUpdateCheckService.shouldGetNewPlantDataTable = false; }
            await StandardActionsAsync(SearchQuery);
            if (DataPlants.Count > 0)
            {
                AreNoPlants = false;
            } else
            {
                AreNoPlants = true;
            }
            IsInitialization = false;
        }

        [RelayCommand]
        public void Disappearing()
        {
            timer.Dispose();
        }

        protected PeriodicTimer timer = new PeriodicTimer(TimeSpan.FromSeconds(10));

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ArePlants))]
        public bool areNoPlants = true;
        public bool ArePlants => !AreNoPlants;
        [ObservableProperty]
        private bool showSearchSuggestionsBox = false;
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

        protected void RebuildPlantsSafely(IEnumerable<Plant> plantList)
        {
            Plants.Clear();
            foreach (var plant in plantList)
            {
                plant.PlantImageSource = plant.ImageLocation is not null ? Base64ImageConverterService.Base64ToImage(plant.ImageLocation) : null;
                Plants.Add(plant);
            }
            OnPropertyChanged("Plants");
        }


        protected void setPlantOrder(string order)
        {
            //this is gross. There has to be a better way.
            switch (order)
            {
                case "Next Action":
                    RebuildPlantsSafely(BackendPlantList.OrderByDescending(_ => new[] { _.WaterPercent, _.MistPercent, _.SunPercent }.Max()));
                    break;

                case "Next Watering":

                    RebuildPlantsSafely(BackendPlantList.OrderByDescending(_ => _.UseWatering).ThenByDescending(_ => _.WaterPercent));
                    break;

                case "Next Misting":
                    RebuildPlantsSafely(BackendPlantList.OrderByDescending(_ => _.UseMisting).ThenByDescending(_ => _.MistPercent));
                    break;

                case "Next Moving":
                    RebuildPlantsSafely(BackendPlantList.OrderByDescending(_ => _.UseMoving).ThenByDescending(_ => _.SunPercent));
                    break;

                case "Alphabetical":
                    RebuildPlantsSafely(BackendPlantList.OrderBy(_ => _.GivenName));
                    break;

                default:
                    RebuildPlantsSafely(BackendPlantList.OrderByDescending(_ => _.UseWatering).ThenByDescending(_ => _.WaterPercent));
                    break;
            }
            //OnPropertyChanged("Plants");
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
            await StandardActionsAsync(SearchQuery);
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
            await StandardActionsAsync(SearchQuery);
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
            await StandardActionsAsync(SearchQuery);
        }

        //[RelayCommand]
        //public async Task SearchPlantsAsync(string inputString)
        //{
        //    if (!IsInitialization)
        //    {
        //        try
        //        {
        //            List<Plant> plantDataList = DataPlants.ToList();
        //            SearchQuery = inputString;
        //            //Plants.Replace(DataPlants);
        //            foreach (var group in PlantGroups)
        //            {
        //                await ShowHidePlantGroupsAsync(group, false);
        //            }
        //            if (!string.IsNullOrWhiteSpace(SearchQuery))
        //            {
        //                for (int i = plantDataList.Count - 1; i >= 0; i--)
        //                {
        //                    if (plantDataList[i].GivenName.IndexOf(SearchQuery) < 0)
        //                    {
        //                        plantDataList.RemoveAt(i);
        //                    }
        //                }
        //            }
        //            RebuildPlantsSafely(plantDataList);
        //            setPlantOrder(CurrentOrderByValue);
        //        }
        //        catch (Exception ex)
        //        {
        //            Debug.WriteLine($"Unable to search plants: {ex.Message}");
        //            await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");
        //        }
        //    }

        //}

        [RelayCommand]
        public async Task GetPlantsAsync(bool shouldSort = false)
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

                RebuildPlantsSafely(plants);
                DataPlants = new ObservableCollection<Plant>(Plants);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unable to get plants: {ex.Message}");
                await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");
            }
            finally
            {
                if (shouldSort)
                {
                    await StandardActionsAsync(SearchQuery);
                }
                IsBusy = false;
                IsRefreshing = false;
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
        protected void QueryAutofillPlantAsyncFromSearch(string searchQuery)
        {
            var TopTenPlants = PlantSuggestions.Where(_ => _.PlantSpecies.Contains(searchQuery, StringComparison.CurrentCultureIgnoreCase)).Take(10);

            TopTenAutoFillPlants = new ObservableCollection<SearchedPlants>();
            foreach(var plant in TopTenPlants)
            {
                TopTenAutoFillPlants.Add(ConvertToSearchedPlant(plant));
            }
            OnPropertyChanged("TopTenAutoFillPlants");
            ShowSearchSuggestionBox();
        }

        protected SearchedPlants ConvertToSearchedPlant(IPlant plant)
        {
           return new SearchedPlants(plant);
        }


        [RelayCommand]
        protected async Task StandardActionsAsync(string searchText = "")
        {
            if (searchText != SearchQuery)
            {
                SearchQuery = searchText;

            }

            if (ShowSearchSuggestionsBox)
            {
                QueryAutofillPlantAsyncFromSearch(SearchQuery);
            }

            BackendPlantList = DataPlants.ToList();
            var temporaryBackendPlantList = new List<Plant>(BackendPlantList);
            foreach (var plant in temporaryBackendPlantList)
            {
                var isSavedFromGroups = await ShowHideSingualrPlantGroupsAsync(plant);
                var isSavedFromSearch = await SearchSingularPlantAsync(plant);
                if (!isSavedFromGroups || !isSavedFromSearch)
                {
                    BackendPlantList.Remove(plant);
                } else if (BackendPlantList.FirstOrDefault(_ => _.GivenName == plant.GivenName) is null)
                {
                    BackendPlantList.Add(plant);
                }

            }
            setPlantOrder(CurrentOrderByValue);
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
        protected void ShowSearchSuggestionBox()
        {
            ShowSearchSuggestionsBox = true;
        }

        [RelayCommand]
        protected void HideSearchSuggestionBox()
        {
            ShowSearchSuggestionsBox = false;
        }

        [RelayCommand]
        private async Task GroupSelectionAsync(PlantGroup plantGroup)
        {
            var specificGroup = PlantGroups.FirstOrDefault(_ => _.GroupId == plantGroup.GroupId);
            specificGroup.IsEnabled = plantGroup.IsEnabled ? false : true;
            OnPropertyChanged("PlantGroups");
            await StandardActionsAsync(SearchQuery);
        }

        public virtual async Task<bool> ShowHideSingualrPlantGroupsAsync(Plant plant)
        {
            var groupEnabled = PlantGroups.FirstOrDefault(_ => _.GroupName == plant.PlantGroupName)?.IsEnabled ?? false;
                if (!groupEnabled)
                {
                    return false;
                }
                return true;
        }

        public virtual async Task<bool> SearchSingularPlantAsync(Plant plant)
        {
            if (plant.GivenName.IndexOf(SearchQuery) < 0)
            {
                return false;
            }
            return true;
        }





        //public virtual async Task ShowHidePlantGroupsAsync(PlantGroup specificGroup, bool awaitSearch = true)
        //{
        //    List<Plant> plantList = Plants.ToList();
        //    foreach (var plant in Plants.Where(_ => _.PlantGroupName == specificGroup.GroupName))
        //    {
        //        if (!plantList.Contains(plant) && specificGroup.IsEnabled)
        //        {
        //            plantList.Add(plant);
        //        }
        //        else if (plantList.Contains(plant) && !specificGroup.IsEnabled)
        //        {
        //            plantList.Remove(plant);
        //        }
        //    }

        //    if (awaitSearch && !String.IsNullOrWhiteSpace(SearchQuery))
        //    {
        //        await SearchPlantsAsync(SearchQuery);
        //    } else
        //    {
        //        RebuildPlantsSafely(plantList);
        //    }
        //}

        [RelayCommand]
        private async Task GoToSetupDetailsGroupNameAsync(Plant plant)
        {
            await GoToSetupDetailsAsync(plant, "GroupName");
        }

        [RelayCommand]
        private async Task GoToSetupDetailsPlantNameAsync(Plant plant)
        {
            await GoToSetupDetailsAsync(plant, "PlantName");
        }

        [RelayCommand]
        private async Task GoToSetupDetailsGivenNameAsync(Plant plant)
        {
            await GoToSetupDetailsAsync(plant, "GivenName");
        }

        [RelayCommand]
        private async Task GoToSetupDetailsWaterAsync(Plant plant)
        {
            await GoToSetupDetailsAsync(plant, "Water");
        }

        [RelayCommand]
        private async Task GoToSetupDetailsRefreshAsync(Plant plant)
        {
            await GoToSetupDetailsAsync(plant, "Refresh");
        }

        [RelayCommand]
        private async Task GoToSetupDetailsSunAsync(Plant plant)
        {
            await GoToSetupDetailsAsync(plant, "Sun");
        }

        [RelayCommand]
        private async Task GoToDetailsSuggestionPlantAsync(SearchedPlants searchedPlant)
        {
            await GoToDetailsAsync(new Plant(searchedPlant) { Id = DataPlants.Any() ? DataPlants.Max(x => x.Id) + 1 : 0});
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

        [RelayCommand]
        private async Task GoToAddGroupAsync()
        {
            await Shell.Current.GoToAsync(nameof(AddGroupPage), true, new Dictionary<string, object>
                {
                    {"Plant", new Plant() },
                    {"PlantGroup", await _databaseService.GetAllPlantGroupAsync() }
                });
            return;
        }

        private async Task GoToSetupDetailsAsync(Plant plant, string plantDetailsSetupPageOpenTab = null)
        {
                await Shell.Current.GoToAsync(nameof(PlantDetailsSetupPage), true, new Dictionary<string, object>
                {
                    {"Plant", plant },
                    {"PlantGroup", await _databaseService.GetAllPlantGroupAsync() },
                    { "SelectedTab", plantDetailsSetupPageOpenTab}
                });
            return;
        }
    }
}