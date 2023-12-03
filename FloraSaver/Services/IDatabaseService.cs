using FloraSaver.Models;

namespace FloraSaver.Services
{
    public interface IDatabaseService
    {
        string StatusMessage { get; set; }

        Task BackupDatabaseAsync(string databaseFileName);

        Task<List<Plant>> TestDbConnectionFromFileAsync(string filePath);

        Task AddUpdateNewPlantAsync(Plant plant);

        Task AddUpdateNewPlantGroupAsync(PlantGroup plantGroup, bool setPlants = true);

        Task DeleteAllAsync();

        Task DeleteAllPlantsAsync();

        Task DeleteAllPlantGroupsAsync();

        Task DeletePlantAsync(Plant plant);

        Task DeletePlantGroupAsync(PlantGroup plantGroup);

        Task<List<Plant>> GetAllPlantAsync();

        Task<List<PlantGroup>> GetAllPlantGroupAsync();
    }
}