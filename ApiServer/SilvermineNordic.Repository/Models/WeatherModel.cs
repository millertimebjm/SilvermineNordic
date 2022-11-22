using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilvermineNordic.Repository.Models
{
    public class WeatherModel
    {
        public DateTime DateTimeUtc { get; set; }
        public decimal TemperatureInCelcius { get; set; }
        public decimal Humidity { get; set; }
    }
}
