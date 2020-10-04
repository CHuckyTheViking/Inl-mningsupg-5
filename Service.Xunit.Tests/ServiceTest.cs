using Microsoft.Azure.Devices;
using Newtonsoft.Json;
using System;
using System.Text;
using Xunit;

namespace Service.Xunit.Tests
{
    public class ServiceTest
    {
        private readonly ServiceClient serviceClient = ServiceClient.CreateFromConnectionString("HostName=WIN20-iothub.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=VpiWQr5A3vMxsO2IaH5/rwcw49EPCqEHJhPZFOWZtlw=");
        private CloudToDeviceMethod methodCaller;



        [Theory]
        [InlineData("Method", "Method")]
        [InlineData("Device23", "Device23")]
        [InlineData("Device23", "Method")] // Failar med flit för att se så den fungerar korrekt.
        public void Should_SetMethodName(string methodName, string expected)
        {
            methodCaller = new CloudToDeviceMethod(methodName);

            Assert.Equal(methodCaller.MethodName, expected);
        }


        [Theory]
        [InlineData("418", "DeviceApp", "RemoteMessageInterval", "0")]
        [InlineData("200", "DeviceApp", "RemoteMessageInterval", "10")]        
        [InlineData("501", "DeviceApp", "RemoteMessage", "10")]
        [InlineData("501", "DeviceProgram", "RemoteMessageInterval", "10")] // Failar med flit för att se så den fungerar korrekt.

        public void Should_ConnectToServiceClient(string expected, string deviceId, string methodName, string payload)
        {

            var methodCaller = new CloudToDeviceMethod(methodName);
            methodCaller.SetPayloadJson(payload);
            var response = serviceClient.InvokeDeviceMethodAsync(deviceId, methodCaller);

            Assert.Equal(expected, response.Result.Status.ToString());

        }

    }
}
