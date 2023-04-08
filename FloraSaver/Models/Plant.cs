using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace FloraSaver.Models
{
    [Table("plant")]
    public class Plant : IPlant
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        private string plantSpecies;
        public string PlantSpecies
        {
            get { return plantSpecies; }
            set { plantSpecies = value?.Trim() ?? string.Empty; }
        }

        private string givenName;
        [SQLite.MaxLength(250), Unique]
        public string GivenName
        {
            get { return givenName; }
            set { givenName = value?.Trim() ?? string.Empty; }
        }

        public DateTime DateOfBirth { get; set; }
        public bool UseWatering { get; set; }
        public bool UseMisting { get; set; }
        public bool UseMoving { get; set; }
        public DateTime DateOfLastWatering { get; set; }
        public TimeSpan TimeOfLastWatering { get; set; }
        public DateTime DateOfNextWatering { get; set; }
        public TimeSpan TimeOfNextWatering { get; set; }
        public DateTime DateOfLastMisting { get; set; }
        public TimeSpan TimeOfLastMisting { get; set; }
        public DateTime DateOfNextMisting { get; set; }
        public TimeSpan TimeOfNextMisting { get; set; }
        public DateTime DateOfLastMove { get; set; }
        public TimeSpan TimeOfLastMove { get; set; }
        public DateTime DateOfNextMove { get; set; }
        public TimeSpan TimeOfNextMove { get; set; }
        [Range(0, 365)]
        public int? WaterInterval { get; set; }
        [Range(0, 365)]
        public int? MistInterval { get; set; }
        [Range(0, 365)]
        public int? SunInterval { get; set; }
        public string ImageLocation { get; set; }

        public double TimeToNextAction(DateTime lastTime, DateTime nextTime)
        {
            return (DateTime.Now - lastTime).TotalSeconds /
                (nextTime - lastTime).TotalSeconds;
        }
    }
}
