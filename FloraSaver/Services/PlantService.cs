using SQLite;
using FloraSaver.Models;

using Plugin.LocalNotification;
using System.Numerics;


namespace FloraSaver.Services
{
    public class PlantService : IPlantService
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
            await conn.CreateTableAsync<PlantGroup>();
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

                StatusMessage = string.Format("{0} record saved (Name: {1})", result, plant.GivenName);
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

        public async Task DeleteAllAsync()
        {
            int result = 0;
            try
            {
                await DeleteAllPlantGroupsAsync();
                // TODO: Call Init()
                await InitAsync();
                
                result = await conn.DeleteAllAsync<Plant>();

                // TODO: Insert the new person into the database


                StatusMessage = string.Format("{0} record(s) deleted. Plant database is now empty", result);
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to delete all plants. Error: {0}", ex.Message);
            }
        }

        public async Task DeleteAllPlantGroupsAsync()
        {
            int result = 0;
            try
            {
                // TODO: Call Init()
                await InitAsync();
                result = await conn.DeleteAllAsync<PlantGroup>();
                var plants = await GetAllPlantAsync();
                foreach (var plant in plants.Where(_ => _.PlantGroupName != "Ungrouped"))
                {
                    plant.PlantGroupName = "Ungrouped";
                    plant.GroupColorHexString = "#A9A9A9";
                    await AddUpdateNewPlantAsync(plant);
                }
                // TODO: Insert the new person into the database


                StatusMessage = string.Format("{0} record(s) deleted. PlantGroup database is now empty", result);
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to delete all PlantGroups. Error: {0}", ex.Message);
            }
        }

        public async Task<List<PlantGroup>> GetAllPlantGroupAsync()
        {
            try
            {
                await InitAsync();
                var allGroups = await conn.Table<PlantGroup>().ToListAsync();
                if (allGroups.Count == 0)
                {
                    await AddUpdateNewPlantGroupAsync(new
                        PlantGroup()
                    {
                        GroupName = "Ungrouped",
                        GroupColorHex = "#A9A9A9"
                    });
                    allGroups = await conn.Table<PlantGroup>().ToListAsync();
                }
                return allGroups;
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to retrieve data. {0}", ex.Message);
            }

            return new List<PlantGroup>();
        }

        public async Task AddUpdateNewPlantGroupAsync(PlantGroup plantGroup)
        {
            int result = 0;
            try
            {
                // TODO: Call Init()
                await InitAsync();
                if (string.IsNullOrEmpty(plantGroup.GroupName))
                    throw new Exception("Valid group name required");

                result = await conn.InsertOrReplaceAsync(plantGroup);

                StatusMessage = string.Format("{0} group saved (Name: {1})", result, plantGroup.GroupName);
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to add group {0}. Error: {1}", plantGroup.GroupName, ex.Message);
            }
        }

        public async Task DeletePlantGroupAsync(PlantGroup plantGroup)
        {
            int result = 0;
            try
            {
                // TODO: Call Init()
                await InitAsync();
                // basic validation to ensure a name was entered
                if (string.IsNullOrEmpty(plantGroup.GroupName))
                    throw new Exception("Valid name required");

                // TODO: Insert the new person into the database
                result = await conn.DeleteAsync(plantGroup);
                
                var plants = await GetAllPlantAsync();
                foreach (var plant in plants.Where(_ => _.PlantGroupName == plantGroup.GroupName))
                {
                    plant.PlantGroupName = "Ungrouped";
                    plant.GroupColorHexString = "#A9A9A9";
                    await AddUpdateNewPlantAsync(plant);
                }
                StatusMessage = string.Format("{0} group(s) deleted (Name: {1})", result, plantGroup.GroupName);

            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to delete {0}. Error: {1}", plantGroup.GroupName, ex.Message);
            }
        }

