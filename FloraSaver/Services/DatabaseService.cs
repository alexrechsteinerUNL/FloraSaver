using SQLite;
using FloraSaver.Models;

using Plugin.LocalNotification;
using System.Numerics;
using CommunityToolkit.Maui.Storage;


namespace FloraSaver.Services
{
    public class DatabaseService : IDatabaseService
    {
        private readonly IPlantNotificationService _plantNotificationService;
        private readonly IFileSaver _fileSaver;
        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        string _dbPath;
        
        public string StatusMessage { get; set; }

        // TODO: Add variable for the SQLite connection
        private SQLiteAsyncConnection conn;
        //Testing May want to expand this into its own test service that inherits from the main service
        public async Task TestDbConnectionFromFileAsync(string filePath)
        {
            var testConn = new SQLiteAsyncConnection(filePath);
            var bloop = await TestGetAllPlantAsync(testConn);
            if (bloop.Count > 0)
            {
                Console.WriteLine("Ah nice!");
                return;
            }
            Console.WriteLine("Aw Darnit!");
            return;
        }
        
        private async Task<List<Plant>> TestGetAllPlantAsync(SQLiteAsyncConnection testConn)
        {
            try
            {
                await InitAsync();
                var allPlants = await testConn.Table<Plant>().ToListAsync();
                return allPlants;
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to retrieve data. {0}", ex.Message);
            }

            return new List<Plant>();
        }
        //End Testing
        private async Task InitAsync()
        {
            if (conn != null)
                return;

            conn = new SQLiteAsyncConnection(_dbPath);

            await conn.CreateTableAsync<Plant>();
            await conn.CreateTableAsync<PlantGroup>();
        }

        public DatabaseService(string dbPath, IPlantNotificationService plantNotificationService, IFileSaver fileSaver)
        {
            _plantNotificationService = plantNotificationService;
            _dbPath = dbPath;
            _fileSaver = fileSaver;
        }

        public async Task BackupDatabaseAsync(string databaseFileName)
        {
            await InitAsync();
            await _fileSaver.SaveAsync(databaseFileName, File.OpenRead(_dbPath), cancellationTokenSource.Token);
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
                await InitAsync();
                if (string.IsNullOrEmpty(plant.GivenName))
                    throw new Exception("Valid name required");
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
                //make sure that the ungrouped group is set! You might need to do it here
                await SetPlantsToGroupAsync(plants);
                //foreach (var plant in plants.Where(_ => _.PlantGroupName != "Ungrouped"))
                //{
                //    plant.PlantGroupName = "Ungrouped";
                //    plant.GroupColorHexString = "#A9A9A9";
                //    await AddUpdateNewPlantAsync(plant);
                //}


                StatusMessage = string.Format("{0} record(s) deleted. PlantGroup database is now empty", result);
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to delete all PlantGroups. Error: {0}", ex.Message);
            }
        }

        public async Task SetPlantsToGroupAsync(IEnumerable<Plant> plantsGroup, string groupName = "Ungrouped", string groupColorHex = "#A9A9A9")
        {
            foreach (var plant in plantsGroup)
            {
                plant.PlantGroupName = groupName;
                plant.GroupColorHexString = groupColorHex;
                await AddUpdateNewPlantAsync(plant);
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
                var plants = await GetAllPlantAsync();
                //foreach (var plant in plants.Where(_ => _.PlantGroupName == plantGroup.GroupName))
                //{
                //    plant.PlantGroupName = "Ungrouped";
                //    plant.GroupColorHexString = "#A9A9A9";
                //    await AddUpdateNewPlantAsync(plant);
                //}

                await SetPlantsToGroupAsync(plants.Where(_ => _.PlantGroupName == plantGroup.GroupName), plantGroup.GroupName, plantGroup.GroupColorHex);

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
                //foreach (var plant in plants.Where(_ => _.PlantGroupName == plantGroup.GroupName))
                //{
                //    plant.PlantGroupName = "Ungrouped";
                //    plant.GroupColorHexString = "#A9A9A9";
                //    await AddUpdateNewPlantAsync(plant);
                //}

                await SetPlantsToGroupAsync(plants.Where(_ => _.PlantGroupName == plantGroup.GroupName));
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
                allPlants = await _plantNotificationService.SetAllNotificationsAsync(allPlants);
                foreach (var plant in allPlants) 
                {
                    await AddUpdateNewPlantAsync(plant);
                }
#endif
                return allPlants;
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to retrieve data. {0}", ex.Message);
            }

            return new List<Plant>();
        }
    }
}
