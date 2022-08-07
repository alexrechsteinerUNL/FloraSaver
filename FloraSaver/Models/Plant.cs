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
        public string PlantSpecies { get; set; }
        [MaxLength(250), Unique]
        public string GivenName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime DateOfLastWatering { get; set; }
        public DateTime DateOfNextWatering { get; set; }
        public string ImageLocation { get; set; }

    }
}
