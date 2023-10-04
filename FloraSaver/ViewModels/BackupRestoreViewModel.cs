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

        public ObservableCollection<Plant> NewPlantsFromFile { get; set; } = new();

        public ObservableCollection<Plant> OldPlants { get; set; } = new();

        public BackupRestoreViewModel(IDatabaseService databaseService, IPlantNotificationService plantNotificationService) : base(databaseService, plantNotificationService)
        {
            databaseService = _databaseService;
            plantNotificationService = _plantNotificationService;
            OldPlants = Plants;
        }

        //Testing
        [RelayCommand]
        async Task BackupDatabaseAsync(string databaseFileName)
        {
            if (string.IsNullOrWhiteSpace(databaseFileName))
            {
                databaseFileName = DefaultFileName;
            }

            if (databaseFileName.Length < 4 || databaseFileName.Substring(databaseFileName.Length - 4) != FileExtension)
            {
                databaseFileName += FileExtension;
            }
            await _databaseService.BackupDatabaseAsync(databaseFileName);
        }

        [RelayCommand]
        async Task ImportDatabaseAsync()
        {
            var result = await FilePicker.PickAsync(new PickOptions { });

            if (result is null)
            {
                return;
            }

            NewPlantsFromFile = new(await _databaseService.TestDbConnectionFromFileAsync(result.FullPath));
            OnPropertyChanged(nameof(NewPlantsFromFile));
            return;
        }
        [RelayCommand]
        void PlantSelectionOld(Plant plant)
        {
            var specificPlant = OldPlants.FirstOrDefault(_ => _.Id == plant.Id);
            specificPlant.IsEnabled = plant.IsEnabled ? false : true;
            OnPropertyChanged("OldPlants");
        }
        [RelayCommand]
        void PlantSelectionNew(Plant plant)
        {
            var specificPlant = NewPlantsFromFile.FirstOrDefault(_ => _.Id == plant.Id);
            specificPlant.IsEnabled = plant.IsEnabled ? false : true;
            OnPropertyChanged("NewPlantsFromFile");
        }
        //End Testing
    }
}
