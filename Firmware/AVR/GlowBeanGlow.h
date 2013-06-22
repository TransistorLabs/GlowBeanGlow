/*
 * Created: 12/2/2012 9:53:12 AM
 *  Author: paul trandem
 *  Copyright (c) 2013 Paul Trandem
 */ 

#ifndef GLOWBEANGLOW_H_
#define GLOWBEANGLOW_H_

	/* Includes: */
	#include <avr/io.h>
	#include <avr/wdt.h>
	#include <avr/power.h>
	#include <avr/interrupt.h>
	#include <string.h>

	#include "Config/AppConfig.h"
	#include "Config/HardwareConfig.h"

	#include "Descriptors.h"
	#include "Drivers/LedDriver.h"
	#include "Drivers/Features.h"
	#include "Drivers/Storage.h"
	#include "Drivers/InputDriver.h"
	#include "Drivers/TempDriver.h"
	#include "OfflineMode.h"
	
	#include <LUFA/Drivers/USB/USB.h>

	/* Structures */
	 
	 typedef union {
		uint8_t RawData[8];
		struct
		{
			uint8_t ButtonSetOne;
			uint8_t TempDataHigh;
			uint8_t TempDataLow;
			uint8_t Debug[5];
		};
	 } ATTR_PACKED GlowBean_OutputReport;
	 

	
	/* Enumerations */

	// The firmware will be in one of the following application states at any given time
	typedef enum
	{
		ApplicationMode_UsbOffline = 0x00,
		ApplicationMode_UsbConnected = 0x01,
		ApplicationMode_UsbActive = 0x02,

		ApplicationMode_DeviceOff = 0xfd,  // primarily used as an initial state from which to switch
		ApplicationMode_UsbErrorState = 0xfe,
		ApplicationMode_GenericErrorState = 0xff
	} Application_ModeOptions;
	
	/* Function Prototypes: */
	
	void SetupHardware(void);

	void EVENT_USB_Device_Connect(void);
	void EVENT_USB_Device_Disconnect(void);
	void EVENT_USB_Device_ConfigurationChanged(void);
	void EVENT_USB_Device_ControlRequest(void);
	void EVENT_USB_Device_StartOfFrame(void);

	bool CALLBACK_HID_Device_CreateHIDReport(USB_ClassInfo_HID_Device_t* const HIDInterfaceInfo,
	uint8_t* const ReportID,
	const uint8_t ReportType,
	void* ReportData,
	uint16_t* const ReportSize);
	void CALLBACK_HID_Device_ProcessHIDReport(USB_ClassInfo_HID_Device_t* const HIDInterfaceInfo,
	const uint8_t ReportID,
	const uint8_t ReportType,
	const void* ReportData,
	const uint16_t ReportSize);

	void CALLBACK_LedDriver_GetNextFrame(LedDriver_OneColorFrame * const nextFrame);
	void EVENT_InputDriver_ButtonDown(uint8_t buttonMask);
	void EVENT_InputDriver_ButtonUp(uint8_t buttonMask);

#endif /* GLOWBEANGLOW_H_ */