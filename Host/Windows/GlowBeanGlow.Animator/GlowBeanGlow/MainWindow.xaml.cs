using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using LibUsbDotNet;
using LibUsbDotNet.DeviceNotify;
using LibUsbDotNet.Main;
using System.Linq;

namespace GlowBeanGlow
{
	public enum Command
	{
		EchoResponse = 0,
		GetWriteBufferLength = 1,
		ChangeRed0 = 2,
		ChangeGreen0 = 3,
		ChangeBlue0 = 4,
		ChangeRed1 = 5,
		ChangeGreen1 = 6,
		ChangeBlue1 = 7,
		ChangeLedFlags = 8,
		SetFrameData = 9,
		SaveAnimation = 10
	}

	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private readonly DispatcherTimer _dispatcherTimer = new DispatcherTimer();
		private int _writeBufferSize = 0;
		private const int HeaderBytes = 8;
		private const int FrameSize = 8;
		private int _maxFrames;

		public static IDeviceNotifier UsbDeviceNotifier = DeviceNotifier.OpenDeviceNotifier();
		public static DateTime LastDataEventDate = DateTime.Now;
		public static UsbDevice MyUsbDevice;

		public WindowFrame CurrentFrame = new WindowFrame();
		public WindowFrame StaticFrame = new WindowFrame();
		private readonly IList<WindowFrame> _frames = new List<WindowFrame>();

		public static UsbDeviceFinder MyUsbFinder = new UsbDeviceFinder(5824, 1500);

		private void Log(string message)
		{
			Status.Text += message + "\n";
			Status.ScrollToEnd();
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			if (!ExitDevice()) e.Cancel = true;

			base.OnClosing(e);
		}

		private bool ExitDevice()
		{
			bool status = true;
			try
			{
				if (MyUsbDevice != null)
				{
					if (MyUsbDevice.IsOpen)
					{
						// If this is a "whole" usb device (libusb-win32, linux libusb-1.0)
						// it exposes an IUsbDevice interface. If not (WinUSB) the 
						// 'wholeUsbDevice' variable will be null indicating this is 
						// an interface of a device; it does not require or support 
						// configuration and interface selection.
						var wholeUsbDevice = MyUsbDevice as IUsbDevice;
						if (!ReferenceEquals(wholeUsbDevice, null))
						{
							// Release interface #0.
							wholeUsbDevice.ReleaseInterface(0);
						}
						MyUsbDevice.Close();
					}
				}
				MyUsbDevice = null;
			}

			catch (Exception exception)
			{
				status = false;
				Log("An error occurred: " + exception.Message);
			}
			finally
			{
				// Free usb resources
				UsbDevice.Exit();
				Log("Device exit complete.");
			}
			return status;
		}

		public MainWindow()
		{
			InitializeComponent();
			ErrorCode ec = ErrorCode.None;

			AppWindow.DataContext = CurrentFrame;
			_dispatcherTimer.Tick += new EventHandler(DispatcherTimerTick);
			_dispatcherTimer.Interval = new TimeSpan(0,0,0,0,40);

			UsbDeviceNotifier.OnDeviceNotify += new EventHandler<DeviceNotifyEventArgs>(UsbDeviceNotifier_OnDeviceNotify);
			EnterDevice();


		}

