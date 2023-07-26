using FloraSaver.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FloraSaver.Services
{
    public  class PickerService
    {
        public static List<Interval> GetWaterIntervals()
        {
            return new List<Interval>() 
            {
                new Interval() { DaysFromNow = 0, IntervalText = "None" },
                new Interval() { DaysFromNow = 1, IntervalText = "Every Day" },
                new Interval() { DaysFromNow = 2, IntervalText = "Every 2 Days" },
                new Interval() { DaysFromNow = 3, IntervalText = "Every 3 Days" },
                new Interval() { DaysFromNow = 7, IntervalText = "Every 7 Days" },
                new Interval() { DaysFromNow = 14, IntervalText = "Every 14 Days" },
                new Interval() { DaysFromNow = 28, IntervalText = "Every 28 Days" },
                new Interval() { DaysFromNow = -1, IntervalText = "Custom" }
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
                .Select(_ => new GroupColors { ColorName = _.Name, Colors = (Color)_.GetValue(null), ColorsHex = ((Color)_.GetValue(null)).ToArgbHex()});
            return colorFields.ToList();
        }
    }
}
