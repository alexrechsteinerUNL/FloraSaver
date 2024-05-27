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
                    case 100:
                        HumidityRange = "85%-100%";
                        break;
                    case 84:
                        HumidityRange = "60%-84%";
                        break;
                    case 59:
                        HumidityRange = "45%-59%";
                        break;
                    case 44:
                        HumidityRange = "30%-44%";
                        break;
                    case 29:
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
