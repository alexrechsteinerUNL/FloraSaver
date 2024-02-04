using FloraSaver.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloraSaver.Utilities
{
    public static class SearchScoreGeneratorUtility
    {

        public static List<SearchedPlants> GenerateSearchScore(List<SearchedPlants> databasePlants, string searchQuery, List<Plant> existingPlants = null, int desiredResultAmount = 15)
        {
            searchQuery = searchQuery.ToLower();
            databasePlants = RecursiveSearch(databasePlants, searchQuery, true, existingPlants, desiredResultAmount);
            return databasePlants;
        }

        private static List<SearchedPlants> RecursiveSearch(List<SearchedPlants> databasePlants, string searchQuery, bool isStartingLetter, List<Plant> existingPlants = null, int desiredResultAmount = 15)
        {
            var plantSpecies = databasePlants.Select(_ => _.PlantSpecies.ToLower());
            Parallel.ForEach(databasePlants, plant =>
            {
                if (isStartingLetter && plant.PlantSpecies.StartsWith(searchQuery))
                {
                    plant.SearchScore += 50 + 10 * searchQuery.Length;
                }
                if (plant.PlantSpecies.Contains(searchQuery))
                {
                    plant.SearchScore += 20 + 10 * searchQuery.Length;
                }
            });

            if (databasePlants.Where(_ => _.SearchScore > 0).Count() > desiredResultAmount || searchQuery.Length == 1)
            {
                return databasePlants;
            }

            var firstHalf = (searchQuery.Length % 2 == 0) ? searchQuery.Substring(0, searchQuery.Length / 2) : searchQuery.Substring(0, searchQuery.Length / 2 + 1);
            var secondHalf = (searchQuery.Length % 2 == 0) ? searchQuery.Substring(searchQuery.Length / 2, searchQuery.Length) : searchQuery.Substring(searchQuery.Length / 2 + 1, searchQuery.Length);


            databasePlants = RecursiveSearch(databasePlants, firstHalf, true, existingPlants, desiredResultAmount);
            databasePlants = RecursiveSearch(databasePlants, secondHalf, false, existingPlants, desiredResultAmount);


            return databasePlants;
        }
    }
}
