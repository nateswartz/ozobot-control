using BluetoothBase.Utils;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace BluetoothBase.Bluetooth
{
    public class BluetoothKernel : IDisposable
    {
        private readonly ILogger _logger;
        private readonly IBluetoothAdapter _bluetoothAdapter;
        public ulong BluetoothAddress { get; set; }
        private IBluetoothDevice _device = null;
        private IBluetoothService _service = null;
        private IBluetoothCharacteristic _characteristic = null;

        private string _bluetoothServiceGuid;
        private string _bluetoothCharacteristicGuid;

        public BluetoothKernel(IBluetoothAdapter bluetoothAdapter, string bluetoothServiceGuid, string bluetoothCharacteristicGuid, ILogger<BluetoothKernel> logger = default)
        {
            _bluetoothAdapter = bluetoothAdapter ?? throw new ArgumentNullException(nameof(bluetoothAdapter));
            _bluetoothServiceGuid = bluetoothServiceGuid;
            _bluetoothCharacteristicGuid = bluetoothCharacteristicGuid;
            _logger = logger;
        }

        public bool IsConnected => _characteristic != null;

        public async Task ConnectAsync()
        {
            _device = await _bluetoothAdapter.GetDeviceAsync(BluetoothAddress);
            _service = await _device.GetServiceAsync(new Guid(_bluetoothServiceGuid));
            _characteristic = await _service.GetCharacteristicAsync(new Guid(_bluetoothCharacteristicGuid));

            _logger?.LogDebug("Connected");
        }


        public Task DisconnectAsync()
        {
            _characteristic = null;
            _service?.Dispose();
            _service = null;
            _device?.Dispose();
            _device = null;

            _logger?.LogDebug("Disconnected");

            return Task.CompletedTask;
        }

        public async Task SendBytesAsync(byte[] data)
        {
            if (_characteristic == null)
            {
                throw new InvalidOperationException($"Cannot invoke {nameof(SendBytesAsync)} when device is not connected ({nameof(_characteristic)} is null)");
            }

            var tmp = $"> {BytesStringUtil.DataToString(data)}";

            _logger?.LogDebug($"> {BytesStringUtil.DataToString(data)}");

            await _characteristic.WriteValueAsync(data);

        }

        public async Task ReceiveBytesAsync(Func<byte[], Task> handler)
        {
            if (_characteristic == null)
            {
                throw new InvalidOperationException($"Cannot invoke {nameof(SendBytesAsync)} when device is not connected ({nameof(_characteristic)} is null)");
            }

            await _characteristic.NotifyValueChangeAsync(async data =>
            {
                var tmp = $"> {BytesStringUtil.DataToString(data)}";

                _logger?.LogDebug($"< {BytesStringUtil.DataToString(data)}");

                await handler(data);
            });

            _logger?.LogDebug("Registered Receive Handler");
        }

        #region Dispose
        private bool disposedValue;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // managed resource
                }

                // unmananged
                _service?.Dispose();
                _device?.Dispose();
                _device = null;

                disposedValue = true;
            }
        }

        ~BluetoothKernel() => Dispose(disposing: false);
        public void Dispose() { Dispose(disposing: true); GC.SuppressFinalize(this); }
        #endregion
    }
}