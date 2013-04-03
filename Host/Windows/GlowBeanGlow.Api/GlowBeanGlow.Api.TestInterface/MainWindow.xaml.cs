using System;
using System.Windows;
using System.Windows.Threading;
using GlowBeanGlow.Api.DataTypes;

namespace GlowBeanGlow.Api.TestInterface
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow
	{
		private readonly UsbDriver _usbDriver;
		private readonly SetFrameInstruction _setFrame;

		public MainWindow()
		{
			InitializeComponent();
			_usbDriver = new UsbDriver();
			_usbDriver.Connect();
			_usbDriver.OnTempChange += OnTempChange;
			_setFrame =
				new SetFrameInstruction
					{
						Blue = (byte) Blue.Value,
						Red = (byte) Red.Value,
						Green = (byte) Green.Value,
						Leds = {LedRawBits = 0x0000}
					};
			RenderFrame();
		}

		private void OnTempChange(double c, double f)
		{
			Dispatcher.Invoke(() =>
				{

				});
		}

		private void Red_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			_setFrame.Red = Convert.ToByte(e.NewValue);
			RenderFrame();
		}

		private void Green_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			_setFrame.Green = Convert.ToByte(e.NewValue);
			RenderFrame();
		}

		private void Blue_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			_setFrame.Blue = Convert.ToByte(e.NewValue);
			RenderFrame();
		}

		private void Led_Changed(object sender, RoutedEventArgs e)
		{
			_setFrame.Leds[0] = Led1.IsChecked.Value;
			_setFrame.Leds[1] = Led2.IsChecked.Value;
			_setFrame.Leds[2] = Led3.IsChecked.Value;
			_setFrame.Leds[3] = Led4.IsChecked.Value;
			_setFrame.Leds[4] = Led5.IsChecked.Value;
			_setFrame.Leds[5] = Led6.IsChecked.Value;
			_setFrame.Leds[6] = Led7.IsChecked.Value;
			_setFrame.Leds[7] = Led8.IsChecked.Value;
			_setFrame.Leds[8] = Led9.IsChecked.Value;
			_setFrame.Leds[9] = Led10.IsChecked.Value;
			_setFrame.Leds[10] = Led11.IsChecked.Value;
			RenderFrame();
		}

		private void RenderFrame()
		{
			_usbDriver.RenderFrame(_setFrame);
		}
	}
}