		private void EnterDevice()
		{
			try
			{
				// Find and open the usb device.
				MyUsbDevice = UsbDevice.OpenUsbDevice(MyUsbFinder);

				// If the device is open and ready
				if (MyUsbDevice == null) Log("Device not found...");

				if (MyUsbDevice.Info.ProductString != "Glow Bean Glow!" || MyUsbDevice.Info.ManufacturerString != "TheThumpNetwork.com")
				{
					//Not the correct device.
					Log("GlowBeanGlow device not found.");
					ExitDevice();
				}

				var wholeUsbDevice = MyUsbDevice as IUsbDevice;
				if (!ReferenceEquals(wholeUsbDevice, null))
				{
					wholeUsbDevice.SetConfiguration(1);
					wholeUsbDevice.ClaimInterface(0);
				}

				if (MyUsbDevice.IsOpen)
				{
					Log(MyUsbDevice.Info.ProductString + " found and opened.");
					Log("Active endpoint count: " + MyUsbDevice.ActiveEndpoints.Count);
					_writeBufferSize = GetWriteBuffer();
					_maxFrames = (int) Math.Floor((decimal)(_writeBufferSize - HeaderBytes)/FrameSize);
				}
			}
			catch (Exception ex)
			{
				Log("There was an error:");
				Log(ex.Message);
			}
			LaunchNewInterface();

		}

		private void UsbDeviceNotifier_OnDeviceNotify(object sender, DeviceNotifyEventArgs e)
		{
			if(e.Device.IdProduct == 1500 && e.Device.IdVendor == 5824)
			{
				if(e.EventType == EventType.DeviceRemoveComplete)
				{
					if (_isPlaying) TogglePlay();
					Log("Disconnecting device.");
					ExitDevice();
				}
				else if(e.EventType == EventType.DeviceArrival)
				{
					Log("Connecting device.");
					EnterDevice();
				}
			}
			//Log(e.ToString());
		}

		private bool SendControlCommand(Command command, short value )
		{
			try
			{
				var controlPacket = new UsbSetupPacket((byte)UsbRequestType.TypeVendor, (byte)command, value, 0, 0);
				object buffer = null;
				int transferred;
				bool result = MyUsbDevice.ControlTransfer(ref controlPacket, buffer, 0, out transferred);
				return result;
			}
			catch (Exception exception)
			{
				Log("");
				Log("An error occurred during control transmission:");
				Log(exception.Message);
			}
			return false;
		}

		private bool ClearLeds()
		{
			bool result = false;
			try
			{
				CurrentFrame.Clear();
				SetControlsToFrame(CurrentFrame);
			
				const byte requestType =
					(byte)UsbEndpointDirection.EndpointOut
					| (byte)UsbRequestRecipient.RecipDevice
					| (byte)UsbRequestType.TypeVendor;

				var controlPacket = new UsbSetupPacket(requestType, (byte)Command.SetFrameData, 0, 0, 8);
				int transferred;
				result = MyUsbDevice.ControlTransfer(ref controlPacket, CurrentFrame.ToByteArray(), 8, out transferred);
				if (!result)
				{
					Log("Frame Data Transfer failure.");
				}
				
			}
			catch (Exception exception)
			{
				Log("");
				Log("An error occurred during control transmission:");
				Log(exception.Message);
			}
			return result;
		}

		private bool SendFrameToDevice(WindowFrame frame)
		{
			bool result = false;

			try
			{
				const byte requestType =
					(byte)UsbEndpointDirection.EndpointOut
					| (byte)UsbRequestRecipient.RecipDevice
					| (byte)UsbRequestType.TypeVendor;

				var controlPacket = new UsbSetupPacket(requestType, (byte)Command.SetFrameData, 0, 0, 8);
				int transferred;
				result = MyUsbDevice.ControlTransfer(ref controlPacket, frame.ToByteArray(), 8, out transferred);
				if (!result)
				{
					Log("Frame Data Transfer failure.");
				}

			}
			catch (Exception exception)
			{
				Log("");
				Log("An error occurred during control transmission:");
				Log(exception.Message);
			}
			return result;
		}

