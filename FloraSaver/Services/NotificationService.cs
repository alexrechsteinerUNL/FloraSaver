using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FloraSaver.Models;
#if (ANDROID || IOS)
using Plugin.LocalNotification;
#endif


namespace FloraSaver.Services
{
    public class NotificationService
    {
        public async Task SetAllNotificationsAsync(List<Plant> plants)
        {
            foreach (var plant in plants)
            {
                // could maybe set a flag instead of 2 methods that both run a notification.
                if (plant.DateOfNextWatering.Date + plant.TimeOfNextWatering > DateTime.Now)
                {
                    await FutureNotificationAsync(plant);
                }
                else
                {
                    await WarnOverdueAsync(plant);
                }
            }
        }

        private async Task WarnOverdueAsync(Plant plant)
        {
            var notification = new NotificationRequest
            {
                NotificationId = plant.Id,
                Title = $"Overdue watering on your '{plant.PlantSpecies}', {plant.GivenName}",
                Description = "You really should water this guy",
                ReturningData = "Dummy data", // Returning data when tapped on notification.
                Schedule =
                {
                    NotifyTime = DateTime.Now // Used for Scheduling local notification, if not specified notification will show immediately.
                }
            };
            await LocalNotificationCenter.Current.Show(notification);
        }

        private async Task FutureNotificationAsync(Plant plant)
        {
            var notification = new NotificationRequest
            {
                NotificationId = plant.Id,
                Title = $"It's time to water your '{plant.PlantSpecies}', {plant.GivenName}",
                Description = "You really should water this guy",
                ReturningData = "Dummy data", // Returning data when tapped on notification.
                Schedule =
                {
                    NotifyTime = plant.DateOfNextWatering.Date + plant.TimeOfNextWatering  // Used for Scheduling local notification, if not specified notification will show immediately.
                }
            };
            await LocalNotificationCenter.Current.Show(notification);
        }
    }
}
