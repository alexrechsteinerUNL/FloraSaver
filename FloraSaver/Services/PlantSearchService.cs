using FloraSaver.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace FloraSaver.Services
{
    public class PlantSearchService
    {
        public async Task<ObservableCollection<Plant>> SearchPlantsAsync(string inputString, ObservableCollection<Plant> plants)
        {
            inputString = inputString.Trim();
            await BonusComputer(inputString, plants);

            return plants;
        }

        private async Task<Dictionary<string, int>> BonusComputer(string inputString, ObservableCollection<Plant> plants)
        {
            var bonusDictionary = new Dictionary<string, int>();
            bonusDictionary["GivenNameBonus"] = 0;
            bonusDictionary["PlantSpeciesBonus"] = 0;
            bonusDictionary["GroupNameBonus"] = 0;
            bonusDictionary["NextDateBonus"] = 0;
            bonusDictionary["LastDateBonus"] = 0;
            //bonus methods go here
            var numberBonus = NumberBonus(inputString, bonusDictionary);
            await Task.WhenAll(numberBonus);
            bonusDictionary["GivenNameBonus"] += 10;
            bonusDictionary["GroupNameBonus"] += 5;
            return new Dictionary<string, int>();
        }

        private async Task<Dictionary<string, int>> NumberBonus(string inputString, Dictionary<string, int> bonusDictionary)
        {
            if (char.IsNumber(inputString[0]))
            {
                bonusDictionary["NextDateBonus"] = 30;
                bonusDictionary["LastDateBonus"] = 30;
            }
            else
            {
                bonusDictionary["GivenNameBonus"] = 15;
                bonusDictionary["PlantSpeciesBonus"] = 15;
                bonusDictionary["GroupNameBonus"] = 15;
            }
            return bonusDictionary;
        }

        private async Task<ObservableCollection<Plant>> PlantsClosestPlantGivenName(string inputString, ObservableCollection<Plant> plants, int bonusPoints = 0)
        {
            return plants;
        }

        private async Task<ObservableCollection<Plant>> PlantsClosestPlantSpecies(string inputString, ObservableCollection<Plant> plants, int bonusPoints = 0)
        {
            return plants;
        }

        private async Task<ObservableCollection<Plant>> PlantsClosestGroupName(string inputString, ObservableCollection<Plant> plants, int bonusPoints = 0)
        {
            return plants;
        }

        private async Task<ObservableCollection<Plant>> PlantsClosestNextPlantDate(string inputString, ObservableCollection<Plant> plants, int bonusPoints = 0)
        {
            return plants;
        }

        private async Task<ObservableCollection<Plant>> PlantsClosestLastPlantDate(string inputString, ObservableCollection<Plant> plants, int bonusPoints = 0)
        {
            return plants;
        }
    }
}