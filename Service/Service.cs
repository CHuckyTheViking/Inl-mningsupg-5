using Microsoft.Azure.Devices;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Service
{
    class Service
    {
        private static readonly ServiceClient serviceClient = ServiceClient.CreateFromConnectionString("HostName=WIN20-iothub.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=VpiWQr5A3vMxsO2IaH5/rwcw49EPCqEHJhPZFOWZtlw=");
        static void Main(string[] args)
        {
            while (true)
            {                
                Console.Write("DeviceName: ");
                string deviceName = Console.ReadLine();
                Console.Write("Method to call: ");
                string deviceMethod = Console.ReadLine();
                Console.Write("How many seconds?: ");
                string secInterval = Console.ReadLine();
                CallMethod(deviceName, deviceMethod, secInterval).GetAwaiter();
                Thread.Sleep(1000);
            }
        }


        public static async Task CallMethod(string deviceId, string methodName, string payload)
        {
            var methodCaller = new CloudToDeviceMethod(methodName) { ResponseTimeout = TimeSpan.FromSeconds(30) };
            methodCaller.SetPayloadJson(payload);

            var response = await serviceClient.InvokeDeviceMethodAsync(deviceId, methodCaller);

            Console.WriteLine($"Response Status: {response.Status}");
            Console.WriteLine(response.GetPayloadAsJson());
        }
    }
}
