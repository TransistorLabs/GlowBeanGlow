GlowBeanGlow v2
============

GlowBeanGlow v2 is a USB HID device with a circle of RGB LEDs, some buttons and a temp sensor.  

Operational Summary
-------------------

It is designed with several modes of operation:

  * **Online Mode**: In this mode, the device is actively connected to a host computer USB port and is ready for use with GlowBeanGlow host software. The device accepts RGB "pixel" data to allow host software to render colors to any/all LEDs.  It also exposes button presses and periodic temperature data via input reports.  The device can also be configured for various "offline" modes such as setting the Nightlight Mode color, and setting animation instructions data for use in the Animation Mode.
  
  * **Offline Modes**: In this mode, the device is connected to a USB power source without a data connection (like a USB wall charger).  When the device detects that it is in offline mode, it offers the following sub-modes:
    * **Nightlight Mode** (also called "static" mode): This mode simply shows all pixels as one color.  This defaults to white, but can be configured to any color when connected to a host computer.
    * **Temperature Mode** (currently incomplete): This mode will indicate whether the detected ambient temperature is warmer or cooler than a specified "normal" temperature.  The "normal" temperature will be configurable when the device is connected to a host computer.
    * **Color Cycle Mode**: This mode simply fades the LEDs through a preset range of colors.
    * **Animation Mode**: This mode plays back a user-programmed animation. The animation instruction set is a work in progress, but currently SetFrame, IncrementFrame, Jump, and Temp/Button condition support is planned and partially implemented.

Note: In Online mode, GBGv2 supports individual colors for each RGB "pixel."  Due to memory contraints in the system, this is *not* currently supported in Offline Animation mode.


Hardware Summary
----------------
_coming soon_


Host API Summary
----------------
_coming soon_


License Information
-------------------
GlowBeanGlow v2 firmware uses the LUFA library created by Dean Camera.  This library is used under the license specified here: https://github.com/TransistorLabs/GlowBeanGlow/blob/master/Firmware/AVR/LUFA/License.txt

GlowBeanGlow v2 Windows Host API uses the HidLibrary created by Mike O'Brien.  This library is used under the license specified here: https://github.com/mikeobrien/HidLibrary/blob/master/LICENSE

All custom GlowBeanGlow v2 source files, including firmware, host software, schematics and board files are copyright (c) 2013 Paul Trandem and are released under the Creative Commons Attribution-ShareAlike 3.0 Unported license, http://creativecommons.org/licenses/by-sa/3.0/.




