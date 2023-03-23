using FloraSaver.Models;

namespace FloraSaver.Services
{
    public interface IPlantService
    {
        Task AddUpdateNewPlantAsync(Plant plant);
        Task DeleteAllAsync();
        Task DeletePlantAsync(Plant plant);
        Task<List<Plant>> GetAllPlantAsync();
        Task SetAllNotificationsAsync(List<Plant> plants);
    }
}