		private bool SendAnimationToDevice()
		{
			if(_frames.Count < 1)
			{
				Log("No frames to write.");
				return false;
			}
			
			bool result = false;
			var animationBytes = new byte[_writeBufferSize];
			var index = 0;

			//Add header
			const int headerSize = 8;
			var usedFrameBytes = (short) (Convert.ToInt16(_frames.Count)*(short)8);
			var usedFramesLowByte = (byte)(usedFrameBytes & 0xff); //Get low byte
			var usedFramesHighByte = (byte)((usedFrameBytes >> 8) & 0xff); //Get high byte

			// Frame Count
			animationBytes[0] = usedFramesLowByte;
			animationBytes[1] = usedFramesHighByte;
			
			// Static Red
			animationBytes[2] = StaticFrame.Red0; 

			//Static Green
			animationBytes[3] = StaticFrame.Green0; 

			//Static Blue
			animationBytes[4] = StaticFrame.Blue0;

			//Frame Delay
			animationBytes[5] = Convert.ToByte(Math.Round((decimal)SpeedSlider.Value * .6m));

			// Pulse Target High
			animationBytes[6] = 250;

			//Pulse Target Low
			animationBytes[7] = 50;

			index += headerSize;

			//Add frames
			foreach(var frame in _frames)
			{
				var frameBytes = frame.ToByteArray();
				var frameByteCount = Buffer.ByteLength(frameBytes);
				Buffer.BlockCopy(frameBytes, 0, animationBytes, index, frameByteCount);
				index += frameByteCount;
			}

			try
			{
				const byte requestType =
					(byte)UsbEndpointDirection.EndpointOut
					| (byte)UsbRequestRecipient.RecipDevice
					| (byte)UsbRequestType.TypeVendor;

				var controlPacket = new UsbSetupPacket(requestType, (byte)Command.SaveAnimation, 0, 0, (short)index);
				int transferred;
				result = MyUsbDevice.ControlTransfer(ref controlPacket, animationBytes, index, out transferred);
				if (!result)
				{
					Log("Frame Data Transfer failure.");
				}
				else
				{
					Log(string.Format("Saved {0} bytes of data to the device.", transferred));
				}
			}
			catch (Exception exception)
			{
				Log("");
				Log("An error occurred during control transmission:");
				Log(exception.Message);
			}
			return result;
		}

		private void SetControlsToFrame(WindowFrame frame)
		{
			CurrentFrame.Red0 = frame.Red0;
			CurrentFrame.Green0 = frame.Green0;
			CurrentFrame.Blue0 = frame.Blue0;
			CurrentFrame.Red1 = frame.Red1;
			CurrentFrame.Green1 = frame.Green1;
			CurrentFrame.Blue1 = frame.Blue1;

			//BrightnessRed.Value = frame.Red0;
			//BrightnessRed2.Value = frame.Red1;
			//BrightnessBlue.Value = frame.Blue0;
			//BrightnessBlue2.Value = frame.Blue1;
			//BrightnessGreen.Value = frame.Green0;
			//BrightnessGree2n.Value = frame.Green1;
			Led0Active.IsChecked = frame.IsBitOn(0);
			Led1Active.IsChecked = frame.IsBitOn(1);
			Led2Active.IsChecked = frame.IsBitOn(2);
			Led3Active.IsChecked = frame.IsBitOn(3);
			Led4Active.IsChecked = frame.IsBitOn(4);
			Led5Active.IsChecked = frame.IsBitOn(5);
			Led6Active.IsChecked = frame.IsBitOn(6);
			Led7Active.IsChecked = frame.IsBitOn(7);
			Led8Active.IsChecked = frame.IsBitOn(8);
			Led9Active.IsChecked = frame.IsBitOn(9);
			Led10Active.IsChecked = frame.IsBitOn(10);
		}

