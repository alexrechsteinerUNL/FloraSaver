using SQLite;
using FloraSaver.Models;

using Plugin.LocalNotification;
using System.Numerics;
using CommunityToolkit.Maui.Storage;
using FloraSaver.Utilities;

namespace FloraSaver.Services
{
    public class DatabaseService : IDatabaseService
    {
        private readonly IPlantNotificationService _plantNotificationService;
        private readonly IFileSaver _fileSaver;
        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private string _dbPath;

        public string StatusMessage { get; set; }

        // TODO: Add variable for the SQLite connection
        private SQLiteAsyncConnection conn;

        //Testing May want to expand this into its own test service that inherits from the main service
        public async Task<List<Plant>> TestDbConnectionFromFileAsync(string filePath)
        {
            var testConn = new SQLiteAsyncConnection(filePath);
            return await TestGetAllPlantAsync(testConn);
        }

        public async Task<List<ClipetDialog>> GetAllClipetDialogsAsync()
        {
            try
            {
                //await InitAsync();
                var allClipetDialogs = await conn.Table<ClipetDialog>().ToListAsync();
                return allClipetDialogs;
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to retrieve data from Autofill Database. {0}", ex.Message);
            }

            return new List<ClipetDialog>();
        }

        public async Task UpdateClipetDialogTableAsync(List<ClipetDialog> Dialogs)
        {
            if (Dialogs.Count > 0)
            {
                await conn.UpdateAllAsync(Dialogs, true);
            }
            
        }

        private async Task PopulateClipetDialogTableAsync()
        {
            // You will need to alter this by either prepending the correct amount manually every time you update it or something smarter
                var clipetDialogCount = Preferences.Default.Get("ClipetDialogCount", 0);
                var currentDialogData = await conn.Table<ClipetDialog>().ToListAsync();
                if (clipetDialogCount != 0 && clipetDialogCount == currentDialogData.Count && clipetDialogCount == ClipetDialogGeneratorUtility.ManualClipetDialogs)
                {
                    return;
                }
                ClipetDialogGeneratorUtility.GenerateClipetDialogs();
                await conn.UpdateAllAsync(ClipetDialogGeneratorUtility.AllClipetDialogs, true);
            
        }











        public async Task<List<AutoFillPlant>> GetAllAutofillPlantAsync()
        {
            try
            {
                //await InitAsync();
                var allAutoFillPlants = await conn.Table<AutoFillPlant>().ToListAsync();
                return allAutoFillPlants;
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to retrieve data from Autofill Database. {0}", ex.Message);
            }

            return new List<AutoFillPlant>();
        }

        private async Task PopulateAutoFillPlantTableAsync()
        {
            // You will need to alter this by either prepending the correct amount manually every time you update it or something smarter

            var autoFillPlantCount = Preferences.Default.Get("AutoFillPlantCount", 0);
            var autoFillData = await conn.Table<AutoFillPlant>().ToListAsync();
            if (autoFillPlantCount != 0 && autoFillPlantCount == autoFillData.Count && autoFillPlantCount == AutoFillPlantGeneratorUtility.ManualPlantCount)
            {
                return;
            }
            AutoFillPlantGeneratorUtility.GenerateAutoFillPlants();
            await conn.DeleteAllAsync<AutoFillPlant>();
            await conn.InsertAllAsync(AutoFillPlantGeneratorUtility.AllAutoFillPlants, true);
        }

        private async Task<List<AutoFillPlant>> QueryAutofillPlantAsyncFromSearchAsync(string searchQuery)
        {
            try
            {
                //right about now, these will be ordered by index when they should probably be ordered through some search scoring method
                await InitAsync();
                var topTenAutoFillPlants = await conn.Table<AutoFillPlant>().Where(_ => _.PlantSpecies.Contains(searchQuery)).Take(10).ToListAsync();
                return topTenAutoFillPlants;
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to retrieve data from Autofill Database. {0}", ex.Message);
            }

            return new List<AutoFillPlant>();
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
            //Testing
            if (conn != null)
            {
                return;
            }


            conn = new SQLiteAsyncConnection(_dbPath);

            await conn.CreateTableAsync<Plant>();
            await conn.CreateTableAsync<PlantGroup>();
            await conn.CreateTableAsync<AutoFillPlant>();
            await conn.CreateTableAsync<ClipetDialog>();
            await PopulateAutoFillPlantTableAsync();
            await PopulateClipetDialogTableAsync();
            //Testing
        }

