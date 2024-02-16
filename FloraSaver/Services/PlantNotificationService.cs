using FloraSaver.Models;
using Microsoft.VisualStudio.Utilities;
using Plugin.LocalNotification;
using System.IO;
using System.Reflection;

namespace FloraSaver.Services
{
    public class PlantNotificationService : IPlantNotificationService
    {
        private string commonPlantOverdueNotificationDescription = "";

        public void PlantNotificationEnder(Plant plant, string plantAction)
        {
            var notificationId = GenerateNotificationId(plant, plantAction);
            LocalNotificationCenter.Current.Cancel(notificationId);
        }

        public static double COOLDOWN_HOURS { get; set; } = 1;
        public static double COOLDOWN_MULTI_HOURS { get; set; } = 24;

        public async Task<List<Plant>> SetAllNotificationsAsync(List<Plant> plants)
        {
            COOLDOWN_HOURS = int.Parse(Preferences.Default.Get("overdue_plants_time_to", "1"));
            COOLDOWN_MULTI_HOURS = int.Parse(Preferences.Default.Get("overdue_plants_multi_time_to", "24"));
            commonPlantOverdueNotificationDescription = string.Empty;
            if (await LocalNotificationCenter.Current.AreNotificationsEnabled() == false)
            {
                await LocalNotificationCenter.Current.RequestNotificationPermission();
            }
            // first make these parallel, but it might be better to do them plant by plant instead of 3 separate loops
            foreach (var plant in plants.Where(_ => _.DateOfNextWatering != _.DateOfLastWatering && _.UseWatering))
            {
                var plantDateWithExtraTime = plant.DateOfNextWatering.AddDays(plant.ExtraWaterTime);
                // could maybe set a flag instead of 2 methods that both run a notification.
                if (plantDateWithExtraTime >= DateTime.Now)
                {

                    plant.IsOverdueWater = false;
                    await FutureNotificationAsync(plant, "water", plantDateWithExtraTime, "future_notify_water");
                }
                else
                {
                    plant.IsOverdueWater = true;
                    //this is gross. Fix it!
                    if (COOLDOWN_HOURS != -1 && DateTime.Now.AddHours(COOLDOWN_HOURS) > plant.PlantWaterOverdueCooldownLastWarned)
                    {
                        await WarnOverdueAsync(plant, "water", plants
                        .FirstOrDefault(plant => plantDateWithExtraTime > DateTime.Now)?
                        .DateOfNextWatering.AddDays(plant.ExtraWaterTime) ?? DateTime.Now,
                        "overdue_notify_water"
                        );

                        plant.PlantWaterOverdueCooldownLastWarned = DateTime.Now;
                    }
                }
            }
            foreach (var plant in plants.Where(_ => _.DateOfNextMisting != _.DateOfLastMisting && _.UseMisting))
            {
                var plantDateWithExtraTime = plant.DateOfNextMisting.AddDays(plant.ExtraMistTime);
                // could maybe set a flag instead of 2 methods that both run a notification.
                if (plantDateWithExtraTime >= DateTime.Now)
                {
                    plant.IsOverdueMist = false;
                    await FutureNotificationAsync(plant, "mist", plantDateWithExtraTime, "future_notify_mist");
                }
                else
                {
                    plant.IsOverdueMist = true;
                    //this is gross. Fix it!
                    if (COOLDOWN_HOURS != -1 && DateTime.Now.AddHours(COOLDOWN_HOURS) > plant.PlantMistOverdueCooldownLastWarned)
                    {
                        await WarnOverdueAsync(plant, "mist", plants
                        .FirstOrDefault(plant => plantDateWithExtraTime > DateTime.Now)?
                        .DateOfNextMisting.AddDays(plant.ExtraMistTime) ?? DateTime.Now,
                        "overdue_notify_mist"
                        );
                        plant.PlantMistOverdueCooldownLastWarned = DateTime.Now;
                    }
                }
            }
            foreach (var plant in plants.Where(_ => _.DateOfNextMove != _.DateOfLastMove && _.UseMoving))
            {
                var plantDateWithExtraTime = plant.DateOfNextMove.AddDays(plant.ExtraMoveTime);
                // could maybe set a flag instead of 2 methods that both run a notification.
                if (plantDateWithExtraTime >= DateTime.Now)
                {
                    plant.IsOverdueSun = false;
                    await FutureNotificationAsync(plant, "move", plantDateWithExtraTime, "future_notify_sun");
                }
                else
                {
                    plant.IsOverdueSun = true;
                    if (COOLDOWN_HOURS != -1 && DateTime.Now.AddHours(COOLDOWN_HOURS) > plant.PlantMoveOverdueCooldownLastWarned)
                    {
                        //this is gross. Fix it!
                        await WarnOverdueAsync(plant, "move", plants
                            .FirstOrDefault(plant => plantDateWithExtraTime > DateTime.Now)?
                            .DateOfNextMove.AddDays(plant.ExtraMoveTime) ?? DateTime.Now,
                            "overdue_notify_sun"
                            );
                        plant.PlantMoveOverdueCooldownLastWarned = DateTime.Now;
                    }
                }
            }

            if (COOLDOWN_MULTI_HOURS != -1 && !string.IsNullOrEmpty(commonPlantOverdueNotificationDescription))
            {
                await RepeatingOverduePlantNotification();
            }

            return plants;
        }

