using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWS.View
{
    public class DayAndNightWeatherInfo
    {
        public string date { get; set; }
        public string week { get; set; }
        public string city { get; set; }
        public string imgSrc_day { get; set; }
        public string imgSrc_night { get; set; }
        public string weatherStatus_day { get; set; }
        public string weatherStatus_night { get; set; }
        public string wind_day { get; set; }
        public string wind_night { get; set; }
        public string windL_day { get; set; }
        public string windL_night { get; set; }
        public string maxTemperature { get; set; }
        public string minTemperature { get; set; }
    }
}
