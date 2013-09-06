/*
* Created: 11/30/2012 8:25:12 PM
*  Author: paul trandem
 *  Copyright (c) 2013 Paul Trandem
*/

#include "GlowBeanGlow.h"

volatile static Application_ModeOptions ApplicationMode = ApplicationMode_UsbOffline;
volatile static Application_ModeOptions LastApplicationMode = ApplicationMode_DeviceOff;
volatile static Features_ModeOptions FeatureMode = FeatureModeOptions_RenderLiveFrameData;
volatile static uint8_t ButtonPressedMask = 0x00;
volatile static uint8_t LastButtonPressedMask = 0x00;
volatile static uint16_t LastTempDataRaw = 0xffff;
volatile static uint8_t ButtonMaskRaw = 0x00;
uint8_t PreviousOutputReportBuffer[sizeof(GlowBean_OutputReport)];


/** LUFA HID Class driver interface configuration and state information. This structure is
	*  passed to all HID Class driver functions, so that multiple instances of the same class
	*  within a device can be differentiated from one another.
	*/
	USB_ClassInfo_HID_Device_t Generic_HID_Interface =
	{
		.Config =
		{
			.InterfaceNumber              = 0,
			.ReportINEndpoint             =
			{
				.Address              = GENERIC_IN_EPADDR,
				.Size                 = GENERIC_EPSIZE,
				.Banks                = 1,
			},
			.PrevReportINBuffer           = PreviousOutputReportBuffer,
			.PrevReportINBufferSize       = sizeof(PreviousOutputReportBuffer),
		},
	};


static void ApplicationMode_Task(void);

/************************************************************************/
/* INTERRUPT SERVICE ROUTINES                                           */
/************************************************************************/
ISR(TIMER0_COMPA_vect, ISR_NOBLOCK)
{
	LedDriver_MillisecondTask();
	TempDriver_MillisecondTask();
}

/************************************************************************/
/* MAIN                                                                 */
/************************************************************************/
int main(void)
{
	SetupHardware();

	GlobalInterruptEnable();
	
	for (;;)
	{
		HID_Device_USBTask(&Generic_HID_Interface);
		USB_USBTask();
		InputDriver_Task();
		TempDriver_Task();
		ApplicationMode_Task();
		LedDriver_Task();
	}
}

// Tasks that require immediate action
void ApplicationMode_Task(void)
{
	switch(ApplicationMode)
	{
		case ApplicationMode_UsbConnected:
			// Usb Connected, but not yet ready to go
			//LedDriver_RenderOneColorFrame(0x00, 0x00, 0xff, 0xffff, 0x0000);
			LedDriver_FadeToColor(0x00, 0x00, 0xff, 0xffff, 0x00ff);
			break;

		case ApplicationMode_UsbActive:
			if(LastApplicationMode != ApplicationMode)
			{
				// Usb Online Mode
				// Set to green if we're first switching into Active Usb
				//LedDriver_RenderOneColorFrame(0x00, 0xff, 0x00, 0xffff, 0x0000);
				LedDriver_FadeToColor(0x00, 0xff, 0x00, 0xffff, 0x00ff);
			}
			break;

		case ApplicationMode_UsbErrorState:
			// Usb connection error mode
			LedDriver_RenderOneColorFrame(0xff, 0x00, 0x00, 0x5555, 0x0000);
			break;

		case ApplicationMode_GenericErrorState:
			// Indicate some other error mode
			LedDriver_RenderOneColorFrame(0xff, 0x00, 0x00, 0xffff, 0x0000);
			break;
			
		case ApplicationMode_UsbOffline:
		default:
			break;
	}
	
	LastApplicationMode = ApplicationMode;
}

