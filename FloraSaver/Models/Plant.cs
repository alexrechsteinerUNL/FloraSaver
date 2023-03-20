using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace FloraSaver.Models
{
    [Table("plant")]
    public class Plant
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
        [MaxLength(250), Unique]
        public string GivenName
        {
            get { return givenName; }
            set { givenName = value?.Trim() ?? string.Empty; }
        }

        public DateTime DateOfBirth { get; set; }
        public DateTime DateOfLastWatering { get; set; }
        public TimeSpan TimeOfLastWatering { get; set; }
        public DateTime DateOfNextWatering { get; set; }
        public TimeSpan TimeOfNextWatering { get; set; }
        public DateTime DateOfLastMisting { get; set; }
        public TimeSpan TimeOfLastMisting { get; set; }
        public DateTime DateOfNextMisting { get; set; }
        public TimeSpan TimeOfNextMisting { get; set; }
        public int WaterInterval { get; set; }
        public DateTime MistInterval { get; set; }
        public string ImageLocation { get; set; }

        public double TimeToNextAction(DateTime lastTime, DateTime nextTime)
        {
            return (DateTime.Now - lastTime).TotalSeconds /
                (nextTime - lastTime).TotalSeconds;
        }
    }
}
