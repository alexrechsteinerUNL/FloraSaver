﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using FloraSaver.Services;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Storage;
using Microsoft.VisualBasic;
using SQLite;

namespace FloraSaver.Models
{
    [Table("plant")]
    public class Plant : ObservableObject, IPlant
    {
        public Plant() { }

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

        public bool HasImage => !string.IsNullOrEmpty(ImageLocation);

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
                }
                else
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
        private double? _waterInterval;
        public double? WaterInterval { get => _waterInterval; set 
            {
                _waterInterval = value;
                //if (value != null)
                //{
                //    BaseWaterIntervalForTempAndHum = FindBase((double)value);
                //}

            } 
        }

        [Range(0, 365)]
        private double? _mistInterval;
        public double? MistInterval { get => _mistInterval; set 
            { 
                _mistInterval = value;
            } 
        }

        [Range(0, 365)]
        public double? SunInterval { get; set; }

        [Range(0, 365)]
        public double? BaseWaterIntervalForTempAndHum { get; set; }

        [Range(0, 365)]
        public double? BaseMistIntervalForTempAndHum { get; set; }

        public double FindBase(double currentInterval)
        {
            if (currentInterval > 2 && currentInterval <= 28)
            {
                if (HumanityIntervalInt < 100 || TemperatureIntervalInt < 100)
                {
                    currentInterval = (2.0 * (((double)HumanityIntervalInt / 100.0) + 1.0) * (currentInterval + ((double)TemperatureIntervalInt / 100.0))) / (1.0 + 2.0 * ((double)HumanityIntervalInt / 100.0));
                }   
            }
            return currentInterval > 1 ? Math.Ceiling(currentInterval) : 1;
        }

        public double FindCurrent(double baseInterval)
        {
            if (baseInterval > 2 && baseInterval <= 28)
            {
                if (HumanityIntervalInt < 100 || TemperatureIntervalInt < 100)
                {
                    baseInterval = baseInterval - (0.5 * (baseInterval / (1.0 + ((double)HumanityIntervalInt / 100.0))) - ((double)TemperatureIntervalInt / 100.0));
                }
            }

            return baseInterval > 1 ? Math.Floor(baseInterval) : 1;
        }

        [Ignore]
        public HumidityInterval Humidity => new() { HumidityLevel = HumanityIntervalInt ?? 0 };
        [Ignore]
        public TemperatureInterval TemperatureF => new() { TemperatureLevel = TemperatureIntervalInt ?? 0 };
        [Ignore]
        public TemperatureInterval TemperatureC => new() { IsCelsius = true, TemperatureLevel = TemperatureIntervalInt ?? 0 };

        public int? HumanityIntervalInt { get; set; }

        public int? TemperatureIntervalInt { get; set; }



        private string _imageLocation;

        public string ImageLocation
        {
            get { return _imageLocation; }
            set
            {
                _imageLocation = value;
            }
        }

        public DateTime PlantWaterOverdueCooldownLastWarned { get; set; }
        public DateTime PlantMistOverdueCooldownLastWarned { get; set; }
        public DateTime PlantMoveOverdueCooldownLastWarned { get; set; }

        private double _waterPercent;

        public double WaterPercent
        {
            get { return _waterPercent; }
            set
            {
                _waterPercent = TimeToNextAction(DateOfLastWatering, DateOfNextWatering);
                _waterPercent = UseWatering ? _waterPercent : 0;
                WaterRect = new Rect(0, 40, Width, Height);
                //WaterRect = new Rect(0, WaterPercent * Height, Width, Height);
                OnPropertyChanged(nameof(WaterPercent));
            }
        }

        private double _mistPercent;

        public double MistPercent
        {
            get { return _mistPercent; }
            set
            {
                _mistPercent = TimeToNextAction(DateOfLastMisting, DateOfNextMisting);
                _mistPercent = UseMisting ? _mistPercent : 0;
                MistRect = new Rect(0, 60, Width, Height);
                //MistRect = new Rect(0, MistPercent * Height, Width, Height);
                OnPropertyChanged(nameof(MistPercent));
            }
        }

        private double _sunPercent;

        public double SunPercent
        {
            get { return _sunPercent; }
            set
            {
                _sunPercent = TimeToNextAction(DateOfLastMove, DateOfNextMove);
                _sunPercent = UseMoving ? _sunPercent : 0;
                SunRect = new Rect(0, 80, Width, Height);
                //SunRect = new Rect(0, SunPercent * Height, Width, Height);
                OnPropertyChanged(nameof(SunPercent));
            }
        }
        
