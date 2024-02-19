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
            PlantSpecies = _plant.PlantSpecies;
            WaterInterval = _plant.WaterInterval;
            MistInterval = _plant.MistInterval;
            SunInterval = _plant.SunInterval;
        }
        public SearchedPlants(IPlant _plant, double searchScore)
        {
            PlantSpecies = _plant.PlantSpecies;
            WaterInterval = _plant.WaterInterval;
            MistInterval = _plant.MistInterval;
            SunInterval = _plant.SunInterval;
            SearchScore = searchScore;
        }

        public SearchedPlants(Plant _plant, double searchScore)
        {
            PlantSpecies = _plant.PlantSpecies;
            WaterInterval = _plant.WaterInterval;
            MistInterval = _plant.MistInterval;
            SunInterval = _plant.SunInterval;
            GivenName = _plant.GivenName;
            IsPlantExisting = true;
            SearchScore = searchScore;
            PlantGroupName = _plant.PlantGroupName;
            GroupColorHexString = _plant.GroupColorHexString;
            GroupColor = _plant.GroupColor;
        }

        public SearchedPlants(Plant _plant)
        {
            PlantSpecies = _plant.PlantSpecies;
            WaterInterval = _plant.WaterInterval;
            MistInterval = _plant.MistInterval;
            SunInterval = _plant.SunInterval;
            GivenName = _plant.GivenName;
            IsPlantExisting = true;
            PlantGroupName = _plant.PlantGroupName;
            GroupColorHexString = _plant.GroupColorHexString;
            GroupColor = _plant.GroupColor;
        }




        public int Id { get; set; } = 0;
        public string PlantSpecies { get; set; } = "";
        public double? WaterInterval { get; set; } = 0.0;
        public double? MistInterval { get; set; } = 0.0;
        public double? SunInterval { get; set; } = 0.0;
        public double SearchScore { get; set; } = 0.0;
        public bool IsPlantExisting { get; set; } = false;
        public string GivenName { get; set; } = "";
        public string PlantGroupName { get; set; } = "Ungrouped";
        public Color GroupColor { get; set; } = Colors.Transparent;
        public string GroupColorHexString { get; set; } = "#A9A9A9";

        public string ConnectedIcon => IsPlantExisting ? "♢" : "+";
    }
}
