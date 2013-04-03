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
            
            _usbDriver.OnModeButtonPressed += () => Dispatcher.Invoke(() => Button1.Style = Resources["ButtonOnStyle"] as Style);
            _usbDriver.OnModeButtonReleased += () => Dispatcher.Invoke(() => Button1.Style = Resources["ButtonOffStyle"] as Style);

            _usbDriver.OnUser1ButtonPressed += () => Dispatcher.Invoke(() => Button2.Style = Resources["ButtonOnStyle"] as Style);
            _usbDriver.OnUser1ButtonReleased += () => Dispatcher.Invoke(() => Button2.Style = Resources["ButtonOffStyle"] as Style);

            _usbDriver.OnUser2ButtonPressed += () => Dispatcher.Invoke(() => Button3.Style = Resources["ButtonOnStyle"] as Style);
            _usbDriver.OnUser2ButtonReleased += () => Dispatcher.Invoke(() => Button3.Style = Resources["ButtonOffStyle"] as Style);

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
                    DegreeOutputC.Text = string.Format("{0:00.0000}° C", c);
                    DegreeOutputF.Text = string.Format("{0:00.0}", f);
				});
		}

		private void RedValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			_setFrame.Red = Convert.ToByte(e.NewValue);
			RenderFrame();
		}

		private void GreenValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			_setFrame.Green = Convert.ToByte(e.NewValue);
			RenderFrame();
		}

		private void BlueValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			_setFrame.Blue = Convert.ToByte(e.NewValue);
			RenderFrame();
		}

		private void LedChanged(object sender, RoutedEventArgs e)
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
