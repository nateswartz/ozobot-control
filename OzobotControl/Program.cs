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
                var device = await adapter.GetDeviceAsync(info.BluetoothAddress);
                var service = await device.GetServiceAsync(new Guid(BluetoothConstants.OzobotService));
                var characteristic = await service.GetCharacteristicAsync(new Guid(BluetoothConstants.OzobotControlCharacteristic));
                var command = "50-02-01";
                await characteristic.WriteValueAsync(BytesStringUtil.StringToData(command));
                var command2 = "68-FF-00-64-64-14";
                await characteristic.WriteValueAsync(BytesStringUtil.StringToData(command2));
            });
            Console.WriteLine("Press Enter to cancel");
            Console.ReadLine();

            Console.WriteLine("Exiting");
        }
    }
}
