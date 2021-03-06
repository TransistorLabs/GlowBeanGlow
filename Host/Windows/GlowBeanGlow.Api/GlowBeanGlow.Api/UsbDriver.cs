﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using GlowBeanGlow.Api.Display;
using GlowBeanGlow.Api.Features;
using GlowBeanGlow.Api.Interfaces;
using HidLibrary;

namespace GlowBeanGlow.Api
{
    public class UsbDriver
    {
        private const int VendorId = 0x03EB;
        private const int ProductId = 0x204F;
        public Action<byte[]> OnReportChange;
        public Action OnModeButtonPressed;
        public Action OnModeButtonReleased;
        public Action<double, double> OnTempChange;
        public Action OnUser1ButtonPressed;
        public Action OnUser1ButtonReleased;
        public Action OnUser2ButtonPressed;
        public Action OnUser2ButtonReleased;
        public Action<bool> OnProgramWriteComplete;

        private bool _attached;
        private bool _connectedToDriver;
        private HidDevice _device;

        private bool _modeButtonLastState;
        private bool _user1ButtonLastState;
        private bool _user2ButtonLastState;
        public IEnumerable<HidDevice> Devices { get; set; }

        /// <summary>
        ///     Occurs when a device is attached.
        /// </summary>
        public event EventHandler DeviceAttached;

        /// <summary>
        ///     Occurs when a device is removed.
        /// </summary>
        public event EventHandler DeviceRemoved;