        private async Task RepeatingOverduePlantNotification()
        {
            //write out all of the plant actions that need to be complete but set this for like 24 hours after the plants are set. Only don't have it if there are no overdue plants
            var notification = new NotificationRequest
            {
                NotificationId = 1,
                Title = $"You have overdue plants:",
                Description = commonPlantOverdueNotificationDescription,
                Android =
                {
                    IconSmallName =
                    {
                        ResourceName = "homeicon",
                    }
                },
                ReturningData = "Dummy data", // Returning data when tapped on notification.
                Schedule =
                {
                    NotifyTime = DateTime.Now.AddHours(COOLDOWN_MULTI_HOURS)  // Used for Scheduling local notification, if not specified notification will show immediately.
                }
            };
            await LocalNotificationCenter.Current.Show(notification);
            commonPlantOverdueNotificationDescription = string.Empty;


        }

        private async Task WarnOverdueAsync(Plant plant, string plantAction, DateTime notifyTime, string iconSource)
        {
            var title = $"Overdue {(plantAction.EndsWith("e") ? plantAction.Remove(plantAction.Length - 1, 1) : plantAction)}ing on your '{plant.PlantSpecies}', {plant.GivenName}";
            var notification = new NotificationRequest
            {
                NotificationId = GenerateNotificationId(plant, plantAction),
                Title = title,
                Description = $"You really should {plantAction} this guy",
                Android =
                {
                    IconSmallName =
                    {
                        ResourceName = iconSource,
                    }
                },
                ReturningData = plant.GivenName, // Returning data when tapped on notification.
                Schedule =
                {
                    NotifyTime = notifyTime.AddHours(COOLDOWN_HOURS) // Used for Scheduling local notification, if not specified notification will show immediately.
                }
            };
            commonPlantOverdueNotificationDescription += $"{title}\n";

            await LocalNotificationCenter.Current.Show(notification);
        }

        private async Task FutureNotificationAsync(Plant plant, string plantAction, DateTime notifyTime, string iconSource)
        {
            var notification = new NotificationRequest
            {
                NotificationId = GenerateNotificationId(plant, plantAction),
                Title = $"It's time to {plantAction} your '{plant.PlantSpecies}', {plant.GivenName}",
                Description = "You really should water this guy",
                Android =
                {
                    IconSmallName =
                    {
                        ResourceName = iconSource,
                    }
                },
                ReturningData = plant.GivenName, // Returning data when tapped on notification.
                Schedule =
                {
                    NotifyTime = notifyTime  // Used for Scheduling local notification, if not specified notification will show immediately.
                }
            };
            await LocalNotificationCenter.Current.Show(notification);
        }

        private int GenerateNotificationId(Plant plant, string plantAction)
        {
            //ids are 1 indexed so that 0 does not yield unconventional results
            var id = plant.Id + 1;
            var idWithIsOverdueThenType = 0;
            switch (plantAction)
            {
                case "water":
                    idWithIsOverdueThenType = id * 100 + ((plant.IsOverdueWater ? 1 : 0) * 10) + 0;
                    break;

                case "mist":
                    idWithIsOverdueThenType = id * 100 + ((plant.IsOverdueMist ? 1 : 0) * 10) + 1;
                    break;

                case "move":
                    idWithIsOverdueThenType = id * 100 + ((plant.IsOverdueSun ? 1 : 0) * 10) + 2;
                    break;
            }
            return idWithIsOverdueThenType;
        }
        //Hey Why not try creating a static class that tries to get the byte arrays of the images, and just use them in the Notification Service.
        //That way, you don't have to re-bring up the images!
        //Right now this just returns null
        private static async Task<byte[]> GetNotificationImageAsync(string source)
        {
            byte[] bytes;
            using var stream = await FileSystem.OpenAppPackageFileAsync("overdue_notify_sun.png");
            if (stream is not null)
            {
                using (var memStream = new MemoryStream())
                {
                    await stream.CopyToAsync(memStream);
                    bytes = memStream.ToArray();
                    return bytes;
                }
            }
            return null;
        }
    }
}