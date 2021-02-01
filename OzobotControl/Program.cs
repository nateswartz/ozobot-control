using BluetoothBase.Bluetooth;
using BluetoothBase.Utils;
using BluetoothWinRT;
using System;
using System.Threading.Tasks;

namespace OzobotControl
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var adapter = new WinRTBluetoothAdapter();
            BluetoothKernel kernel = null;
            adapter.Discover(async info =>
            {
                Console.WriteLine($"Discovered device: {info.Name}");
                if (!info.Name.StartsWith("Ozo") || kernel != null)
                    return;
                kernel = new BluetoothKernel(adapter, BluetoothConstants.OzobotService, BluetoothConstants.OzobotControlCharacteristic, null)
                {
                    BluetoothAddress = info.BluetoothAddress
                };
                await Task.CompletedTask;
            });

            while (kernel == null)
            {
                await Task.Delay(10);
            }

            await kernel.ConnectAsync();
            Console.WriteLine("Connecting...");
            while (!kernel.IsConnected)
            {
                await Task.Delay(10);
            }
            var stopCommand = "50-02-01";
            await kernel.SendBytesAsync(BytesStringUtil.StringToData(stopCommand));

            Console.WriteLine("Enter a command (i.e. 44-FF-FF-37-35-FF):");
            var command = Console.ReadLine();
            await kernel.SendBytesAsync(BytesStringUtil.StringToData(command));

            //// Control all lights
            //var lightOff = "44-FF-FF-00-00-00";
            //var lightOnFull = "44-FF-FF-37-35-FF";
            //var lightRed = "44-FF-FF-37-00-00";

            //// Control 3 big lights
            ////var somethingElse = "44-2b-00-00-FF-FF";
            //var something = "44-2b-00-37-00-00";

            //// Control 2 smaller lights
            ////var somethingElse = "44-15-00-00-FF-FF";

            //// Control just the top light
            ////var somethingElse = "44-01-07-FF-60-00";

            //// Control everything except the top light
            //var somethingElse = "44-3E-07-FF-60-00";

            //await kernel.SendBytesAsync(BytesStringUtil.StringToData(lightRed));
            //await Task.Delay(1000);
            //await kernel.SendBytesAsync(BytesStringUtil.StringToData(lightOnFull));
            //await Task.Delay(1000);
            //await kernel.SendBytesAsync(BytesStringUtil.StringToData(lightOff));
            //await Task.Delay(2000);
            //await kernel.SendBytesAsync(BytesStringUtil.StringToData(somethingElse));
            //await Task.Delay(2000);
            //await kernel.SendBytesAsync(BytesStringUtil.StringToData(lightOff));
            //await Task.Delay(2000);
            //await kernel.SendBytesAsync(BytesStringUtil.StringToData(something));
            //Console.WriteLine("Sent command");
            //Console.WriteLine("Press Enter to cancel");
            //Console.ReadLine();

            Console.WriteLine("Exiting");
        }
    }
}
