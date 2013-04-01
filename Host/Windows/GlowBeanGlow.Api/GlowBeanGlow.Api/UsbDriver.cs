using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GlowBeanGlow.Api.DataTypes;
using HidLibrary;

namespace GlowBeanGlow.Api
{
	public class UsbDriver
	{
		private const int VendorId = 0x03EB;
		private const int ProductId = 0x204F;
		private HidDevice _device;
		private bool _attached = false;
		private bool _connectedToDriver = false;

		/// <summary>
		/// Occurs when a device is attached.
		/// </summary>
		public event EventHandler DeviceAttached;

		/// <summary>
		/// Occurs when a device is removed.
		/// </summary>
		public event EventHandler DeviceRemoved;

		/// <summary>
		/// 
		/// After a successful connection, a DeviceAttached event will normally be sent.
		/// </summary>
		/// <returns>True if a device is connected, False otherwise.</returns>
		public bool Connect()
		{
			_device = HidDevices.Enumerate(VendorId, ProductId).FirstOrDefault();

			if (_device != null)
			{
				_connectedToDriver = true;
				_device.OpenDevice();

				_device.Inserted += DeviceAttachedHandler;
				_device.Removed += DeviceRemovedHandler;

				_device.MonitorDeviceEvents = true;

				_device.ReadReport(OnReport);

				return true;
			}

			return false;
		}

		public void RenderFrame(SetFrameInstruction setFrame)
		{
			if (_connectedToDriver)
			{
				var report = new HidReport(9, new HidDeviceData(setFrame.GetByteArray(0), HidDeviceData.ReadStatus.NoDataRead));
				_device.WriteReport(report, Console.WriteLine);
			}
		}

		private void DeviceAttachedHandler()
		{
			_attached = true;

			if (DeviceAttached != null)
				DeviceAttached(this, EventArgs.Empty);

			_device.ReadReport(OnReport);
		}

		private void DeviceRemovedHandler()
		{
			_attached = false;

			if (DeviceRemoved != null)
				DeviceRemoved(this, EventArgs.Empty);
		}

		private void OnReport(HidReport report)
		{
			if (_attached == false) { return; }
		}
	}
}
