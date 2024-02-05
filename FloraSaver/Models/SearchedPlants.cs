using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloraSaver.Models
{
    public class SearchedPlants : IPlant
    {
        public SearchedPlants(IPlant _plant)
        {
            Id = _plant.Id;
            PlantSpecies = _plant.PlantSpecies;
            WaterInterval = _plant.WaterInterval;
            MistInterval = _plant.MistInterval;
            SunInterval = _plant.SunInterval;
        }
        public SearchedPlants(IPlant _plant, double searchScore)
        {
            Id = _plant.Id;
            PlantSpecies = _plant.PlantSpecies;
            WaterInterval = _plant.WaterInterval;
            MistInterval = _plant.MistInterval;
            SunInterval = _plant.SunInterval;
            SearchScore = searchScore;
        }

        public SearchedPlants(Plant _plant, double searchScore)
        {
            Id = _plant.Id;
            PlantSpecies = _plant.PlantSpecies;
            WaterInterval = _plant.WaterInterval;
            MistInterval = _plant.MistInterval;
            SunInterval = _plant.SunInterval;
            GivenName = _plant.GivenName;
            IsPlantExisting = true;
            SearchScore = searchScore;
        }

        public SearchedPlants(Plant _plant)
        {
            Id = _plant.Id;
            PlantSpecies = _plant.PlantSpecies;
            WaterInterval = _plant.WaterInterval;
            MistInterval = _plant.MistInterval;
            SunInterval = _plant.SunInterval;
            GivenName = _plant.GivenName;
            IsPlantExisting = true;
        }




        public int Id { get; set; }
        public string PlantSpecies { get; set; }
        public double? WaterInterval { get; set; }
        public double? MistInterval { get; set; }
        public double? SunInterval { get; set; }
        public double SearchScore { get; set; } = 0.0;
        public bool IsPlantExisting { get; set; } = false;
        public string GivenName { get; set; }

        public string ConnectedIcon => IsPlantExisting ? "🗐" : "+";
    }
}