        public DatabaseService(string dbPath, IPlantNotificationService plantNotificationService, IFileSaver fileSaver)
        {
            _plantNotificationService = plantNotificationService;
            _dbPath = dbPath;
            _fileSaver = fileSaver;
        }

        public async Task<FileSaverResult> BackupDatabaseAsync(string databaseFileName)
        {
            await InitAsync();
            return await _fileSaver.SaveAsync(databaseFileName, File.OpenRead(_dbPath), cancellationTokenSource.Token);
        }

        public async Task AddUpdateNewPlantAsync(Plant plant)
        {
            int result = 0;
            try
            {
                // TODO: Call Init()
                await InitAsync();
                var plantGroupsTask = GetAllPlantGroupAsync();
                if (string.IsNullOrEmpty(plant.GivenName) || string.IsNullOrEmpty(plant.PlantSpecies))
                    throw new Exception("Valid name required");

                result = await conn.InsertOrReplaceAsync(plant);

                StatusMessage = string.Format("{0} record saved (Name: {1})", result, plant.GivenName);

                var plantGroups = await plantGroupsTask;
                if (!plantGroups.Select(_ => _.GroupName).Contains(plant.PlantGroupName))
                {
                    await AddUpdateNewPlantGroupAsync(new PlantGroup { GroupId = plantGroups.Count + 1, GroupName = plant.PlantGroupName, GroupColorHex = plant.GroupColor.ToHex() }, false);
                }
#if (ANDROID || IOS)
                var plants = await GetAllPlantAsync();
                await _plantNotificationService.SetAllNotificationsAsync(plants);
#endif
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
#if (ANDROID || IOS)
                var plants = await GetAllPlantAsync();
                await _plantNotificationService.SetAllNotificationsAsync(plants);
#endif
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to delete {0}. Error: {1}", plant.GivenName, ex.Message);
            }
        }

        public async Task DeleteAllPlantsAsync()
        {
            int result = 0;
            try
            {
                await InitAsync();

                result = await conn.DeleteAllAsync<Plant>();

                StatusMessage = string.Format("{0} record(s) deleted. Plant database is now empty", result);
#if (ANDROID || IOS)
                await _plantNotificationService.SetAllNotificationsAsync(null);
#endif
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to delete all plants. Error: {0}", ex.Message);
            }
        }

        public async Task DeleteAllAsync()
        {
            int result = 0;
            try
            {
                await DeleteAllPlantGroupsAsync();
                await InitAsync();

                result = await conn.DeleteAllAsync<Plant>();

                StatusMessage = string.Format("{0} record(s) deleted. Plant database is now empty", result);
#if (ANDROID || IOS)
                await _plantNotificationService.SetAllNotificationsAsync(null);
#endif
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
#if (ANDROID || IOS)
                await _plantNotificationService.SetAllNotificationsAsync(plants);
#endif
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

        public async Task AddUpdateNewPlantGroupAsync(PlantGroup plantGroup, bool setPlants = true, string oldPlantNameIfApplicable = null)
        {
            int result = 0;
            try
            {
                // TODO: Call Init()
                await InitAsync();
                if (string.IsNullOrEmpty(plantGroup.GroupName))
                    throw new Exception("Valid group name required");

                result = await conn.InsertOrReplaceAsync(plantGroup);

                //foreach (var plant in plants.Where(_ => _.PlantGroupName == plantGroup.GroupName))
                //{
                //    plant.PlantGroupName = "Ungrouped";
                //    plant.GroupColorHexString = "#A9A9A9";
                //    await AddUpdateNewPlantAsync(plant);
                //}
                var plants = await GetAllPlantAsync();
                if (setPlants)
                {
                    var selectGroupName = string.IsNullOrEmpty(oldPlantNameIfApplicable) ? plantGroup.GroupName : oldPlantNameIfApplicable;

                    await SetPlantsToGroupAsync(plants.Where(_ => _.PlantGroupName == selectGroupName), plantGroup.GroupName, plantGroup.GroupColorHex);
                }
#if (ANDROID || IOS)
                await _plantNotificationService.SetAllNotificationsAsync(plants);
#endif
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
#if (ANDROID || IOS)
                await _plantNotificationService.SetAllNotificationsAsync(plants);
#endif
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