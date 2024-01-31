using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloraSaver.Models
{
    class SearchedPlants : IPlant
    {
        SearchedPlants(IPlant _plant)
        {
            Id = _plant.Id;
            PlantSpecies = _plant.PlantSpecies;
            WaterInterval = _plant.WaterInterval;
            MistInterval = _plant.MistInterval;
            SunInterval = _plant.SunInterval;
        }
        SearchedPlants(IPlant _plant, string searchQuery)
        {
            Id = _plant.Id;
            PlantSpecies = _plant.PlantSpecies;
            WaterInterval = _plant.WaterInterval;
            MistInterval = _plant.MistInterval;
            SunInterval = _plant.SunInterval;
        }

        SearchedPlants(Plant _plant, string searchQuery)
        {
            Id = _plant.Id;
            PlantSpecies = _plant.PlantSpecies;
            WaterInterval = _plant.WaterInterval;
            MistInterval = _plant.MistInterval;
            SunInterval = _plant.SunInterval;
            IsPlantExisting = true;
        }

        SearchedPlants(Plant _plant)
        {
            Id = _plant.Id;
            PlantSpecies = _plant.PlantSpecies;
            WaterInterval = _plant.WaterInterval;
            MistInterval = _plant.MistInterval;
            SunInterval = _plant.SunInterval;
            IsPlantExisting = true;
        }




        public int Id { get; set; }
        public string PlantSpecies { get; set; }
        public double? WaterInterval { get; set; }
        public double? MistInterval { get; set; }
        public double? SunInterval { get; set; }
        public double SearchScore { get; set; } = 0.0;
        public bool IsPlantExisting { get; set; } = false;

        public string ConnectedIcon => IsPlantExisting ? "+" : "🗐";
        public double GenerateSearchScore()
        {
            return (double)SearchScore;
        }
    }
}
