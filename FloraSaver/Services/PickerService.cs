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
    }


}