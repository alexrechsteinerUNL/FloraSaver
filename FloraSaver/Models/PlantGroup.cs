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

        private bool _isEnabled = true;
        [Ignore]
        public bool IsEnabled {
            get
            {
                return _isEnabled;
            }
            set
            {
                _isEnabled = value;
                OnPropertyChanged(nameof(SelectedColor));
                OnPropertyChanged(nameof(SelectedTextColor));
            } 
        } 

        [Ignore]
        public Color SelectedColor => IsEnabled ? Color.FromArgb("#e1ad01") : Color.FromArgb("#000000");
        [Ignore]
        public Color SelectedTextColor => IsEnabled ? Color.FromArgb("#000000") : Color.FromArgb("#FFFFFF");
    }
}
