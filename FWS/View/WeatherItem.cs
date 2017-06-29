using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWS.View
{
    public class WeatherItem
    {
        public int areaID { get; set; }
        public string date { get; set; }
        public string imageSrc { get; set; }
        public string weatherStatus { get; set; }
        public string maxTemperature { get; set; }
        public string minTemperature { get; set; }
    }
}
