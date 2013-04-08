using System;
using GlowBeanGlow.Api.Display;
using GlowBeanGlow.Api.Interfaces;

namespace GlowBeanGlow.Api.Features
{
    public class DeviceConfiguration
    {
        /*	typedef struct 
            {
                uint8_t StaticRed;
                uint8_t StaticGreen;
                uint8_t StaticBlue;
                uint8_t ReservedA;
                uint8_t ReservedB;
                uint16_t MaxInstructions;
                uint8_t ReservedC;
            } Storage_SettingsResponse;
         * */

        public DeviceConfiguration()
        {
            OfflineColor = new RgbColor();
        }

        /// <summary>
        /// The current setting of the colors used in static mode
        /// </summary>
        public RgbColor OfflineColor { get; set; }

        /// <summary>
        /// The max number of 8-byte instructions that can be stored in the device for offline Animation Mode
        /// </summary>
        public ushort MaxInstructions { get; private set; }

        /// <summary>
        /// Used to get a populated DeviceConfiguration object based on a byte data array
        /// </summary>
        /// <param name="byteData">The USB configuration report</param>
        /// <returns>A new DeviceConfiguration setting</returns>
        public static DeviceConfiguration CreateConfigurationObjectFromBytes(byte[] byteData)
        {
            var settings = new DeviceConfiguration
                {
                    OfflineColor = { Red = byteData[1], Green = byteData[2], Blue = byteData[3] },
                    MaxInstructions = (ushort)BitConverter.ToInt16(byteData, 6)
                };
            return settings;
        }
    }
}
