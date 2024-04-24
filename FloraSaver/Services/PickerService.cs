using FloraSaver.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FloraSaver.Services
{
    public class PickerService
    {
        public static List<Interval> GetWaterIntervals()
        {
            return new List<Interval>()
            {
                new Interval() { NumFromNow = 0, IntervalText = "None" },
                new Interval() { NumFromNow = 1, IntervalText = "Every Day" },
                new Interval() { NumFromNow = 2, IntervalText = "Every 2 Days" },
                new Interval() { NumFromNow = 3, IntervalText = "Every 3 Days" },
                new Interval() { NumFromNow = 7, IntervalText = "Every 7 Days" },
                new Interval() { NumFromNow = 14, IntervalText = "Every 14 Days" },
                new Interval() { NumFromNow = 28, IntervalText = "Every 28 Days" },
                new Interval() { NumFromNow = -1, IntervalText = "Custom" }
            };
        }

        public static List<string> GetOrderByValues()
        {
            return new List<string>()
            {
                "Next Action",
                "Next Watering",
                "Next Misting",
                "Next Moving",
                "Alphabetical"
            };
        }

        public static List<GroupColors> GetSelectableColors()
        {
            var colorType = typeof(Colors);
            var colorFields = colorType.GetFields()
                .Where(_ => _.FieldType == typeof(Color))
                .Select(_ => new GroupColors { ColorName = _.Name, Colors = (Color)_.GetValue(null), ColorsHex = ((Color)_.GetValue(null)).ToArgbHex() });
            return colorFields.ToList();
        }
        public static List<Interval> GetCooldownBeforePlantActionsOverdueNotification()
        {
            return new List<Interval>
            {
            new Interval() {IntervalText ="1 Hour", NumFromNow = 1},
            new Interval() {IntervalText ="3 Hours", NumFromNow = 3  },
            new Interval() {IntervalText ="6 Hours", NumFromNow = 6  },
            new Interval() {IntervalText = "12 Hours",NumFromNow = 12 },
            new Interval() {IntervalText ="24 Hours", NumFromNow = 24 },
            new Interval() {IntervalText = "Never", NumFromNow = -1 },
            };
        }

        public static List<Interval> GetInActionBeforeMultiOverdueNotification()
        {
            return new List<Interval>
            {
            new Interval(){IntervalText ="12 Hours",NumFromNow =  12 },
            new Interval(){IntervalText ="1 Day",NumFromNow =  24},
            new Interval(){IntervalText ="3 Days",NumFromNow =  72  },
            new Interval(){IntervalText ="6 Days",NumFromNow =  144  },
            new Interval(){IntervalText = "Never",NumFromNow =  -1 },
            };
        }

        public static List<HumidityInterval> GetHumidityPercent()
        {
            return new List<HumidityInterval>
            {
                new HumidityInterval(){IntervalText ="85%-100%: I live in the sauna", HumidityLevel =  85 },
                new HumidityInterval(){IntervalText ="60%-84%: My windows are crying",HumidityLevel =  60},
                new HumidityInterval(){IntervalText ="45%-59% The air feels like a hug",HumidityLevel =  45  },
                new HumidityInterval(){IntervalText ="30%-44% Normal Indoor Humidity",HumidityLevel =  30  },
                new HumidityInterval(){IntervalText = "0%-29% My lips chapped on impact",HumidityLevel =  0 },
            };
        }

        public static List<TemperatureInterval> GetTemperatureF()
        {
            return new List<TemperatureInterval>
            {
                new TemperatureInterval(){IntervalText = $"100°: Air conditioning is for the weak!", TemperatureLevel =  100 },
                new TemperatureInterval(){IntervalText = $"80°-99°: Thriving in my own lane",TemperatureLevel =  80},
                new TemperatureInterval(){IntervalText = $"60°-79° Normal Indoor Temperatures",TemperatureLevel =  60 },
                new TemperatureInterval(){IntervalText = $"40°-59° We got blankets. We're living good",TemperatureLevel =  40  },
                new TemperatureInterval(){IntervalText = $"20°-39° It's only a little frosty",TemperatureLevel =  20 },
                new TemperatureInterval(){IntervalText = $"0°-19° They call my room 'fridge space'",TemperatureLevel =  0 }
            };
        }

        public static List<TemperatureInterval> GetTemperatureC()
        {
            return new List<TemperatureInterval>
            {
                new TemperatureInterval(){IntervalText = $"{TemperatureInterval.ConvertToCelsius(100)}°+: Air conditioning is for the weak!", TemperatureLevel =  100, IsCelsius = true },
                new TemperatureInterval(){IntervalText = $"{TemperatureInterval.ConvertToCelsius(80)}°-{TemperatureInterval.ConvertToCelsius(99)}°: Thriving in my own lane",TemperatureLevel =  80, IsCelsius = true},
                new TemperatureInterval(){IntervalText = $"{TemperatureInterval.ConvertToCelsius(60)}°-{TemperatureInterval.ConvertToCelsius(79)}° Normal Indoor Temperatures",TemperatureLevel =  60, IsCelsius = true },
                new TemperatureInterval(){IntervalText = $"{TemperatureInterval.ConvertToCelsius(40)}°-{TemperatureInterval.ConvertToCelsius(59)}° We got blankets. We're living good",TemperatureLevel =  40, IsCelsius = true  },
                new TemperatureInterval(){IntervalText = $"{TemperatureInterval.ConvertToCelsius(20)}°-{TemperatureInterval.ConvertToCelsius(39)}° It's only a little frosty",TemperatureLevel =  20, IsCelsius = true },
                new TemperatureInterval(){IntervalText = $"{TemperatureInterval.ConvertToCelsius(0)}°-{TemperatureInterval.ConvertToCelsius(19)}° They call my room 'fridge space'",TemperatureLevel =  0, IsCelsius = true }
            };
        }
    }


}