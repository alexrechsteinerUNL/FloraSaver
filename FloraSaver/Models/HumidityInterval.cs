using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloraSaver.Models
{
    public class HumidityInterval
    {
        public string HumidityRange { get; private set; }
        
        private int _humidityLevel;
        public int HumidityLevel {
            get { return _humidityLevel; }
            set
            {
                _humidityLevel = value;
                switch (value) 
                {
                    case 85:
                        HumidityRange = "85%-100%";
                        break;
                    case 60:
                        HumidityRange = "60%-84%";
                        break;
                    case 45:
                        HumidityRange = "45%-59%";
                        break;
                    case 30:
                        HumidityRange = "30%-44%";
                        break;
                    case 0:
                        HumidityRange = "0%-29%";
                        break;
                    default:
                        HumidityRange = "30%-44%";
                        break;
                }
            }
        }
        public string IntervalText { get; set; }
    }
}
