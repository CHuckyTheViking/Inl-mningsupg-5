using System;
using System.Collections.Generic;
using System.Text;

namespace Device.Models
{
    public class WeatherData
    {
        public Current current { get; set; }
    }
    public class Current
    {
        public int Temperature { get; set; }
        public int Humidity { get; set; }
    }
}
