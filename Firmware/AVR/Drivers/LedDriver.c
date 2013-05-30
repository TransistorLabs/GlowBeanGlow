/*
 * This file contains the functionality to drive LEDs
 *
 * Created: 1/9/2013 7:27:22 PM
 *  Author: paul trandem
 *  Copyright (c) 2013 Paul Trandem
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
		LED8PIN,    LED9PIN,    LED10PIN,	LED11PIN};
			
volatile static uint8_t *LedConfig_Ports[PORTCONFIG_LEDCOUNT] 
	= {	LED0PORT,   LED1PORT,   LED2PORT,   LED3PORT,   
		LED4PORT,   LED5PORT,   LED6PORT,   LED7PORT,   
		LED8PORT,   LED9PORT,   LED10PORT,	LED11PORT};
		
static LedDriver_OneColorFrame currentFrame;
static LedDriver_FullColorFrame workingFullFrame;
static LedDriver_FullColorFrame currentFullFrame;
static LedDriver_FrameType currentFrameType;

volatile static uint8_t *lastPort =	LED0PORT;
static uint8_t  lastPin =	LED0PIN;
static void (*LedDriver_CALLBACK_GetNextFrame)(LedDriver_OneColorFrame * const nextFrame);
static bool millisecondTimer_Flag = false;


/************************************************************************/
/* PRIVATE FUNCTION DECLARATIONS                                        */
/************************************************************************/
static void ProcessMillisecondTask(void);


/************************************************************************/
/* PUBLIC FUNCTIONS                                                     */
/************************************************************************/

// Driver initialization
void LedDriver_Init(void (*CALLBACK_GetNextFrame)(LedDriver_OneColorFrame * const nextFrame))
{
	LedDriver_CALLBACK_GetNextFrame = CALLBACK_GetNextFrame;
	PortConfig_InitHardware();
}

