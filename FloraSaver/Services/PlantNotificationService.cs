﻿using FloraSaver.Models;
using Plugin.LocalNotification;

namespace FloraSaver.Services
{
    public class PlantNotificationService : IPlantNotificationService
    {
        public void PlantNotificationEnder(Plant plant, string plantAction)
        {
            var notificationId = GenerateNotificationId(plant, plantAction);
            LocalNotificationCenter.Current.Cancel(notificationId);
        }

        public static double COOLDOWN_HOURS { get; set; } = .10;

        public async Task<List<Plant>> SetAllNotificationsAsync(List<Plant> plants)
        {
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
                    await FutureNotificationAsync(plant, "water", plantDateWithExtraTime);
                }
                else
                {
                    plant.IsOverdueWater = true;
                    //this is gross. Fix it!
                    if (DateTime.Now.AddHours(COOLDOWN_HOURS) > plant.PlantWaterOverdueCooldownLastWarned)
                    {
                        await WarnOverdueAsync(plant, "water", plants
                        .FirstOrDefault(plant => plantDateWithExtraTime > DateTime.Now)?
                        .DateOfNextWatering.AddDays(plant.ExtraWaterTime) ?? DateTime.Now);
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
                    await FutureNotificationAsync(plant, "mist", plantDateWithExtraTime);
                }
                else
                {
                    plant.IsOverdueMist = true;
                    //this is gross. Fix it!
                    if (DateTime.Now.AddHours(COOLDOWN_HOURS) > plant.PlantMistOverdueCooldownLastWarned)
                    {
                        await WarnOverdueAsync(plant, "mist", plants
                        .FirstOrDefault(plant => plantDateWithExtraTime > DateTime.Now)?
                        .DateOfNextMisting.AddDays(plant.ExtraMistTime) ?? DateTime.Now);
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
                    await FutureNotificationAsync(plant, "move", plantDateWithExtraTime);
                }
                else
                {
                    plant.IsOverdueSun = true;
                    if (DateTime.Now.AddHours(COOLDOWN_HOURS) > plant.PlantMoveOverdueCooldownLastWarned)
                    {
                        //this is gross. Fix it!
                        await WarnOverdueAsync(plant, "move", plants
                            .FirstOrDefault(plant => plantDateWithExtraTime > DateTime.Now)?
                            .DateOfNextMove.AddDays(plant.ExtraMoveTime) ?? DateTime.Now);
                        plant.PlantMoveOverdueCooldownLastWarned = DateTime.Now;
                    }
                }
            }
            return plants;
        }

        private async Task WarnOverdueAsync(Plant plant, string plantAction, DateTime notifyTime)
        {
            var notification = new NotificationRequest
            {
                NotificationId = GenerateNotificationId(plant, plantAction),
                Title = $"Overdue {(plantAction.EndsWith("e") ? plantAction.Remove(plantAction.Length - 1, 1) : plantAction)}ing on your '{plant.PlantSpecies}', {plant.GivenName}",
                Description = $"You really should {plantAction} this guy",
                ReturningData = "Dummy data", // Returning data when tapped on notification.
                Schedule =
                {
                    NotifyTime = notifyTime // Used for Scheduling local notification, if not specified notification will show immediately.
                }
            };
            await LocalNotificationCenter.Current.Show(notification);
        }

        private async Task FutureNotificationAsync(Plant plant, string plantAction, DateTime notifyTime)
        {
            var notification = new NotificationRequest
            {
                NotificationId = GenerateNotificationId(plant, plantAction),
                Title = $"It's time to {plantAction} your '{plant.PlantSpecies}', {plant.GivenName}",
                Description = "You really should water this guy",
                ReturningData = "Dummy data", // Returning data when tapped on notification.
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
    }
}