/** Configures the board hardware and chip peripherals for the demo's functionality. */
void SetupHardware()
{
	/* Disable watchdog if enabled by bootloader/fuses */
	MCUSR &= ~(1 << WDRF);
	
	/*
		My two-year-old daughter, Gretchen, to whom this project is dedicated
		and for whom it was originally conceived, managed to sneak
		on to my keyboard today. She typed the following into this code file:
		
		nnn    cv
		
		While minimal, I thought her addition poetic, so here it remains.
		
		- paul (March 10th, 2013)
	*/
	
	wdt_disable();

	/* Disable clock division */
	clock_prescale_set(clock_div_1);

	/* Timer0 initialization */

	// Prescale to 64, TopA at 250 (0xfa), results in 1ms timer
	Timer0Config_EnableCTCMode(0xfa, 0x00, Timer0Config_IOClk_64);

	// Enable Timer0 Interrupt A, do not enable global interrupts yet
	Timer0Config_EnableMatchInterrupts(true, false, false);

	/* Driver Initialization */
	Features_Init();
	InputDriver_Init(EVENT_InputDriver_ButtonDown, EVENT_InputDriver_ButtonUp);
	LedDriver_Init(CALLBACK_LedDriver_GetNextFrame);
	TempDriver_Init();
	OfflineMode_Init();
	USB_Init();
	
	LedDriver_Clear();
}

/************************************************************************/
/* INPUT DRIVER EVENT CALLBACK                                          */
/************************************************************************/
void EVENT_InputDriver_ButtonUp(uint8_t buttonMask)
{
	ButtonMaskRaw = buttonMask;
	ButtonPressedMask &= ~buttonMask;
	
	if(ApplicationMode == ApplicationMode_UsbOffline)
	{
		if(INPUTDRIVER_MODESWITCHKEYMASK(buttonMask))
		{
			LedDriver_Clear();
			OfflineMode_SetNextOfflineMode();
		}
	}
}

void EVENT_InputDriver_ButtonDown(uint8_t buttonMask)
{
	ButtonMaskRaw = buttonMask;
	ButtonPressedMask |= buttonMask;
	
	if(INPUTDRIVER_USERASWITCHKEYMASK(buttonMask))
	{
		OfflineMode_ProcessButtonPressUserA();
	}
}

/************************************************************************/
/* LED DRIVER CALLBACK                                                  */
/************************************************************************/
void CALLBACK_LedDriver_GetNextFrame(LedDriver_OneColorFrame * const nextFrame)
{
	switch(ApplicationMode)
	{
		case ApplicationMode_UsbConnected:
			// Usb Connected, but not yet ready to go
			//LedDriver_RenderOneColorFrame(0x00, 0x00, 0xff, 0xffff, 0x0000);
			break;

		case ApplicationMode_UsbActive:
			// Usb Online Mode
			if(LastApplicationMode != ApplicationMode)
			{
				//LedDriver_RenderOneColorFrame(0x00, 0xff, 0x00, 0xffff, 0x0000);
			}
			else
			{
				if(Features_GetFeatureMode() == FeatureModeOptions_ProgramPlaying)
				{
					OfflineMode_GetNextFrame(nextFrame);
				}
				else
				{
					nextFrame->MillisecondsHold = 0x00ff;
				}
			}
			break;

		case ApplicationMode_UsbErrorState:
			// Usb connection error mode
			LedDriver_RenderOneColorFrame(0xff, 0x00, 0x00, 0xffff, 0x0000);
			break;

		case ApplicationMode_GenericErrorState:
			// Indicate some other error mode
			LedDriver_RenderOneColorFrame(0xff, 0x00, 0x00, 0x5555, 0x0000);
			break;
			
		case ApplicationMode_UsbOffline:
		default:
			OfflineMode_GetNextFrame(nextFrame);
			break;
	}
}


/************************************************************************/
/* USB EVENT HANDLERS and CALLBACKS                                     */
/************************************************************************/

/** Event handler for the library USB Connection event. */
void EVENT_USB_Device_Connect(void)
{
	ApplicationMode = ApplicationMode_UsbConnected;
}

/** Event handler for the library USB Disconnection event. */
void EVENT_USB_Device_Disconnect(void)
{
	ApplicationMode = ApplicationMode_UsbOffline;
}

/** Event handler for the library USB Configuration Changed event. */
void EVENT_USB_Device_ConfigurationChanged(void)
{
	bool ConfigSuccess = true;
	ConfigSuccess &= HID_Device_ConfigureEndpoints(&Generic_HID_Interface);

	USB_Device_EnableSOFEvents();

	if(ConfigSuccess)
	{
		ApplicationMode = ApplicationMode_UsbActive;
	}
	else
	{
		ApplicationMode = ApplicationMode_UsbErrorState;
	}
}

