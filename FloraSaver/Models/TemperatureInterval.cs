using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloraSaver.Models
{
    public class TemperatureInterval
    {
        public string FCIndicator => IsCelsius ? "°C" : "°F";
        public bool IsCelsius { get; set; } = false;
        public string TemperatureRange { get; private set; } = "60°-79°";
        
        private int _temperatureLevel;
        public int TemperatureLevel {
            get { return (int)_temperatureLevel; }
            set
            {
                _temperatureLevel = value;
                var lowBound = 0;
                var highBound = 0;
                switch (value) 
                { 
                    
                    case 100:
                        lowBound = IsCelsius ? ConvertToCelsius(value) : value;
                        TemperatureRange = $"{lowBound}°+";
                        break;
                    case 80:
                        lowBound = IsCelsius ? ConvertToCelsius(value) : value; 
                        highBound = IsCelsius ? ConvertToCelsius(99) : 99;
                        TemperatureRange = $"{lowBound}°-{highBound}°";
                        break;
                    case 60:
                        lowBound = IsCelsius ? ConvertToCelsius(value) : value;
                        highBound = IsCelsius ? ConvertToCelsius(79) : 79;
                        TemperatureRange = $"{lowBound}°-{highBound}°";
                        break;
                    case 40:
                        lowBound = IsCelsius ? ConvertToCelsius(value) : value;
                        highBound = IsCelsius ? ConvertToCelsius(59) : 59;
                        TemperatureRange = $"{lowBound}°-{highBound}°";
                        break;
                    case 20:
                        lowBound = IsCelsius ? ConvertToCelsius(value) : value;
                        highBound = IsCelsius ? ConvertToCelsius(39) : 39;
                        TemperatureRange = $"{lowBound}°-{highBound}°";
                        break;
                    case 0:
                        lowBound = IsCelsius ? ConvertToCelsius(value) : value;
                        highBound = IsCelsius ? ConvertToCelsius(19) : 19;
                        TemperatureRange = $"{lowBound}°-{highBound}°";
                        break;
                    default:
                        lowBound = IsCelsius ? ConvertToCelsius(60) : 60;
                        highBound = IsCelsius ? ConvertToCelsius(79) : 79;
                        TemperatureRange = $"{lowBound}°-{highBound}°";
                        break;
                }
            }
        }
        public string IntervalText { get; set; }

        public static int ConvertToCelsius(double temperature)
        {
            return (int) Math.Floor((temperature - 32) * (5f / 9f));
        }
    }
}
