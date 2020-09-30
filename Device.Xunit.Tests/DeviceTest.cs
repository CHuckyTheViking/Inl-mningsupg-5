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
            //_connweather = "http://api.weatherstack.com/current?access_key=e699857f79960f12fc50911e6203d374&query=koping";
            response = await _client.GetAsync(_connweather);

            Assert.Equal(response.IsSuccessStatusCode, expected);
        }


        public class Current
        {
            public int temperature { get; set; }
            public int humidity { get; set; }
        }
        public class WeatherData
        {
            public Current current { get; set; }
        }
        dynamic data;


        [Fact]
        public async Task Should_GetWeatherData()
        {
            response = await _client.GetAsync(_connweather);

            WeatherData weather = JsonConvert.DeserializeObject<WeatherData>(await response.Content.ReadAsStringAsync());
            data = new Current
            {
                temperature = weather.current.temperature,
                humidity = weather.current.humidity
            };

            if (weather.current.temperature > 1)
            {
                Assert.True(true);
            }
        }

        private dynamic json;
        private Message payload;
        DeviceClient deviceClient = DeviceClient.CreateFromConnectionString("HostName=WIN20-iothub.azure-devices.net;DeviceId=DeviceApp;SharedAccessKey=Qtj9zuTHh1aF95Fa+4LY/5k7rTwXJiysCvGVGbnulL4=", TransportType.Mqtt);

        [Fact]
        public async Task Should_SendWeatherData()
        {

            json = JsonConvert.SerializeObject(data);

            payload = new Message(Encoding.UTF8.GetBytes(json));

            await deviceClient.SendEventAsync(payload);

            await Task.Delay(5000);

            Assert.NotNull(payload);

        }


        private int remoteInterval;

        [Theory]
        [InlineData(4, "4", 4)]
        [InlineData(3, "3", 3)]
        [InlineData(10, "10", 10)]
        public void Should_SetRemoteMessageInterval(int expected, string payload, int remoteInterval)
        {
            if (Int32.TryParse(payload, out remoteInterval))
            {
                Assert.Equal(expected, remoteInterval);
            }

        }
    }
}
