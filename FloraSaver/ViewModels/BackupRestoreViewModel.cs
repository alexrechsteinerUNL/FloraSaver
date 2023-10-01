using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FloraSaver.Services;
using System.Linq;
namespace FloraSaver.ViewModels
{
    public partial class BackupRestoreViewModel : BaseViewModel
    {
        [ObservableProperty]
        public string defaultFileName = $"Plants_{DateTime.Now}.db3";
        [ObservableProperty]
        public string fileExtension = ".db3";
        [ObservableProperty]
        public string fileName = $"Plants_{DateTime.Now}.db3";

        public BackupRestoreViewModel(IDatabaseService databaseService) 
        {
            _databaseService = databaseService;
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
            await _databaseService.TestDbConnectionFromFileAsync(result.FullPath);
            return;
        }
        //End Testing
    }
}