        /// <summary>
        ///     After a successful connection, a DeviceAttached event will normally be sent.
        /// </summary>
        /// <returns>True if a device is connected, False otherwise.</returns>
        public bool Connect()
        {
            //TODO: introduce a way to handle multiple devices throughout driver
            Devices = HidDevices.Enumerate(VendorId, ProductId);
            _device = Devices.FirstOrDefault();

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

        public void RenderFrame(LiveFrame frame)
        {
            if (_connectedToDriver)
            {
                var report = new HidReport(9,
                                           new HidDeviceData(frame.GetReportData(0),
                                                             HidDeviceData.ReadStatus.NoDataRead));
                _device.WriteReport(report, null);
            }
        }

        public void RenderFrame(FullColorLiveFrame frame)
        {
            if (_connectedToDriver)
            {
                for (int i = 0; i < 6; i++)
                {
                    var report = new HidReport(9, new HidDeviceData(frame.GetReportDataForPage(i), HidDeviceData.ReadStatus.NoDataRead));
                    _device.WriteReport(report, null);
                    Thread.Sleep(5);
                }
            }
        }

        public bool StartPlaybackOfStoredProgram()
        {
            if (_connectedToDriver && !_isProgramming)
            {
                var playCommand = new SetFeatureReport {Command = SetFeatureCommands.ChangeFeatureMode};
                playCommand.CommandData[0] = (byte) FeatureModeOptions.PlayStoredProgram;
                return _device.WriteFeatureData(playCommand.GetReportData());
            }
            return false;
        }

        public void StopPlaybackOfStoredProgram()
        {
            if (_connectedToDriver && !_isProgramming)
            {
                var stopCommand = new SetFeatureReport { Command = SetFeatureCommands.ChangeFeatureMode };
                stopCommand.CommandData[0] = (byte)FeatureModeOptions.RenderLiveFrameData;
                _device.WriteFeatureData(stopCommand.GetReportData());
            }
        }

        private bool _isProgramming = false;
        public void WriteAnimationProgram(IList<IInstruction> instructions)
        {
            _isProgramming = true;
            var stack = new Stack<IInstruction>(instructions.Reverse());
            
            // Put device in program mode
            var startCommand = new SetFeatureReport {Command = SetFeatureCommands.ChangeFeatureMode};
            startCommand.CommandData[0] = (byte)FeatureModeOptions.StoreProgramStart;
            _device.WriteFeatureData(startCommand.GetReportData());

            WriteProgramData(stack);
        }

        private void WriteProgramData(Stack<IInstruction> stack)
        {
            if (stack.Count == 0)
            {
                //Finalize program
                SendProgramStopCommand();
                return;
            }

            var topInstruction = stack.Pop();
            _device.Write(topInstruction.GetReportData(),
                          (success) =>
                              {
                                  if (!success)
                                  {
                                      
                                      var stopSuccess = SendProgramStopCommand();

                                      if (!stopSuccess)
                                      {
                                          throw new ApplicationException("Program Write failed. Device in unknown state.");
                                      }
                                  }
                                  Thread.Sleep(50); // TODO: this delay thing is pretty lame; look into better ways to ensure a write gets completed
                                  WriteProgramData(stack);
                                  
                              });
        }

        private bool SendProgramStopCommand()
        {
            var stopCommand = new SetFeatureReport {Command = SetFeatureCommands.ChangeFeatureMode};
            stopCommand.CommandData[0] = (byte) FeatureModeOptions.StoreProgramStop;
            var result =  _device.WriteFeatureData(stopCommand.GetReportData());
            _isProgramming = false;

            if (OnProgramWriteComplete != null)
            {
                OnProgramWriteComplete(result);
            }

            return result;
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
            if (_attached == false)
            {
                return;
            }
            byte[] reportBytes = report.Data;

            byte buttons = reportBytes[0];
            bool button1Pressed = ((buttons & 0x08) > 0);
            bool button2Pressed = ((buttons & 0x80) > 0);
            bool button3Pressed = ((buttons & 0x40) > 0);

            // trigger pressed events on edges
            if (button1Pressed && !_modeButtonLastState)
            {
                if (OnModeButtonPressed != null)
                {
                    OnModeButtonPressed();
                    _modeButtonLastState = true;
                }
            }

            if (button2Pressed && !_user1ButtonLastState)
            {
                if (OnUser1ButtonPressed != null)
                {
                    OnUser1ButtonPressed();
                    _user1ButtonLastState = true;
                }
            }

            if (button3Pressed && !_user2ButtonLastState)
            {
                if (OnUser2ButtonPressed != null)
                {
                    OnUser2ButtonPressed();
                    _user2ButtonLastState = true;
                }
            }

            // trigger released events on edges
            if (!button1Pressed && _modeButtonLastState)
            {
                if (OnModeButtonReleased != null)
                {
                    OnModeButtonReleased();
                    _modeButtonLastState = false;
                }
            }

            if (!button2Pressed && _user1ButtonLastState)
            {
                if (OnUser1ButtonReleased != null)
                {
                    OnUser1ButtonReleased();
                    _user1ButtonLastState = false;
                }
            }

            if (!button3Pressed && _user2ButtonLastState)
            {
                if (OnUser2ButtonReleased != null)
                {
                    OnUser2ButtonReleased();
                    _user2ButtonLastState = false;
                }
            }

            OnReportChange(reportBytes);
            ProcessTemperature(reportBytes);
            
            _device.ReadReport(OnReport);
        }

        public DeviceConfiguration GetDeviceConfiguration()
        {
            if (_device == null) return null;

            byte[] reportBytes;
            _device.ReadFeatureData(out reportBytes);
            DeviceConfiguration config = DeviceConfiguration.CreateConfigurationObjectFromBytes(reportBytes);
            return config;
        }

        public void SetDeviceConfiguration(RgbColor staticColor)
        {
            if (_device == null) return;

            var report = new SetFeatureReport {Command = SetFeatureCommands.SetStaticColor};
            staticColor.GetBytes().CopyTo(report.CommandData, 0);
            _device.WriteFeatureData(report.GetReportData());
        }

        private void ProcessTemperature(byte[] reportBytes)
        {
            byte tempHighByte = reportBytes[1];
            byte tempLowByte = reportBytes[2];

            bool isNegative = (tempHighByte & 0x80) > 0;
            
            byte tempWholeNumberDataC = 0x00;

            var data = new byte[2];

            if (isNegative)
            {
                data[1] = 0xFF;
            }

            tempWholeNumberDataC = (byte) (tempHighByte << 1);
            tempWholeNumberDataC |= (byte) ((tempLowByte & 0x80) >> 7);
            data[0] = tempWholeNumberDataC;

            short tempCIntPortion = BitConverter.ToInt16(data, 0);

            float tempC = tempCIntPortion;

            if ((tempLowByte & 0x08) > 0) { tempC += 0.0625F; }
            if ((tempLowByte & 0x10) > 0) { tempC += 0.125F; }
            if ((tempLowByte & 0x20) > 0) { tempC += 0.25F; }
            if ((tempLowByte & 0x40) > 0) { tempC += 0.5F; }

            
            //°C  x  9/5 + 32 = °F
            float tempF = tempC*9/5 + 32;

            if (OnTempChange != null)
            {
                OnTempChange(tempC, tempF);
            }
        }

    }
}