/** Event handler for the library USB Control Request reception event. */
void EVENT_USB_Device_ControlRequest(void)
{
	HID_Device_ProcessControlRequest(&Generic_HID_Interface);
}

/** Event handler for the USB device Start Of Frame event. */
void EVENT_USB_Device_StartOfFrame(void)
{
	HID_Device_MillisecondElapsed(&Generic_HID_Interface);
}

/** HID class driver callback function for the creation of HID reports to the host.
 *
 *  \param[in]     HIDInterfaceInfo  Pointer to the HID class interface configuration structure being referenced
 *  \param[in,out] ReportID    Report ID requested by the host if non-zero, otherwise callback should set to the generated report ID
 *  \param[in]     ReportType  Type of the report to create, either HID_REPORT_ITEM_In or HID_REPORT_ITEM_Feature
 *  \param[out]    ReportData  Pointer to a buffer where the created report should be stored
 *  \param[out]    ReportSize  Number of bytes written in the report (or zero if no report is to be sent)
 *
 *  \return Boolean true to force the sending of the report, false to let the library determine if it needs to be sent
 */
bool CALLBACK_HID_Device_CreateHIDReport(USB_ClassInfo_HID_Device_t* const HIDInterfaceInfo,
        uint8_t* const ReportID,
        const uint8_t ReportType,
        void* ReportData,
        uint16_t* const ReportSize)
{
	if(ReportType == HID_REPORT_ITEM_Feature)
	{
		*ReportSize = Features_CreateReport(ReportData);
		return true;
	}
	else
	{
		TempDriver_TemperatureData tempData = (TempDriver_TemperatureData){.RawData = 0xffff};
		TempDriver_GetTempDataStructure(&tempData);
		
		GlowBean_OutputReport *inputReport = (GlowBean_OutputReport*)ReportData;
		
		inputReport->ButtonSetOne = ButtonPressedMask;
		inputReport->TempDataHigh = tempData.RawDataHigh;
		inputReport->TempDataLow = tempData.RawDataLow;
				
		*ReportSize = sizeof(USB_KeyboardReport_Data_t);

		if((ButtonPressedMask != LastButtonPressedMask) || (tempData.RawData != LastTempDataRaw))
		{
			LastButtonPressedMask = ButtonPressedMask;
			LastTempDataRaw = tempData.RawData;
			
			return true;
		}
	}
	
	return false;
}

/** HID class driver callback function for the processing of HID reports from the host.
 *
 *  \param[in] HIDInterfaceInfo  Pointer to the HID class interface configuration structure being referenced
 *  \param[in] ReportID    Report ID of the received report from the host
 *  \param[in] ReportType  The type of report that the host has sent, either HID_REPORT_ITEM_Out or HID_REPORT_ITEM_Feature
 *  \param[in] ReportData  Pointer to a buffer where the received report has been stored
 *  \param[in] ReportSize  Size in bytes of the received HID report
 */

void CALLBACK_HID_Device_ProcessHIDReport(USB_ClassInfo_HID_Device_t* const HIDInterfaceInfo,
        const uint8_t ReportID,
        const uint8_t ReportType,
        const void* ReportData,
        const uint16_t ReportSize)
{
	if(ReportType == HID_REPORT_ITEM_Feature)
	{
		Features_ProcessReport(ReportData);
	}
	else
	{
		Features_ModeOptions mode = Features_GetFeatureMode();
		Instructions_Instruction* instruction;
		
		switch(mode)
		{
			case FeatureModeOptions_PlayStoredProgram:
				break;

			case FeatureModeOptions_StoreProgramInProgress:
				Features_StoreInstructionData(ReportData);
				// break; DEBUG: Falling through allows visualization of feature storage data
				
			case FeatureModeOptions_RenderLiveFrameData:
			default:
				LedDriver_RenderFrame(ReportData);
				break;
		}
	}
}