        private double TimeToNextAction(DateTime lastTime, DateTime nextTime)
        {
            if (nextTime > DateTime.Now && lastTime < DateTime.Now)
            {
                return Math.Floor(((DateTime.Now - lastTime).TotalSeconds /
                (nextTime - lastTime).TotalSeconds)*100)/100;
            }
            else
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

        private bool _isEnabled;

        [Ignore]
        public bool IsEnabled
        {
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
        public string WaterIntervalText => WaterInterval == 1.0 ? "Every Day" : $"Every {WaterInterval} Days";
        [Ignore]
        public string MistIntervalText => MistInterval == 1.0 ? "Every Day" : $"Every {MistInterval} Days";
        [Ignore]
        public string SunIntervalText => SunInterval == 1.0 ? "Every Day" : $"Every {SunInterval} Days";
        
        [Ignore]
        public ImageSource PlantImageSource { get; set; }

        [Ignore]
        public Color SelectedColor => IsEnabled ? Color.FromArgb("#e1ad01") : Color.FromArgb("#000000");

        [Ignore]
        public Color SelectedTextColor => IsEnabled ? Color.FromArgb("#000000") : Color.FromArgb("#FFFFFF");

        [Ignore]
        public double Width { get; set; } = 105;

        [Ignore]
        public double Height { get; set; } = 105;

        private Rect _waterRect;

        [Ignore]
        public Rect WaterRect
        {
            get { return _waterRect; }
            set
            {
                _waterRect = value;
                OnPropertyChanged(nameof(WaterRect));
            }
        }

        private Rect _mistRect;

        [Ignore]
        public Rect MistRect
        {
            get { return _mistRect; }
            set
            {
                _mistRect = value;
                OnPropertyChanged(nameof(MistRect));
            }
        }

        private Rect _sunRect;

        [Ignore]
        public Rect SunRect
        {
            get { return _sunRect; }
            set
            {
                _sunRect = value;
                OnPropertyChanged(nameof(SunRect));
            }
        }

        [Ignore]
        public PlantValidationArgs Validation { get; set; } = new PlantValidationArgs();
        [Ignore]
        public bool IsPlantExisting { get; set; } = false;
        public void Validate(List<string> unsafePlantNames)
        {
            Validation.Validate(this, unsafePlantNames);
        }

        [Ignore]
        public string Source { get; set; } = "";
        [Ignore]
        public bool IsPlantSource => !string.IsNullOrEmpty(Source);

        public Plant(Plant _Plant)
        {

            DateOfBirth = _Plant.DateOfBirth;
            DateOfLastMisting = _Plant.DateOfLastMisting;
            DateOfLastMove = _Plant.DateOfLastMove;
            DateOfLastWatering = _Plant.DateOfLastWatering;
            DateOfNextMisting = _Plant.DateOfNextMisting;
            DateOfNextMove = _Plant.DateOfNextMove;
            DateOfNextWatering = _Plant.DateOfNextWatering;
            GivenName = _Plant.GivenName;
            ImageLocation = _Plant.ImageLocation;
            MistInterval = _Plant.MistInterval;
            PlantSpecies = _Plant.PlantSpecies;
            SunInterval = _Plant.SunInterval;
            TimeOfLastMisting = _Plant.TimeOfLastMisting;
            TimeOfLastMove = _Plant.TimeOfLastMove;
            TimeOfLastWatering = _Plant.TimeOfLastWatering;
            TimeOfNextMisting = _Plant.TimeOfNextMisting;
            TimeOfNextMove = _Plant.TimeOfNextMove;
            TimeOfNextWatering = _Plant.TimeOfNextWatering;
            WaterInterval = _Plant.WaterInterval;
            PlantGroupName = _Plant.PlantGroupName;
            GroupColorHexString = _Plant.GroupColorHexString;
            HumanityIntervalInt = _Plant.HumanityIntervalInt;
            TemperatureIntervalInt = _Plant.TemperatureIntervalInt;
            Source = _Plant.Source;
            Validation = _Plant.Validation;
        }

        public Plant(SearchedPlants _SearchedPlants)
        {
            DateOfLastMisting = DateTime.Now.Date;
            DateOfLastMove = DateTime.Now.Date;
            DateOfLastWatering = DateTime.Now.Date;
            MistInterval = _SearchedPlants.MistInterval;
            PlantSpecies = _SearchedPlants.PlantSpecies;
            SunInterval = _SearchedPlants.SunInterval;
            TimeOfLastMisting = DateTime.Now.TimeOfDay;
            TimeOfLastMove = DateTime.Now.TimeOfDay;
            TimeOfLastWatering = DateTime.Now.TimeOfDay;
            DateOfBirth = DateTime.Now;
            WaterInterval = _SearchedPlants.WaterInterval;
            Source = _SearchedPlants.Source;
            IsPlantExisting = _SearchedPlants.IsPlantExisting;
            

            if (WaterInterval != null)
            {
                if (_SearchedPlants.IsPlantExisting)
                {
                    BaseWaterIntervalForTempAndHum = FindBase((double)WaterInterval);
                }
                DateOfNextWatering = DateOfLastWatering.AddDays((double)WaterInterval);
                TimeOfNextWatering = DateOfNextWatering.TimeOfDay;
            } 
            else
            {
                DateOfNextWatering = DateOfLastWatering;
                TimeOfNextWatering = TimeOfLastWatering;
            }

            if (MistInterval != null)
            {
                if (_SearchedPlants.IsPlantExisting)
                {
                    BaseMistIntervalForTempAndHum = FindBase((double)MistInterval);
                }
                DateOfNextMisting = DateOfLastMisting.AddDays((double)MistInterval);
                TimeOfNextMisting = DateOfNextMisting.TimeOfDay;
            }
            else
            {
                DateOfNextMisting = DateOfLastMisting;
                TimeOfNextMisting = TimeOfLastMisting;
            }

            if (SunInterval != null)
            {
                DateOfNextMove = DateOfLastMove.AddDays((double)SunInterval);
                TimeOfNextMove = DateOfNextMove.TimeOfDay;
            }
            else
            {
                DateOfNextMove = DateOfLastMove;
                TimeOfNextMove = TimeOfLastMove;
            }


            PlantGroupName = _SearchedPlants.PlantGroupName;
            GroupColorHexString = _SearchedPlants.GroupColorHexString;
            UseWatering = true;
            UseMisting = true;
            UseMoving = false;
        }

        public Plant(IPlant _Plant)
        {
            MistInterval = _Plant.MistInterval;
            PlantSpecies = _Plant.PlantSpecies;
            SunInterval = _Plant.SunInterval;
            WaterInterval = _Plant.WaterInterval;
        }
    }
}