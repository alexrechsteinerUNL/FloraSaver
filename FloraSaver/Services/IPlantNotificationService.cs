using FloraSaver.Models;

namespace FloraSaver.Services
{
    public interface IPlantNotificationService
    {
        Task SetAllNotificationsAsync(List<Plant> plants);
        public void PlantNotificationEnder(Plant plant, string plantAction);
    }
}