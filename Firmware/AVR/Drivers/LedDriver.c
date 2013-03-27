/*
 * LedDriver.c
	This file contains the functionality to drive LEDs
 *
 * Created: 1/9/2013 7:27:22 PM
 *  Author: Paul Trandem
 */ 

#include <util/delay.h>
#include "LedDriver.h"
#include "Config/HardwareConfig.h"

/************************************************************************/
/* PRIVATE VARIABLES                                                    */
/************************************************************************/

static const uint8_t LedConfig_Pins[PORTCONFIG_LEDCOUNT] 
	= {	LED0PIN,    LED1PIN,    LED2PIN,    LED3PIN,    
		LED4PIN,    LED5PIN,    LED6PIN,    LED7PIN,    
		LED8PIN,    LED9PIN,    LED10PIN};
			
volatile static uint8_t *LedConfig_Ports[PORTCONFIG_LEDCOUNT] 
	= {	LED0PORT,   LED1PORT,   LED2PORT,   LED3PORT,   
		LED4PORT,   LED5PORT,   LED6PORT,   LED7PORT,   
		LED8PORT,   LED9PORT,   LED10PORT};
		
static LedDriver_Frame currentFrame;
volatile static uint8_t *lastPort =	LED0PORT;
static uint8_t  lastPin =	LED0PIN;
static void (*LedDriver_CALLBACK_GetNextFrame)(LedDriver_Frame * const nextFrame);
static bool millisecondTimer_Flag = false;


/************************************************************************/
/* PRIVATE FUNCTION DECLARATIONS                                        */
/************************************************************************/
static void ProcessMillisecondTask(void);


/************************************************************************/
/* PUBLIC FUNCTIONS                                                     */
/************************************************************************/

// Driver initialization
void LedDriver_Init(void (*CALLBACK_GetNextFrame)(LedDriver_Frame * const nextFrame))
{
	LedDriver_CALLBACK_GetNextFrame = CALLBACK_GetNextFrame;
	PortConfig_InitHardware();
}

// Main task; should be called often, i.e. in a main loop
bool LedDriver_Task(void)
{
	uint8_t i;
	for(i = 0; i < PORTCONFIG_LEDCOUNT; i++)
	{
		uint16_t bit = currentFrame.LedState.RawData;
		RED	= currentFrame.Red;
		BLU = currentFrame.Blue;
		GRN = currentFrame.Green;

		bit &= _BV(i);
		if(bit != 0x00)
		{
			// Turn on the current LED
			*LedConfig_Ports[i] |= _BV(LedConfig_Pins[i]);
		}
		
		//turn off last port-pin combo
		*lastPort &= ~(_BV(lastPin));
				
		//Set new last port-pin combo
		lastPort = LedConfig_Ports[i];
		lastPin = LedConfig_Pins[i];

		// TODO: add some microsecond delay here or not?
		_delay_us(100);
	}
	
	//turn off final port-pin combo
	*lastPort &= ~(_BV(lastPin));

	ProcessMillisecondTask();

	return true;
}

// Typically called by a timer interrupt, so this function should be kept very short and very fast
void LedDriver_MillisecondTask(void)
{
	millisecondTimer_Flag = true;
}

// Can be used to directly override frame data
void LedDriver_RenderFrame(const LedDriver_Frame * const frameData)
{
	currentFrame = *frameData;
}

// Can be used to directly override frame data
void LedDriver_RenderKeyFrame(uint8_t red, uint8_t green, uint8_t blue, uint16_t ledsMask, uint16_t millisecondHold)
{
	currentFrame.Red = red;
	currentFrame.Green = green;
	currentFrame.Blue = blue;
	currentFrame.LedState.RawData = ledsMask;
	currentFrame.MillisecondsHold = millisecondHold & 0xffff;
}

void LedDriver_Clear(void)
{
	currentFrame = (LedDriver_Frame) {
		.Red = 0x00,
		.Blue = 0x00,
		.Green = 0x00,
		.LedState = (LedDriver_LedState) { .RawData = 0x0000 },
		.MillisecondsHold = 0x0000
	};
}

void LedDriver_TestWhite(void)
{
	currentFrame = (LedDriver_Frame) {
		.Red = 0xff,
		.Blue = 0xff,
		.Green = 0xff,
		.LedState = (LedDriver_LedState) { .RawData = 0xffff },
		.MillisecondsHold = 0x0000
	};
}

void LedDriver_TestColor(uint8_t red, uint8_t green, uint8_t blue)
{
	currentFrame = (LedDriver_Frame) {
		.Red = red,
		.Blue = green,
		.Green = blue,
		.LedState = (LedDriver_LedState) { .RawData = 0xffff },
		.MillisecondsHold = 0x0000
	};
}

/************************************************************************/
/* PRIVATE FUNCTIONS                                                    */
/************************************************************************/

static void ProcessMillisecondTask(void)
{
	if(millisecondTimer_Flag)
	{
		//
		millisecondTimer_Flag = false;
			
		if(!currentFrame.MillisecondsHold)
		{
			if(!LedDriver_CALLBACK_GetNextFrame)
			{
				//LedDriver_RenderFrameParts(0xff, 0x00, 0x00, 0xffff, 0xffff);
				//return;
			}
			LedDriver_CALLBACK_GetNextFrame(&currentFrame);
		}
		else
		{
			currentFrame.MillisecondsHold--;
		}
	}
}