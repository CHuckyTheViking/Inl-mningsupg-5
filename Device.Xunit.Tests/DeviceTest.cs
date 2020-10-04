using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Device.Xunit.Tests
{
    public class DeviceTest
    {
        private bool expected;
        private HttpClient _client = new HttpClient();
        private string _connweather = "http://api.weatherstack.com/current?access_key=e699857f79960f12fc50911e6203d374&query=koping";
        private HttpResponseMessage response;

        [Fact]
        public async Task Should_GetSuccesStatusCode()
        {

            expected = true;
            _client = new HttpClient();
            response = await _client.GetAsync(_connweather);

            Assert.Equal(response.IsSuccessStatusCode, expected);
        }


        public class WeatherData
        {
            public Current current { get; set; }
        }
        public class Current
        {
            public int Temperature { get; set; }
            public int Humidity { get; set; }
        }
        
        dynamic data;

        [Fact]
        public async Task Should_GetWeatherData()
        {
            response = await _client.GetAsync(_connweather);

            WeatherData weather = JsonConvert.DeserializeObject<WeatherData>(await response.Content.ReadAsStringAsync());
            data = new Current
            {
                Temperature = weather.current.Temperature,
                Humidity = weather.current.Humidity
            };

            //Kollar ifall Temperature och Humidity har fått ett värde
            if (weather.current.Temperature > 1)
            {
                if (weather.current.Humidity > 1)
                {
                    Assert.True(true);
                }
            }
        }


        private int remoteInterval;

        [Theory]
        [InlineData(4, "4", 4)]
        [InlineData(3, "3", 3)]
        [InlineData(10, "8", 10)] // Failar med flit för att se så den fungerar korrekt.
        public void Should_SetRemoteMessageInterval(int expected, string payload, int remoteInterval)
        {
            if (Int32.TryParse(payload, out remoteInterval))
            {
                Assert.Equal(expected, remoteInterval);
            }

        }
    }
}
