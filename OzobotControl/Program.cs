using BluetoothBase.Bluetooth;
using BluetoothBase.Utils;
using BluetoothWinRT;
using System;

namespace OzobotControl
{
    class Program
    {
        static void Main(string[] args)
        {
            var adapter = new WinRTBluetoothAdapter();

            adapter.Discover(async info =>
            {
                Console.WriteLine($"Discovered device: {info.Name}");
                if (!info.Name.StartsWith("Ozo"))
                    return;
                var kernel = new BluetoothKernel(adapter, BluetoothConstants.OzobotService, BluetoothConstants.OzobotControlCharacteristic, null);
                kernel.BluetoothAddress = info.BluetoothAddress;
                await kernel.ConnectAsync();
                while (!kernel.IsConnected)
                {
                    Console.WriteLine("Waiting...");
                }
                var command = "50-02-01";
                await kernel.SendBytesAsync(BytesStringUtil.StringToData(command));
                var command2 = "68-00-FF-64-00-14";
                await kernel.SendBytesAsync(BytesStringUtil.StringToData(command2));
                Console.WriteLine("Sent command");
            });
            Console.WriteLine("Press Enter to cancel");
            Console.ReadLine();

            Console.WriteLine("Exiting");
        }
    }
}
