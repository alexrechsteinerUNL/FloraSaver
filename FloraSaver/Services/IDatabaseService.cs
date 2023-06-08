using FloraSaver.Models;

namespace FloraSaver.Services
{
    public interface IDatabaseService
    {
        string StatusMessage { get; set; }

        Task AddUpdateNewPlantAsync(Plant plant);
        Task AddUpdateNewPlantGroupAsync(PlantGroup plantGroup);
        Task DeleteAllAsync();
        Task DeleteAllPlantGroupsAsync();
        Task DeletePlantAsync(Plant plant);
        Task DeletePlantGroupAsync(PlantGroup plantGroup);
        Task<List<Plant>> GetAllPlantAsync();
        Task<List<PlantGroup>> GetAllPlantGroupAsync();
        void PlantNotificationEnder(Plant plant, string plantAction);
        Task SetAllNotificationsAsync(List<Plant> plants);
    }
}