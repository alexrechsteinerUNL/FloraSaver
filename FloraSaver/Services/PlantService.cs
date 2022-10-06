using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using FloraSaver.Models;

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
            // TODO: Init then retrieve a list of Person objects from the database into a list
            try
            {
                await InitAsync();
                return await conn.Table<Plant>().ToListAsync();
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to retrieve data. {0}", ex.Message);
            }

            return new List<Plant>();
        }
    }
}
