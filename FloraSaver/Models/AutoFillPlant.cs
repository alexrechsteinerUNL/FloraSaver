using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloraSaver.Models
{
    [Table("AutoFillPlant")]
    public class AutoFillPlant : IPlant
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string PlantSpecies { get; set; }
        public double? WaterInterval { get; set; }
        public double? MistInterval { get; set; }
        public double? SunInterval { get; set; }
        public string PlantSource { get; set; }

        //maybe don't just do the base and instead use predefinied intervals to construct the plants when they are found from the database.
        public AutoFillPlant(IPlant _Plant, string source = null)
        {
            if (!string.IsNullOrEmpty(source))
            {
                PlantSource = source;
            }
        }

        public AutoFillPlant(string plantSpecies, double? waterInterval, double? mistInterval, double? sunInterval, string plantSource)
        {
            PlantSpecies = plantSpecies;
            WaterInterval = waterInterval;
            MistInterval = mistInterval;
            SunInterval = sunInterval;
            PlantSource = plantSource;
        }

        public AutoFillPlant() : base()
        {
        }
    }
}
