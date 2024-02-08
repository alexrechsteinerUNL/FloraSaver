using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FloraSaver.Models;
using FloraSaver.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace FloraSaver.ViewModels
{
    public partial class BackupRestoreViewModel : HandlingViewModel, INotifyPropertyChanged
    {
        [ObservableProperty]
        public string defaultFileName = $"Plants_{DateTime.Now}.db3";

        [ObservableProperty]
        public string fileExtension = ".db3";

        [ObservableProperty]
        public string fileName = $"Plants_{DateTime.Now}.db3";

        [ObservableProperty]
        public string clipetBackupText = "Backup Your Plants!";

        [ObservableProperty]
        public bool showCurrentPlants = false;

        public ObservableCollection<Plant> NewPlantsFromFile { get; set; } = new();

        public ObservableCollection<Plant> OldPlants { get; set; } = new();

        public BackupRestoreViewModel(IDatabaseService databaseService, IPlantNotificationService plantNotificationService) : base(databaseService, plantNotificationService)
        {
            databaseService = _databaseService;
            plantNotificationService = _plantNotificationService;
            OldPlants = Plants;
        }

        [RelayCommand]
        private async Task BackupDatabaseAsync(string databaseFileName)
        {
            if (string.IsNullOrWhiteSpace(databaseFileName))
            {
                databaseFileName = DefaultFileName;
            }

            if (databaseFileName.Length < 4 || databaseFileName.Substring(databaseFileName.Length - 4) != FileExtension)
            {
                databaseFileName.Replace(".", "");
                databaseFileName += FileExtension;
            }
            try
            {
                var result = await _databaseService.BackupDatabaseAsync(databaseFileName);
                if (!result.IsSuccessful)
                {
                    ClipetBackupText = "Oh no! That didn't work! Please try a different name or location, and make sure you hit that save button!";
                } else
                {
                    ClipetBackupText = $"You have backed up your plants with the filename '{databaseFileName}'";
                }
            } catch (Exception ex)
            {
                ClipetBackupText = "Oh no! That didn't work! Please try a different name or location, and make sure you hit that save button!";
            }
            
        }

        [RelayCommand]
        private async Task ImportDatabaseAsync()
        {
            var result = await FilePicker.PickAsync(new PickOptions { });

            if (result is null)
            {
                return;
            }

            NewPlantsFromFile = new(await _databaseService.TestDbConnectionFromFileAsync(result.FullPath));
            OnPropertyChanged(nameof(NewPlantsFromFile));
            if (NewPlantsFromFile.Count > 0 ) { ShowCurrentPlants = true; }
            return;
        }

        [RelayCommand]
        private void PlantSelectionOld(Plant plant)
        {
            var specificPlant = OldPlants.FirstOrDefault(_ => _.Id == plant.Id);
            specificPlant.IsEnabled = plant.IsEnabled ? false : true;
            var samePlant = NewPlantsFromFile.FirstOrDefault(_ => _.GivenName == plant.GivenName);
            if (samePlant != null)
            {
                samePlant.IsEnabled = false;
            }
            OnPropertyChanged("NewPlantsFromFile");
            OnPropertyChanged("OldPlants");
        }

        [RelayCommand]
        private void PlantSelectionNew(Plant plant)
        {
            var specificPlant = NewPlantsFromFile.FirstOrDefault(_ => _.Id == plant.Id);
            specificPlant.IsEnabled = plant.IsEnabled ? false : true;
            var samePlant = OldPlants.FirstOrDefault(_ => _.GivenName == plant.GivenName);
            if (samePlant != null)
            {
                samePlant.IsEnabled = false;
            }
            OnPropertyChanged("NewPlantsFromFile");
            OnPropertyChanged("OldPlants");
        }

        [RelayCommand]
        private async Task AcceptMergeAsync()
        {
            if (NewPlantsFromFile.Any(_ => _.IsEnabled == true))
            {
                var alertMessage = "Are you sure? ";
                var allPlants = NewPlantsFromFile.Where(_ => _.IsEnabled == true).ToList();
                var oldPlantsReplaced = OldPlants.Where(_ => allPlants.Select(_ => _.GivenName).Contains(_.GivenName)).Select(_ => _.GivenName).ToList();
                var oldPlantsRemoved = OldPlants.Where(_ => _.IsEnabled == false && !oldPlantsReplaced.Contains(_.GivenName)).Select(_ => _.GivenName).ToList();
                if (oldPlantsRemoved.Count > 0)
                {
                    alertMessage += $"Current Plants: '{string.Join(", ", oldPlantsRemoved)}' will be lost forever! ";
                }
                if (oldPlantsReplaced.Count > 0)
                {
                    alertMessage += (oldPlantsRemoved.Count) > 0 ? $"And '{string.Join(", ", oldPlantsReplaced)}' will be replaced by the merge! " : $"Current Plants: '{string.Join(", ", oldPlantsReplaced)}' will be replaced by the merge! ";
                }
                allPlants.AddRange(OldPlants.Where(_ => _.IsEnabled == true).ToList());
                Plants = new ObservableCollection<Plant>(allPlants);
                bool reallyMerge = await Application.Current.MainPage.DisplayAlert("OH HOLD ON!", $"{alertMessage}. Some plant groups might be corrupted.", "Merge Em!", "Please Don't");
                if (reallyMerge)
                {
                    await _databaseService.DeleteAllPlantsAsync();
                    foreach (var plant in Plants)
                    {
                        await _databaseService.AddUpdateNewPlantAsync(plant);
                    }
                }
                ShouldUpdateCheckService.ForceToGetNewGroupData();
                ShouldUpdateCheckService.ForceToGetNewPlantData();
            }
        }

        //End Testing
    }
}