        public async Task<List<Plant>> GetAllPlantAsync()
        {
            try
            {
                await InitAsync();
                var allPlants = await conn.Table<Plant>().ToListAsync();
#if (ANDROID || IOS)
                await SetAllNotificationsAsync(allPlants); //Ideally the notifications would be decoupled into its own service
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
            foreach (var plant in plants.Where(_ => _.DateOfNextWatering != _.DateOfLastWatering && _.UseWatering))
            {
                var plantDateWithExtraTime = plant.DateOfNextWatering.AddDays(plant.ExtraWaterTime);
                // could maybe set a flag instead of 2 methods that both run a notification.
                if (plantDateWithExtraTime >= DateTime.Now)
                {
                    plant.IsOverdueWater = false;
                    await FutureNotificationAsync(plant, "water");
                }
                else
                {
                    plant.IsOverdueWater = true;
                    //this is gross. Fix it!
                    await WarnOverdueAsync(plant, "water", plants
                        .FirstOrDefault(plant => plantDateWithExtraTime > DateTime.Now)?
                        .DateOfNextWatering.AddDays(plant.ExtraWaterTime) ?? DateTime.Now);
                }
            }
            foreach (var plant in plants.Where(_ => _.DateOfNextMisting != _.DateOfLastMisting && _.UseMisting))
            {
                var plantDateWithExtraTime = plant.DateOfNextMisting.AddDays(plant.ExtraMistTime);
                // could maybe set a flag instead of 2 methods that both run a notification.
                if (plantDateWithExtraTime >= DateTime.Now)
                {
                    plant.IsOverdueMist = false;
                    await FutureNotificationAsync(plant, "mist");
                }
                else
                {
                    plant.IsOverdueMist = true;
                    //this is gross. Fix it!
                    await WarnOverdueAsync(plant, "mist", plants
                        .FirstOrDefault(plant => plantDateWithExtraTime > DateTime.Now)?
                        .DateOfNextMisting.AddDays(plant.ExtraMistTime) ?? DateTime.Now);
                }
            }
            foreach (var plant in plants.Where(_ => _.DateOfNextMove != _.DateOfLastMove && _.UseMoving))
            {
                var plantDateWithExtraTime = plant.DateOfNextMove.AddDays(plant.ExtraMoveTime);
                // could maybe set a flag instead of 2 methods that both run a notification.
                if (plantDateWithExtraTime >= DateTime.Now)
                {
                    plant.IsOverdueWater = false;
                    await FutureNotificationAsync(plant, "move");
                }
                else
                {
                    plant.IsOverdueWater = true;
                    //this is gross. Fix it!
                    await WarnOverdueAsync(plant, "move", plants
                        .FirstOrDefault(plant => plantDateWithExtraTime > DateTime.Now) ?
                        .DateOfNextMove.AddDays(plant.ExtraMoveTime) ?? DateTime.Now);
                }
            }
        }

        private async Task WarnOverdueAsync(Plant plant, string plantAction, DateTime notifyTime)
        {
            var notification = new NotificationRequest
            {
                NotificationId = GenerateNotificationId(plant, plantAction),
                Title = $"Overdue {(plantAction.EndsWith("e") ? plantAction.Remove(plantAction.Length -1, 1) : plantAction)}ing on your '{plant.PlantSpecies}', {plant.GivenName}",
                Description = $"You really should {plantAction} this guy",
                ReturningData = "Dummy data", // Returning data when tapped on notification.
                Schedule =
                {
                    NotifyTime = notifyTime // Used for Scheduling local notification, if not specified notification will show immediately.
                }
            };
            await LocalNotificationCenter.Current.Show(notification);
        }

        private async Task FutureNotificationAsync(Plant plant, string plantAction)
        {
            var notification = new NotificationRequest
            {
                NotificationId = GenerateNotificationId(plant, plantAction),
                Title = $"It's time to {plantAction} your '{plant.PlantSpecies}', {plant.GivenName}",
                Description = "You really should water this guy",
                ReturningData = "Dummy data", // Returning data when tapped on notification.
                Schedule =
                {
                    NotifyTime = plant.DateOfNextWatering  // Used for Scheduling local notification, if not specified notification will show immediately.
                }
            };
            await LocalNotificationCenter.Current.Show(notification);
        }
        public void PlantNotificationEnder(Plant plant, string plantAction)
        {
            var notificationId = GenerateNotificationId(plant, plantAction);
            LocalNotificationCenter.Current.Cancel(notificationId);
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
