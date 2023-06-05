using System;
using System.Collections.Generic;
using System.Text;

namespace WeaherForecastApp.Models
{
   [Serializable]
    public class GridData
    {
        public string TitleCity { get; set; }
        public string image { get; set; }
        public string TemperatureCity { get; set; }
        public string DescriptionWeather { get; set; }
        public GridData()
        {
            
        }
    }
}
