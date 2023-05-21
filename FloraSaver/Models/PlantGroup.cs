using CommunityToolkit.Mvvm.ComponentModel;
using SQLite;

namespace FloraSaver.Models
{
    [Table("plantGroup")]
    public class PlantGroup : ObservableObject
    {
        [PrimaryKey, AutoIncrement]
        public int GroupId { get; set; }

        public string GroupName { get; set; }

        public List<int> Plants { get; set; }
        public string GroupColorHex { get; set; }
    }
}