		private WindowFrame GetFrameFromControls()
		{
			var frame = new WindowFrame
			            	{
			            		Red0 = (byte) BrightnessRed.Value,
			            		Red1 = (byte) BrightnessRed2.Value,
			            		Blue0 = (byte) BrightnessBlue.Value,
			            		Blue1 = (byte) BrightnessBlue2.Value,
			            		Green0 = (byte) BrightnessGreen.Value,
			            		Green1 = (byte) BrightnessGree2n.Value
			            	};
			frame.SetBitValue(0, Led0Active.IsChecked.Value);
			frame.SetBitValue(1, Led1Active.IsChecked.Value);
			frame.SetBitValue(2, Led2Active.IsChecked.Value);
			frame.SetBitValue(3, Led3Active.IsChecked.Value);
			frame.SetBitValue(4, Led4Active.IsChecked.Value);
			frame.SetBitValue(5, Led5Active.IsChecked.Value);
			frame.SetBitValue(6, Led6Active.IsChecked.Value);
			frame.SetBitValue(7, Led7Active.IsChecked.Value);
			frame.SetBitValue(8, Led8Active.IsChecked.Value);
			frame.SetBitValue(9, Led9Active.IsChecked.Value);
			frame.SetBitValue(10, Led10Active.IsChecked.Value);
			return frame;
		}

		private int GetWriteBuffer()
		{
			try
			{
				const byte requestType =
					(byte) UsbEndpointDirection.EndpointIn 
					| (byte) UsbRequestRecipient.RecipDevice 
					| (byte) UsbRequestType.TypeVendor;

				var controlPacket = new UsbSetupPacket(requestType, (byte)Command.GetWriteBufferLength, 0, 0, 2);
				var buffer = new byte[2];
				int transferred;
				bool result = MyUsbDevice.ControlTransfer(ref controlPacket, buffer, 2, out transferred);
				if(result)
				{
					int length = buffer[0] + (buffer[1] << 8);
					Log("");
					Log("Reported Writer Buffer Length: " + length);
					return length;
				}
				return 0;
			}
			catch (Exception exception)
			{
				Log("");
				Log("An error occurred during control transmission:");
				Log(exception.Message);
			}
			return 0;
		}

		//private int GetWriteBuffer()
		//{
		//    UsbEndpointReader reader = MyUsbDevice.OpenEndpointReader(ReadEndpointID.Ep01);
		//    var readBuffer = new byte[64];
		//    int bytesRead;
		//    ErrorCode errorCode = ErrorCode.None;
		//    while (errorCode == ErrorCode.None)
		//    {
		//        errorCode = reader.Read(readBuffer, 5000, out bytesRead);
		//    }
		//    return 0;
		//}

