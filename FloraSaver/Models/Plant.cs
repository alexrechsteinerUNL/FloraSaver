using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui.Controls.Shapes;
using SQLite;

namespace FloraSaver.Models
{
    [Table("plant")]
    public class Plant : ObservableObject, IPlant
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

        public string PlantGroupName { get; set; } = "Ungrouped";
        public string GroupColorHexString { get; set; } = "#A9A9A9";

        [Ignore]
        public Color GroupColor => Color.FromArgb(GroupColorHexString);

        public DateTime DateOfBirth { get; set; }
        public bool IsOverdueWater { get; set; } = false;
        public bool IsOverdueMist { get; set; } = false;
        public bool IsOverdueSun { get; set; } = false;
        
        private bool _useWatering;
        private bool _useMisting;
        private bool _useMoving;

        public bool UseWatering 
        {
            get { return _useWatering; }
            set 
            {
                _useWatering = value;
                if (value)
                {
                    _waterOpacity = 1;
                } else
                {
                    _waterOpacity = .3;
                }
            }
        }
        public bool UseMisting
        {
            get { return _useMisting; }
            set
            {
                _useMisting = value;
                if (value)
                {
                    _mistOpacity = 1;
                }
                else
                {
                    _mistOpacity = .3;
                }
            }
        }
        public bool UseMoving
        {
            get { return _useMoving; }
            set
            {
                _useMoving = value;
                if (value)
                {
                    _moveOpacity = 1;
                }
                else
                {
                    _moveOpacity = .3;
                }
            }
        }
        public DateTime DateOfLastWatering { get; set; }
        
        private TimeSpan _timeOfLastWatering;
        public TimeSpan TimeOfLastWatering
        {
            get
            {
                return _timeOfLastWatering;
            }
            set
            {
                _timeOfLastWatering = value;
                DateOfLastWatering = DateOfLastWatering.Date.Add(value);
            }
        }
        public DateTime DateOfNextWatering { get; set; }

        private TimeSpan _timeOfNextWatering;
        public TimeSpan TimeOfNextWatering 
        { 
            get 
            {
                return _timeOfNextWatering;
            } 
            set
            {
                _timeOfNextWatering = value;
                DateOfNextWatering = DateOfNextWatering.Date.Add(value);
            } 
        }
        public double ExtraWaterTime { get; set; } = 0;
        public DateTime DateOfLastMisting { get; set; }
        private TimeSpan _timeOfLastMisting;
        public TimeSpan TimeOfLastMisting
        {
            get
            {
                return _timeOfLastMisting;
            }
            set
            {
                _timeOfLastMisting = value;
                DateOfLastMisting = DateOfLastMisting.Date.Add(value);
            }
        }
        public DateTime DateOfNextMisting { get; set; }
        private TimeSpan _timeOfNextMisting;
        public TimeSpan TimeOfNextMisting
        {
            get
            {
                return _timeOfNextMisting;
            }
            set
            {
                _timeOfNextMisting = value;
                DateOfNextMisting = DateOfNextMisting.Date.Add(value);
            }
        }
        public double ExtraMistTime { get; set; } = 0;
        public DateTime DateOfLastMove { get; set; }
        private TimeSpan _timeOfLastMove;
        public TimeSpan TimeOfLastMove
        {
            get
            {
                return _timeOfLastMove;
            }
            set
            {
                _timeOfLastMove = value;
                DateOfLastMove = DateOfLastMove.Date.Add(value);
            }
        }
        public DateTime DateOfNextMove { get; set; }
        private TimeSpan _timeOfNextMove;
        public TimeSpan TimeOfNextMove
        {
            get
            {
                return _timeOfNextMove;
            }
            set
            {
                _timeOfNextMove = value;
                DateOfNextMove = DateOfNextMove.Date.Add(value);
            }
        }
        public double ExtraMoveTime { get; set; } = 0;
        [Range(0, 365)]
        public int? WaterInterval { get; set; }
        [Range(0, 365)]
        public int? MistInterval { get; set; }
        [Range(0, 365)]
        public int? SunInterval { get; set; }
        public string ImageLocation { get; set; }
        
        private double _waterPercent;
        public double WaterPercent
        {
            get { return _waterPercent;  }
            set 
            { 
                _waterPercent = TimeToNextAction(DateOfLastWatering, DateOfNextWatering);
                WaterRect = new Rect(0, 40, Width, Height);
                //WaterRect = new Rect(0, WaterPercent * Height, Width, Height);
                OnPropertyChanged(nameof(WaterRect));
            }
        }

        private double _mistPercent;
        public double MistPercent
        {
            get { return _mistPercent; }
            set 
            {
                _mistPercent = TimeToNextAction(DateOfLastMisting, DateOfNextMisting);
                MistRect = new Rect(0, 60, Width, Height);
                //MistRect = new Rect(0, MistPercent * Height, Width, Height);
                OnPropertyChanged(nameof(MistRect));
            }
        }

        private double _sunPercent;
        public double SunPercent
        {
            get { return _sunPercent; }
            set 
            {
                _sunPercent = TimeToNextAction(DateOfLastMove, DateOfNextMove);
                SunRect = new Rect(0, 80, Width, Height);
                //SunRect = new Rect(0, SunPercent * Height, Width, Height);
                OnPropertyChanged(nameof(SunRect));
            }
        }

        private double TimeToNextAction(DateTime lastTime, DateTime nextTime)
        {
            if (nextTime > DateTime.Now && lastTime < DateTime.Now)
            {
                return (DateTime.Now - lastTime).TotalSeconds /
                (nextTime - lastTime).TotalSeconds;
            } else
            {
                return (nextTime < DateTime.Now && nextTime != lastTime) ? 1 : 0;
            }
            
        }

        private double _waterOpacity;
        private double _mistOpacity;
        private double _moveOpacity;
        [Ignore]
        public double WaterOpacity 
        { 
            get { return _waterOpacity; }
            set
            {
                _waterOpacity = value;
            } 
        }
        [Ignore]
        public double MistOpacity
        {
            get { return _mistOpacity; }
            set
            {
                _mistOpacity = value;
            }
        }

        [Ignore]
        public double MoveOpacity
        {
            get { return _moveOpacity; }
            set
            {
                _moveOpacity = value;
            }
        }





        [Ignore]
        public double Width { get; set; } = 105;
        [Ignore]
        public double Height { get; set; } = 105;

        private Rect _waterRect;
        [Ignore]
        public Rect WaterRect 
        { 
            get { return _waterRect; } 
            set { _waterRect = value;
                OnPropertyChanged(nameof(WaterRect));
            }
        }

        private Rect _mistRect;
        [Ignore]
        public Rect MistRect 
        { 
            get { return _mistRect; } 
            set { _mistRect = value;
                OnPropertyChanged(nameof(MistRect));
            } 
        }
        
        private Rect _sunRect;
        [Ignore]
        public Rect SunRect 
        { 
            get { return _sunRect; }
            set { _sunRect = value;
                OnPropertyChanged(nameof(SunRect));
            }
        }
    }
}
