using FloraSaver.Models;

namespace FloraSaver.Services
{
    public interface IPlantNotificationService
    {
        public Task<List<Plant>> SetAllNotificationsAsync(List<Plant> plants);
        public void PlantNotificationEnder(Plant plant, string plantAction);
    }
}