using BluetoothBase.Bluetooth;
using BluetoothBase.Utils;
using BluetoothWinRT;
using System;
using System.Threading.Tasks;

namespace OzobotControl
{
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

    class Program
    {
        static BluetoothKernel _kernel = null;
        static WinRTBluetoothAdapter _adapter = null;

        static async Task Main(string[] args)
        {
            _adapter = new WinRTBluetoothAdapter();

            _adapter.Discover(HandleDiscover);

            while (_kernel == null)
            {
                await Task.Delay(10);
            }

            await _kernel.ConnectAsync();
            Console.WriteLine("Connecting...");
            while (!_kernel.IsConnected)
            {
                await Task.Delay(10);
            }
            var stopCommand = "50-02-01";
            await _kernel.SendBytesAsync(BytesStringUtil.StringToData(stopCommand));

            while (true)
            {
                Console.WriteLine("Enter a command (i.e. 44-FF-FF-37-35-FF):");
                var command = Console.ReadLine();
                if (string.IsNullOrEmpty(command))
                    break;
                await _kernel.SendBytesAsync(BytesStringUtil.StringToData(command));
            }

            Console.WriteLine("Exiting");
        }

        static async Task HandleDiscover(BluetoothDeviceInfo info)
        {
            Console.WriteLine($"Discovered device: {info.Name}");
            if (!info.Name.StartsWith("Ozo") || _kernel != null)
                return;
            _kernel = new BluetoothKernel(_adapter, BluetoothConstants.OzobotService, BluetoothConstants.OzobotControlCharacteristic, null)
            {
                BluetoothAddress = info.BluetoothAddress
            };
            await Task.CompletedTask;
        }
    }
}
