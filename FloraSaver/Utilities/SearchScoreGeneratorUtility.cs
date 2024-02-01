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
        public static List<SearchedPlants> GenerateSearchScore(List<SearchedPlants> databasePlants, string searchQuery, List<Plant> existingPlants = null)
        {
            searchQuery = searchQuery.ToLower();
            return databasePlants;
        }

        private static List<SearchedPlants> SearchIfSearchQueryOneLetter(List<SearchedPlants> databasePlants, string searchQuery, bool isStartingLetter, List<Plant> existingPlants = null)
        {
            var plantSpecies = databasePlants.Select(_ => _.PlantSpecies.ToLower());
            Parallel.ForEach(databasePlants, plant =>
            {
                if (isStartingLetter && plant.PlantSpecies.StartsWith(searchQuery))
                {
                    plant.SearchScore += 50;
                }
                if (plant.PlantSpecies.Contains(searchQuery))
                {
                    plant.SearchScore += 20;
                }
            });
            return databasePlants;
        }

        private static List<SearchedPlants> SearchIfSearchQueryMultipleLetters(List<SearchedPlants> databasePlants, string searchQuery, List<Plant> existingPlants = null)
        {
            var plantSpecies = databasePlants.Select(_ => _.PlantSpecies.ToLower());
            Parallel.ForEach(databasePlants, plant =>
            {
                if (plant.PlantSpecies.StartsWith(searchQuery))
                {
                    plant.SearchScore += 75 + 5 * searchQuery.Length;
                }
                if (plant.PlantSpecies.Contains(searchQuery))
                {
                    plant.SearchScore += 40 + 5 * searchQuery.Length; ;
                }
            });
            var isStartingLetter = true;
            Parallel.ForEach(searchQuery, character =>
            {
                SearchIfSearchQueryOneLetter(databasePlants, character.ToString(), isStartingLetter, existingPlants);
                isStartingLetter = false;
            });


            return databasePlants;
        }

        private static List<SearchedPlants> SearchIfSearchQueryThreeLettersOrMore(List<SearchedPlants> databasePlants, string searchQuery, List<Plant> existingPlants = null)
        {
            return databasePlants;
        }
    }
}
