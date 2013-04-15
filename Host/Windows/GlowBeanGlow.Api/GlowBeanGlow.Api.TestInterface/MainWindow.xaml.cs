﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Threading;
using GlowBeanGlow.Api.Display;
using GlowBeanGlow.Api.Instructions;
using GlowBeanGlow.Api.Interfaces;

namespace GlowBeanGlow.Api.TestInterface
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly UsbDriver _usbDriver;
        private readonly LiveFrame _frame;

        public MainWindow()
        {
            InitializeComponent();
            _usbDriver = new UsbDriver();
            _usbDriver.Connect();
            _usbDriver.OnTempChange += OnTempChange;

            _usbDriver.OnModeButtonPressed += () => Dispatcher.Invoke(() => { Button1.Style = Resources["ButtonOnStyle"] as Style; });
            _usbDriver.OnModeButtonReleased += () => Dispatcher.Invoke(() => Button1.Style = Resources["ButtonOffStyle"] as Style);

            _usbDriver.OnUser1ButtonPressed += () => Dispatcher.Invoke(() => { Button2.Style = Resources["ButtonOnStyle"] as Style; });
            _usbDriver.OnUser1ButtonReleased += () => Dispatcher.Invoke(() => Button2.Style = Resources["ButtonOffStyle"] as Style);

            _usbDriver.OnUser2ButtonPressed += () => Dispatcher.Invoke(() => { Button3.Style = Resources["ButtonOnStyle"] as Style; });
            _usbDriver.OnUser2ButtonReleased += () => Dispatcher.Invoke(() => Button3.Style = Resources["ButtonOffStyle"] as Style);

            // TODO: add proper context binding here
            _frame =
                new LiveFrame()
                    {
                        Color = { Red = (byte)Red.Value, Green = (byte)Green.Value, Blue = (byte)Blue.Value },
                        Leds = { LedRawBits = 0x0000 }
                    };
            RenderFrame();
        }

        private void OnTempChange(double c, double f)
        {
            Application.Current.Dispatcher.Invoke(() =>
                {
                    DegreeOutputC.Text = string.Format("{0:00.0000}° C", c);
                    DegreeOutputF.Text = string.Format("{0:00.0}", f);
                });
        }

        private void RedValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _frame.Color.Red = Convert.ToByte(e.NewValue);
            RenderFrame();
        }

        private void GreenValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _frame.Color.Green = Convert.ToByte(e.NewValue);
            RenderFrame();
        }

        private void BlueValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _frame.Color.Blue = Convert.ToByte(e.NewValue);
            RenderFrame();
        }

        private void LedChanged(object sender, RoutedEventArgs e)
        {
            _frame.Leds[0] = Led1.IsChecked.Value;
            _frame.Leds[1] = Led2.IsChecked.Value;
            _frame.Leds[2] = Led3.IsChecked.Value;
            _frame.Leds[3] = Led4.IsChecked.Value;
            _frame.Leds[4] = Led5.IsChecked.Value;
            _frame.Leds[5] = Led6.IsChecked.Value;
            _frame.Leds[6] = Led7.IsChecked.Value;
            _frame.Leds[7] = Led8.IsChecked.Value;
            _frame.Leds[8] = Led9.IsChecked.Value;
            _frame.Leds[9] = Led10.IsChecked.Value;
            _frame.Leds[10] = Led11.IsChecked.Value;
            RenderFrame();
        }

        private void RenderFrame()
        {
            _usbDriver.RenderFrame(_frame);
        }

        private void Configuration_Click(object sender, RoutedEventArgs e)
        {
            var configuration = _usbDriver.GetDeviceConfiguration();
            if (configuration == null) return;

            var configOut = string.Format("Offline Red: \t{0}\nOffline Green: \t{1}\nOffline Blue: \t{2}\nMax instructions: \t{3}",
                                          configuration.OfflineColor.Red,
                                          configuration.OfflineColor.Green,
                                          configuration.OfflineColor.Blue,
                                          configuration.MaxInstructions);
            MessageBox.Show(configOut);
        }

        private void SetStatic_Click(object sender, RoutedEventArgs e)
        {
            _usbDriver.SetDeviceConfiguration(_frame.Color);
        }

        private FullColorLiveFrame _fullColorFrame = null;
        private void FullColorRotateTest_Click(object sender, RoutedEventArgs e)
        {
            if (_fullColorFrame == null)
            {
                _fullColorFrame = new FullColorLiveFrame();
                _fullColorFrame.Leds.LedRawBits = 0x0007;
                _fullColorFrame.Colors.Add(new Display.RgbColor { Red = 0xff, Green = 0x00, Blue = 0x00 });
                _fullColorFrame.Colors.Add(new Display.RgbColor { Red = 0xaa, Green = 0x10, Blue = 0x00 });
                _fullColorFrame.Colors.Add(new Display.RgbColor { Red = 0x88, Green = 0x44, Blue = 0x00 });
                _fullColorFrame.Colors.Add(new Display.RgbColor { Red = 0x66, Green = 0x88, Blue = 0x00 });
                _fullColorFrame.Colors.Add(new Display.RgbColor { Red = 0x00, Green = 0xff, Blue = 0x00 });
                _fullColorFrame.Colors.Add(new Display.RgbColor { Red = 0x00, Green = 0xff, Blue = 0x66 });
                _fullColorFrame.Colors.Add(new Display.RgbColor { Red = 0x00, Green = 0xdd, Blue = 0x99 });
                _fullColorFrame.Colors.Add(new Display.RgbColor { Red = 0x00, Green = 0x99, Blue = 0xff });
                _fullColorFrame.Colors.Add(new Display.RgbColor { Red = 0x00, Green = 0x00, Blue = 0xff });
                _fullColorFrame.Colors.Add(new Display.RgbColor { Red = 0x33, Green = 0x00, Blue = 0xff });
                _fullColorFrame.Colors.Add(new Display.RgbColor { Red = 0x66, Green = 0x00, Blue = 0xdd });
            }
            else
            {
                _fullColorFrame.Leds.RotateClockwise();
            }

            _usbDriver.RenderFrame(_fullColorFrame);
            
        }

        private FullColorLiveFrame _fullColorSpectrumFrame = null;
        private void FullColorSpectrumTest_Click(object sender, RoutedEventArgs e)
        {
            if (_fullColorSpectrumFrame == null)
            {
                _fullColorSpectrumFrame = new FullColorLiveFrame();
                _fullColorSpectrumFrame.Leds.LedRawBits = 0xffff;
                _fullColorSpectrumFrame.Colors.Add(new Display.RgbColor { Red = 0xff, Green = 0x00, Blue = 0x00 });
                _fullColorSpectrumFrame.Colors.Add(new Display.RgbColor { Red = 0xaa, Green = 0x10, Blue = 0x00 });
                _fullColorSpectrumFrame.Colors.Add(new Display.RgbColor { Red = 0x88, Green = 0x44, Blue = 0x00 });
                _fullColorSpectrumFrame.Colors.Add(new Display.RgbColor { Red = 0x66, Green = 0x88, Blue = 0x00 });
                _fullColorSpectrumFrame.Colors.Add(new Display.RgbColor { Red = 0x00, Green = 0xff, Blue = 0x00 });
                _fullColorSpectrumFrame.Colors.Add(new Display.RgbColor { Red = 0x00, Green = 0xff, Blue = 0x66 });
                _fullColorSpectrumFrame.Colors.Add(new Display.RgbColor { Red = 0x00, Green = 0xdd, Blue = 0x99 });
                _fullColorSpectrumFrame.Colors.Add(new Display.RgbColor { Red = 0x00, Green = 0x99, Blue = 0xff });
                _fullColorSpectrumFrame.Colors.Add(new Display.RgbColor { Red = 0x00, Green = 0x00, Blue = 0xff });
                _fullColorSpectrumFrame.Colors.Add(new Display.RgbColor { Red = 0x33, Green = 0x00, Blue = 0xff });
                _fullColorSpectrumFrame.Colors.Add(new Display.RgbColor { Red = 0x66, Green = 0x00, Blue = 0xdd });
            }
            else
            {
                _fullColorSpectrumFrame.RotateColorsClockwise();
            }

            _usbDriver.RenderFrame(_fullColorSpectrumFrame);

        }

        private void WriteTestAnimation_Click(object sender, RoutedEventArgs e)
        {
            var instructions = new List<IInstruction>();
            instructions.Add(new SetFrameInstruction { Color = new RgbColor { Red = 0xff, Green = 0x00, Blue = 0x00 }, MillisecondsHold = 0x00ff, Leds = new LedState { LedRawBits = 0x5555 } });
            instructions.Add(new SetFrameInstruction { Color = new RgbColor { Red = 0xff, Green = 0x00, Blue = 0x00 }, MillisecondsHold = 0x00ff, Leds = new LedState { LedRawBits = 0xaaaa } });
            _usbDriver.WriteAnimationProgram(instructions);
        }
    }
}
