using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using GlowBeanGlow.Api.Display;
using GlowBeanGlow.Api.Instructions;
using GlowBeanGlow.Api.Interfaces;
using GlowBeanGlow.Compiler;
using Microsoft.Win32;

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
            _usbDriver.OnReportChange += (bytes) => Dispatcher.Invoke(() =>
                {
                    RawByteOutput.Text = "";
                    foreach (var b in bytes)
                    {
                        RawByteOutput.Text += string.Format("{0:X2} ", b);
                    }
                });

            _usbDriver.OnTempChange += OnTempChange;

            _usbDriver.OnModeButtonPressed += () => Dispatcher.Invoke(() => { Button1.Style = Resources["ButtonOnStyle"] as Style; });
            _usbDriver.OnModeButtonReleased += () => Dispatcher.Invoke(() => Button1.Style = Resources["ButtonOffStyle"] as Style);

            _usbDriver.OnUser2ButtonPressed += () => Dispatcher.Invoke(() => { Button3.Style = Resources["ButtonOnStyle"] as Style; });
            _usbDriver.OnUser2ButtonReleased += () => Dispatcher.Invoke(() => Button3.Style = Resources["ButtonOffStyle"] as Style);

            _usbDriver.OnProgramWriteComplete += (success) => MessageBox.Show("Write Complete. Successful: " + success);

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
            _frame.Leds[11] = Led12.IsChecked.Value;
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

            var configOut = string.Format("Offline Red: \t{0}\nOffline Green: \t{1}\nOffline Blue: \t{2}\nMax instructions: \t{3}\nTemp Device ID: \t{4}",
                                          configuration.OfflineColor.Red,
                                          configuration.OfflineColor.Green,
                                          configuration.OfflineColor.Blue,
                                          configuration.MaxInstructions,
                                          configuration.TempDeviceId);
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
                _fullColorFrame.Colors.Add(new Display.RgbColor { Red = 0x99, Green = 0x00, Blue = 0x99 });
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
                _fullColorSpectrumFrame = new FullColorLiveFrame {Leds = {LedRawBits = 0xffff}};
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
                _fullColorSpectrumFrame.Colors.Add(new Display.RgbColor { Red = 0x99, Green = 0x00, Blue = 0x99 });
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
            instructions.Add(new SetFrameInstruction { Color = new RgbColor { Red = 0xff, Green = 0x00, Blue = 0x00 }, MillisecondsHold = 0x00ff, Leds = new LedState { LedRawBits  = 0x00ff } });
            instructions.Add(new SetFrameInstruction { Color = new RgbColor { Red = 0, Green = 0, Blue = 255 }, MillisecondsHold = 0x000f, Leds = new LedState { LedRawBits = 0x0001 } });
            instructions.Add(new IncrementFrameInstruction
                {
                    BlueIncrement = -1,
                    GreenIncrement = 1,
                    ColorIncrementCount = 255,
                    ColorIncrementDelayMs = 4
                });

            instructions.Add(new IncrementFrameInstruction
            {
                BlueIncrement = 1,
                GreenIncrement = -2,
                RedIncrement = 4,
                ColorIncrementCount = 127,
                ColorIncrementDelayMs = 4,
                LedShiftCount = 33,
                LedShiftDelayMs = 6,
                LedShiftType = LedShiftOptions.ShiftLedRight
            });

            instructions.Add(new IncrementFrameInstruction
            {
                BlueIncrement = -1,
                GreenIncrement = 4,
                RedIncrement = -2,
                ColorIncrementCount = 33,
                ColorIncrementDelayMs = 4,
                LedShiftCount = 33,
                LedShiftDelayMs = 4,
                LedShiftType = LedShiftOptions.ShiftLedLeft
            });

            instructions.Add(new SetFrameInstruction { Color = new RgbColor { Red = 0x00, Green = 0xff, Blue = 0x00 }, MillisecondsHold = 0x03eb, Leds = new LedState { LedRawBits = 0x0100 } });

            instructions.Add(new IncrementFrameInstruction
            {
                LedShiftCount = 33,
                LedShiftDelayMs = 10,
                LedShiftType = LedShiftOptions.ShiftLedRight
            });

            instructions.Add(new IncrementFrameInstruction
            {
                LedShiftCount = 33,
                LedShiftDelayMs = 10,
                LedShiftType = LedShiftOptions.ShiftLedLeft
            });

            instructions.Add(new SetFrameInstruction { Color = new RgbColor { Red = 0x00, Green = 0xff, Blue = 0x00 }, MillisecondsHold = 0x0300, Leds = new LedState { LedRawBits = 0x0200 } });
            instructions.Add(new SetFrameInstruction { Color = new RgbColor { Red = 0x00, Green = 0xff, Blue = 0x00 }, MillisecondsHold = 0x0300, Leds = new LedState { LedRawBits = 0x0400 } });
            instructions.Add(new JumpToInstruction { JumpTargetIndex = 6 });

            _usbDriver.WriteAnimationProgram(instructions);
        }

        private bool _isPlaying = false;
        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isPlaying)
            {
                _usbDriver.StopPlaybackOfStoredProgram();
                _isPlaying = false;
                PlayButton.Content = "Play";
            }
            else
            {
                var started = _usbDriver.StartPlaybackOfStoredProgram();
                if (started)
                {
                    _isPlaying = true; 
                    PlayButton.Content = "Stop";
                }
            }
        }

        private void RotateRightButton_Click(object sender, RoutedEventArgs e)
        {
            _frame.Leds.RotateClockwise();
            RenderFrame();
        }

        private void RotateLeftButton_Click(object sender, RoutedEventArgs e)
        {
            _frame.Leds.RotateCounterClockwise();
            RenderFrame();
        }

        private string _lastFilename;
        private IList<IInstruction> _lastInstructions = new List<IInstruction>();
        private void CompileWrite_Click(object sender, RoutedEventArgs e)
        {
            var openDialog = new OpenFileDialog();
            if (openDialog.ShowDialog().Value)
            {
                try
                {
                    _lastFilename = openDialog.FileName;
                    CompileCurrent();
                    _usbDriver.WriteAnimationProgram(_lastInstructions);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        private void ReCompileReWrite_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CompileCurrent();
                _usbDriver.WriteAnimationProgram(_lastInstructions.ToList());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void CompileReWrite_Click(object sender, RoutedEventArgs e)
        {
            _usbDriver.WriteAnimationProgram(_lastInstructions.ToList());
        }

        private void CompileCurrent()
        {
            var fileContents = File.ReadAllText(_lastFilename);
            fileContents = Preprocessor.Process(fileContents);
            var lexer = new Lexer(fileContents);
            var tokens = lexer.GetTokens();
            var parser = new Parser(tokens);
            _lastInstructions = parser.Parse().ToList();
        }
    }
}