// Main task; should be called often, i.e. in a main loop
bool LedDriver_Task(void)
{
	uint8_t i;
	if(currentFrameType == LedFrameType_Frame)
	{
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
	}
	else
	{
		// Render full color frame
		for(i = 0; i < PORTCONFIG_LEDCOUNT; i++)
		{
			uint16_t bit = currentFullFrame.LedState.RawData;
			
			RED	= currentFullFrame.RGB[i][0];
			GRN = currentFullFrame.RGB[i][1];
			BLU = currentFullFrame.RGB[i][2];

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


void LedDriver_RenderFrame(const LedDriver_Frame * const frameData)
{
	LedDriver_OneColorFrame *frame;
	LedDriver_FullColorFramePart *framePart;
	LedDriver_FullColorFramePartLast *lastFramePart;
	
	uint8_t pageIndex;
	
	switch(frameData->FrameType)
	{
		case LedFrameType_Frame:
			frame = (LedDriver_OneColorFrame *) frameData;
			currentFrame = *frame;
			currentFrameType = LedFrameType_Frame;
			break;
		
		case LedFrameType_FullColorFramePart:
			{
				framePart = (LedDriver_FullColorFramePart*)frameData;
				
				if(framePart->FramePage > 4)
				{ 
					// invalid frame page; bail.
					// TODO: this should be derived from PORTCONFIG_LEDCOUNT at some point, and not hardcoded.
					return;
				}
				pageIndex = framePart->FramePage * 2;	
				workingFullFrame.RGB[pageIndex][0] = framePart->RGBA[0];
				workingFullFrame.RGB[pageIndex][1] = framePart->RGBA[1];
				workingFullFrame.RGB[pageIndex][2] = framePart->RGBA[2];
				
				++pageIndex;
				workingFullFrame.RGB[pageIndex][0] = framePart->RGBB[0];
				workingFullFrame.RGB[pageIndex][1] = framePart->RGBB[1];
				workingFullFrame.RGB[pageIndex][2] = framePart->RGBB[2];
			}			
			break;
			
		case LedFrameType_FullColorLastFramePart:
			{
				lastFramePart = (LedDriver_FullColorFramePartLast*) frameData;
				workingFullFrame.RGB[10][0] = lastFramePart->RGBA[0];
				workingFullFrame.RGB[10][1] = lastFramePart->RGBA[1];
				workingFullFrame.RGB[10][2] = lastFramePart->RGBA[2];
				workingFullFrame.LedState.RawData = lastFramePart->LedState.RawData;
				currentFullFrame = workingFullFrame;
				currentFrameType = LedFrameType_FullColorFramePart;
			}			
			break;
			
		
	}
}

static uint8_t fadeRedIncrement = 0x00;
static uint8_t fadeGreenIncrement = 0x00;
static uint8_t fadeBlueIncrement = 0x00;
static uint8_t fadeCount = 0x00;

void LedDriver_FadeToColor(uint8_t red, uint8_t green, uint8_t blue, uint8_t durationInMs)
{
	int8_t redDistance, greenDistance, blueDistance = 0x00;
	redDistance		= red - currentFrame.Red;
	greenDistance	= green - currentFrame.Green;
	blueDistance	= blue - currentFrame.Blue;
	
	fadeRedIncrement	= redDistance / durationInMs;
	fadeGreenIncrement	= greenDistance / durationInMs;
	fadeBlueIncrement	= blueDistance / durationInMs;
	
	fadeCount = durationInMs;
}

// Can be used to directly override frame data
void LedDriver_RenderOneColorFrame(uint8_t red, uint8_t green, uint8_t blue, uint16_t ledsMask, uint16_t millisecondHold)
{
	currentFrame.Red = red;
	currentFrame.Green = green;
	currentFrame.Blue = blue;
	currentFrame.LedState.RawData = ledsMask;
	currentFrame.MillisecondsHold = millisecondHold & 0xffff;
	currentFrameType = LedFrameType_Frame;
}

void LedDriver_Clear(void)
{
	currentFrame = (LedDriver_OneColorFrame) {
		.Red = 0x00,
		.Blue = 0x00,
		.Green = 0x00,
		.LedState = (LedDriver_LedState) { .RawData = 0x0000 },
		.MillisecondsHold = 0x0000,
		.FrameType = LedFrameType_Frame
	};
	currentFrameType = LedFrameType_Frame;
}

void LedDriver_TestWhite(void)
{
	currentFrame = (LedDriver_OneColorFrame) {
		.Red = 0xff,
		.Blue = 0xff,
		.Green = 0xff,
		.LedState = (LedDriver_LedState) { .RawData = 0xffff },
		.MillisecondsHold = 0x0000,
		.FrameType = LedFrameType_Frame
	};
	currentFrameType = LedFrameType_Frame;
}

void LedDriver_TestColor(uint8_t red, uint8_t green, uint8_t blue)
{
	currentFrame = (LedDriver_OneColorFrame) {
		.Red = red,
		.Blue = green,
		.Green = blue,
		.LedState = (LedDriver_LedState) { .RawData = 0xffff },
		.MillisecondsHold = 0x0000,
		.FrameType = LedFrameType_Frame
	};
	currentFrameType = LedFrameType_Frame;
}

/************************************************************************/
/* PRIVATE FUNCTIONS                                                    */
/************************************************************************/

static void ProcessMillisecondTask(void)
{
	if(millisecondTimer_Flag)
	{
		//TODO: check frame type and handle ms hold for fullcolorframe type
		millisecondTimer_Flag = false;
		
		if(currentFrameType == LedFrameType_Frame)
		{
			if(fadeCount > 0)
			{
				// handle fades
				--fadeCount;
				currentFrame.Red += fadeRedIncrement;
				currentFrame.Green += fadeGreenIncrement;
				currentFrame.Blue += fadeBlueIncrement;
			}
			else
			{
				// otherwise, normal keyframe-style stuff
				if(!currentFrame.MillisecondsHold)
				{
					if(!LedDriver_CALLBACK_GetNextFrame)
					{
						return;
					}
					LedDriver_CALLBACK_GetNextFrame(&currentFrame);
				}
				else
				{
					currentFrame.MillisecondsHold--;
				}	
			}
		}
	}
}