		private void BrightnessRed_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			var result = SendControlCommand(Command.ChangeRed0, Convert.ToByte(e.NewValue));
		}

		private void BrightnessGreen_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			var result = SendControlCommand(Command.ChangeGreen0, Convert.ToByte(e.NewValue));
		}

		private void BrightnessBlue_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			var result = SendControlCommand(Command.ChangeBlue0, Convert.ToByte(e.NewValue));
		}

		private void BrightnessRed2_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			var result = SendControlCommand(Command.ChangeRed1, Convert.ToByte(e.NewValue));
		}

		private void BrightnessGreen2_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			var result = SendControlCommand(Command.ChangeGreen1, Convert.ToByte(e.NewValue));
		}

		private void BrightnessBlue2_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			var result = SendControlCommand(Command.ChangeBlue1, Convert.ToByte(e.NewValue));
		}

		private void BrightnessRedStatic_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			StaticFrame.Red0 = StaticFrame.Red1;
			var result = SendControlCommand(Command.ChangeRed1, Convert.ToByte(e.NewValue));
		}

		private void BrightnessGreenStatic_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			StaticFrame.Green0 = StaticFrame.Green1;
			var result = SendControlCommand(Command.ChangeGreen1, Convert.ToByte(e.NewValue));
		}

		private void BrightnessBlueStatic_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			StaticFrame.Blue0 = StaticFrame.Blue1;
			var result = SendControlCommand(Command.ChangeBlue1, Convert.ToByte(e.NewValue));
		}

		private void LedFlag_Changed(object sender, RoutedEventArgs e)
		{
			short led = 0x00;
			if (Led0Active.IsChecked.Value) led += 1;
			if (Led1Active.IsChecked.Value) led += 2;
			if (Led2Active.IsChecked.Value) led += 4;
			if (Led3Active.IsChecked.Value) led += 8;
			if (Led4Active.IsChecked.Value) led += 16;
			if (Led5Active.IsChecked.Value) led += 32;
			if (Led6Active.IsChecked.Value) led += 64;
			if (Led7Active.IsChecked.Value) led += 128;
			if (Led8Active.IsChecked.Value) led += 256;
			if (Led9Active.IsChecked.Value) led += 512;
			if (Led10Active.IsChecked.Value) led += 1024;
			SendControlCommand(Command.ChangeLedFlags, led);
		}

		private void ClearButton_Click(object sender, RoutedEventArgs e)
		{
			ClearLeds();
		}

		private void AddFrameButton_Click(object sender, RoutedEventArgs e)
		{
			int frameNumber = _frames.Count + 1;
			if (frameNumber > _maxFrames)
			{
				Log(string.Format("Frame NOT added. You have reached the maximum frame count of {0}.", _maxFrames));
				return;
			}
			var frame = GetFrameFromControls();
			_frames.Add(frame);
			var item = new ListBoxItem {Content = frameNumber};
			FrameList.Items.Add(item);
		}

		private void DeleteFrameButton_Click(object sender, RoutedEventArgs e)
		{
			var item = FrameList.SelectedItem as ListBoxItem;
			if (item != null)
			{
				var frameNumber = Convert.ToInt32(item.Content) - 1;
				_frames.RemoveAt(frameNumber);
				FrameList.Items.Remove(FrameList.SelectedItem);
			}
		}

		private void FrameList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var item = FrameList.SelectedItem as ListBoxItem;
			if(item != null)
			{
				var frameNumber = Convert.ToInt32(item.Content) - 1;
				SetControlsToFrame(_frames[frameNumber]);
				SendFrameToDevice(_frames[frameNumber]);
			}
		}

		private void PlayButton_Click(object sender, RoutedEventArgs e)
		{
			TogglePlay();
		}

		private bool _isPlaying = false;		
		private void TogglePlay()
		{
			if (!_isPlaying)
			{
				_totalFrames = _frames.Count;
				if(_totalFrames < 1)
				{
					Log("No frames to play.");
					return;
				}
				_isPlaying = true;
				PlayButton.Content = "Stop";
				_currentFramePlayIndex = 0;
				_dispatcherTimer.Start();
			}
			else
			{
				_dispatcherTimer.Stop();
				_isPlaying = false;
				PlayButton.Content = "Play";

			}
		}

		private int _currentFramePlayIndex = 0;
		private int _totalFrames = 0;
		private void DispatcherTimerTick(object sender, EventArgs e)
		{
			if(_isPlaying)
			{
				SendFrameToDevice(_frames[_currentFramePlayIndex]);
				_currentFramePlayIndex++;

				//loop if necessary
				if (_currentFramePlayIndex == _totalFrames) _currentFramePlayIndex = 0;
			}
		}

		private void SpeedSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			SpeedLabel.Content = e.NewValue;
			_dispatcherTimer.Interval = new TimeSpan(0,0,0,0,(int)e.NewValue);
		}

		private void SaveToDeviceButton_Click(object sender, RoutedEventArgs e)
		{
			SendAnimationToDevice();
		}

		private void PreviewStaticColor_Checked(object sender, RoutedEventArgs e)
		{
			StaticFrame.OnBits = ushort.MaxValue;
			AppWindow.DataContext = StaticFrame;
			SendControlCommand(Command.ChangeLedFlags, (short)StaticFrame.OnBits);

		}

		private void PreviewStaticColor_Unchecked(object sender, RoutedEventArgs e)
		{
			AppWindow.DataContext = CurrentFrame;
			LedFlag_Changed(this,new RoutedEventArgs());
		}

		private void button1_Click(object sender, RoutedEventArgs e)
		{
			LaunchNewInterface();
		}

		private void LaunchNewInterface()
		{
			var win = new GlowBeanWindow();
			win.Show();
		}
	}
}
