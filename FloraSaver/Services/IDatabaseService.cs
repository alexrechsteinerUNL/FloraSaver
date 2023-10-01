using FloraSaver.Models;

namespace FloraSaver.Services
{
    public interface IDatabaseService
    {
        string StatusMessage { get; set; }
        Task BackupDatabaseAsync(string databaseFileName);
        Task TestDbConnectionFromFileAsync(string filePath);
        Task AddUpdateNewPlantAsync(Plant plant);
        Task AddUpdateNewPlantGroupAsync(PlantGroup plantGroup);
        Task DeleteAllAsync();
        Task DeleteAllPlantGroupsAsync();
        Task DeletePlantAsync(Plant plant);
        Task DeletePlantGroupAsync(PlantGroup plantGroup);
        Task<List<Plant>> GetAllPlantAsync();
        Task<List<PlantGroup>> GetAllPlantGroupAsync();
    }
}