using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using FloraSaver.Models;
#if (ANDROID || IOS)
using Plugin.LocalNotification;
#endif


namespace FloraSaver.Services
{
    public class PlantService
    {
        string _dbPath;

        public string StatusMessage { get; set; }

        // TODO: Add variable for the SQLite connection
        private SQLiteAsyncConnection conn;
        private async Task InitAsync()
        {
            if (conn != null)
                return;

            conn = new SQLiteAsyncConnection(_dbPath);

            await conn.CreateTableAsync<Plant>();
        }

        public PlantService(string dbPath)
        {
            _dbPath = dbPath;
        }

        public async Task AddUpdateNewPlantAsync(Plant plant)
        {
            int result = 0;
            try
            {
                // TODO: Call Init()
                await InitAsync();
                if (string.IsNullOrEmpty(plant.GivenName) || string.IsNullOrEmpty(plant.PlantSpecies))
                    throw new Exception("Valid name required");
                result = await conn.InsertOrReplaceAsync(plant);

                StatusMessage = string.Format("{0} record(s) added (Name: {1})", result, plant.GivenName);
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to add {0}. Error: {1}", plant.GivenName, ex.Message);
            }
        }

        public async Task DeletePlantAsync(Plant plant)
        {
            int result = 0;
            try
            {
                // TODO: Call Init()
                await InitAsync();
                // basic validation to ensure a name was entered
                if (string.IsNullOrEmpty(plant.GivenName))
                    throw new Exception("Valid name required");

                // TODO: Insert the new person into the database
                result = await conn.DeleteAsync(plant);

                StatusMessage = string.Format("{0} record(s) deleted (Name: {1})", result, plant.GivenName);
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to delete {0}. Error: {1}", plant.GivenName, ex.Message);
            }
        }

        public async Task<List<Plant>> GetAllPlantAsync()
        {
            try
            {
                await InitAsync();
                var allPlants = await conn.Table<Plant>().ToListAsync();
#if (ANDROID || IOS)
                await SetAllNotificationsAsync(allPlants);
#endif
                return allPlants;
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to retrieve data. {0}", ex.Message);
            }

            return new List<Plant>();
        }

        public async Task SetAllNotificationsAsync(List<Plant> plants)
        {
            foreach (var plant in plants)
            {
                // could maybe set a flag instead of 2 methods that both run a notification.
                if (plant.DateOfNextWatering > DateTime.Now)
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
                    NotifyTime = plant.DateOfNextWatering // Used for Scheduling local notification, if not specified notification will show immediately.
                }
            };
            await LocalNotificationCenter.Current.Show(notification);
        }


    }
}
