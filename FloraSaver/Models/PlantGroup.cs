using CommunityToolkit.Mvvm.ComponentModel;
using SQLite;
using System;

namespace FloraSaver.Models
{
    [Table("plantgroup")]
    public class PlantGroup : ObservableObject
    {
        [PrimaryKey, AutoIncrement]
        public int GroupId { get; set; }
        [SQLite.MaxLength(250), Unique]
        public string GroupName { get; set; }
        public string GroupColorHex { get; set; }
        [Ignore]
        public Color GroupColor => Color.FromArgb(GroupColorHex);
    }
}
