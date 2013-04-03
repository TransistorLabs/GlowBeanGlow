using System;
using System.Globalization;
using System.Linq;
using System.Timers;
using GlowBeanGlow.Api.DataTypes;
using HidLibrary;

namespace GlowBeanGlow.Api
{
	public class UsbDriver
	{
		private const int VendorId = 0x03EB;
		private const int ProductId = 0x204F;
		private HidDevice _device;
		private static Timer _deviceReadTimer;

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

		public Action<double, double> OnTempChange;

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
			var reportBytes = report.Data;

			byte tempHighByte = reportBytes[1];
			byte tempLowByte = reportBytes[2];
			byte buttons = reportBytes[0];

			bool isNegative = (tempHighByte & 0x80) > 0;


			byte tempWholeNumberDataC = 0x00;

			byte[] data = new byte[2];

			if (isNegative)
			{
				data[1] = 0xFF;
			}

			tempWholeNumberDataC = (byte)(tempHighByte << 1);
			tempWholeNumberDataC |= (byte)((tempLowByte & 0x80) >> 7);
			data[0] = tempWholeNumberDataC;

			var tempCIntPortion = BitConverter.ToInt16(data, 0);

			float tempC = tempCIntPortion;

			if ((tempLowByte & 0x08) > 0) { tempC += 0.0625F; }
			if ((tempLowByte & 0x10) > 0) { tempC += 0.125F; }
			if ((tempLowByte & 0x20) > 0) { tempC += 0.25F; }
			if ((tempLowByte & 0x40) > 0) { tempC += 0.5F; }


			//°C  x  9/5 + 32 = °F
			var tempF = tempC * 9 / 5 + 32;

			if (OnTempChange != null)
			{
				OnTempChange(tempC, tempF);
			}

			//bool button1Pressed = ((buttons & (byte)0x80) > 0);
			//bool button2Pressed = ((buttons & (byte)0x08) > 0);
			//bool button3Pressed = ((buttons & (byte)0x40) > 0);

			//this.Dispatcher.Invoke((Action)(() =>
			//{
			//	RawByteOutput.Text = sb.ToString();
			//	DegreeOutputC.Text = string.Format("{0:00.0000}° C", tempC);
			//	DegreeOutputF.Text = string.Format("{0:00.0}", tempF);

			//	Button1.Style = Resources["ButtonOffStyle"] as Style;
			//	Button2.Style = Resources["ButtonOffStyle"] as Style;
			//	Button3.Style = Resources["ButtonOffStyle"] as Style;

			//	if (button1Pressed) Button1.Style = Resources["ButtonOnStyle"] as Style;
			//	if (button2Pressed) Button2.Style = Resources["ButtonOnStyle"] as Style;
			//	if (button3Pressed) Button3.Style = Resources["ButtonOnStyle"] as Style;

			//}));
			_device.ReadReport(OnReport);
		}
	}
}
