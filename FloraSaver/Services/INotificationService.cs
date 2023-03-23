using FloraSaver.Models;

namespace FloraSaver.Services
{
    public interface INotificationService
    {
        Task SetAllNotificationsAsync(List<Plant> plants);
    }
}