using Device.Models;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Device
{
    class Program
    {
        private static DeviceClient deviceClient = DeviceClient.CreateFromConnectionString("HostName=WIN20-iothub.azure-devices.net;DeviceId=DeviceApp;SharedAccessKey=Qtj9zuTHh1aF95Fa+4LY/5k7rTwXJiysCvGVGbnulL4=", TransportType.Mqtt);

        private static string _connweather = "http://api.weatherstack.com/current?access_key=e699857f79960f12fc50911e6203d374&query=koping";
        private static HttpClient _client = new HttpClient();
        private static HttpResponseMessage response = new HttpResponseMessage();
        private static int remoteInterval = 5;

        static void Main(string[] args)
        {
            //Service.InvokeMethod("DeviceApp", "RemoteMessageInterval", "10").GetAwaiter();
            deviceClient.SetMethodHandlerAsync("RemoteMessageInterval", RemoteMessageInterval, null);

            SendMessageAsync().GetAwaiter();

            Console.ReadKey();
        }

        public static async Task SendMessageAsync()
        {
            while (true)
            {
                try
                {
                    response = await _client.GetAsync(_connweather);

                    if (response.IsSuccessStatusCode)
                    {
                        WeatherData weather = JsonConvert.DeserializeObject<WeatherData>(await response.Content.ReadAsStringAsync());
                        var data = new Current
                        {
                            temperature = weather.current.temperature,
                            humidity = weather.current.humidity
                        };

                        var json = JsonConvert.SerializeObject(data);

                        var payload = new Message(Encoding.UTF8.GetBytes(json));

                        await deviceClient.SendEventAsync(payload);
                        Console.WriteLine($"Message sent: {json}");
                        Console.WriteLine("-----------------------------------------------");
                        await Task.Delay(remoteInterval * 1000);
                    }
                }
                catch (Exception)
                { throw; }

            }
        }

        public static Task<MethodResponse> RemoteMessageInterval(MethodRequest request, object userContext)
        {


            var payload = Encoding.UTF8.GetString(request.Data).Replace("\"", "");

            if (Int32.TryParse(payload, out remoteInterval))
            {
                Console.WriteLine($"Interval set remotely to: {remoteInterval} seconds.");

                var json = "{\"result\": \"The direct method: " + request.Name + ", was executed properly\"}";
                return Task.FromResult(new MethodResponse(Encoding.UTF8.GetBytes(json), 200));
            }
            else
            {
                var json = "{\"result\": \"Method not implemented\"}";
                return Task.FromResult(new MethodResponse(Encoding.UTF8.GetBytes(json), 501));

            }



        }
    }
}
