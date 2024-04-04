using CommunityToolkit.Maui.Storage;
using FloraSaver.Models;

namespace FloraSaver.Services
{
    public interface IDatabaseService
    {
        string StatusMessage { get; set; }

        Task<FileSaverResult> BackupDatabaseAsync(string databaseFileName);

        Task<List<Plant>> TestDbConnectionFromFileAsync(string filePath);

        Task AddUpdateNewPlantAsync(Plant plant);

        Task AddUpdateNewPlantGroupAsync(PlantGroup plantGroup, bool setPlants = true, string oldPlantNameIfApplicable = "");

        Task DeleteAllAsync();

        Task DeleteAllPlantsAsync();

        Task DeleteAllPlantGroupsAsync();

        Task DeletePlantAsync(Plant plant);

        Task DeletePlantGroupAsync(PlantGroup plantGroup);

        Task<List<Plant>> GetAllPlantAsync();

        Task<List<PlantGroup>> GetAllPlantGroupAsync();

        public Task<List<AutoFillPlant>> GetAllAutofillPlantAsync();

        public Task<List<ClipetDialog>> GetAllClipetDialogsAsync();

        public Task PopulateClipetDialogTableAsync();
        public Task PopulateAutoFillPlantTableAsync();
        public Task UpdateClipetDialogTableAsync(List<ClipetDialog> Dialogs);
    }
}