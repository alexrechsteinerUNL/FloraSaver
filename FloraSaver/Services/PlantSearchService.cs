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
            return plants;
        }

        private async Task<ObservableCollection<Plant>> PlantsClosestPlantGivenName(string inputString, ObservableCollection<Plant> plants)
        {
            return plants;
        }

        private async Task<ObservableCollection<Plant>> PlantsClosestPlantSpecies(string inputString, ObservableCollection<Plant> plants)
        {
            return plants;
        }

        private async Task<ObservableCollection<Plant>> PlantsClosestGroupName(string inputString, ObservableCollection<Plant> plants)
        {
            return plants;
        }

        private async Task<ObservableCollection<Plant>> PlantsClosestPlantDate(string inputString, ObservableCollection<Plant> plants)
        {
            return plants;
        }
    }
}
