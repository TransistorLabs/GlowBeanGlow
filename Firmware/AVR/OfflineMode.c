/*
 * Created: 2/3/2013 1:36:03 PM
 *  Author: paul trandem
 *  Copyright (c) 2013 Paul Trandem
 */ 

#include "OfflineMode.h"
#include "Instructions.h"
#include "Drivers/Storage.h"
#include "Drivers/TempDriver.h"
#include "TransistorLabs/common.h"
#include <stdlib.h>

#define NOFRAMESET	0xFFFF

static OfflineMode_ModeOptions OfflineMode = OfflineMode_Static;
static volatile Instructions_Instruction currentInstruction;

static void InitCurrentMode(void);
static void GetNextFrame_StaticMode(LedDriver_OneColorFrame * const frameData);
static void GetNextFrame_TempMode(LedDriver_OneColorFrame * const frameData);
static void GetNextFrame_CycleMode(LedDriver_OneColorFrame * const frameData);
static void GetNextFrame_AnimateMode(LedDriver_OneColorFrame * const frameData);
static void GetNextFrame_OffMode(LedDriver_OneColorFrame * const frameData);

static uint16_t animationMode_TotalInstructionCount = 0;
static uint16_t animationMode_InstructionProgramCounter = 0;
static uint16_t animateMode_ButtonPressFrameIndex = NOFRAMESET;

static uint8_t staticModeRed = 0xff;
static uint8_t staticModeGreen = 0xff;
static uint8_t staticModeBlue = 0xff;

static uint8_t colorIncrementMsRemaining;
static uint8_t ledShiftMsRemaining;

static uint8_t cycleModeRed		= 0x00;
static uint8_t cycleModeGreen	= 0x00;
static uint8_t cycleModeBlue	= 0x00;
static uint8_t cycleModeState	= 0x00;

static void (*GetNextFrame[NUMOFFLINEMODES])(LedDriver_OneColorFrame * const frameData) = {
	GetNextFrame_StaticMode, 
	GetNextFrame_TempMode,
	GetNextFrame_CycleMode, 
	GetNextFrame_AnimateMode, 
	GetNextFrame_OffMode};


void OfflineMode_Init(void)
{
	InitCurrentMode();
}

void OfflineMode_GetNextFrame(LedDriver_OneColorFrame * const frameData )
{
	GetNextFrame[OfflineMode](frameData);
}

void OfflineMode_SetNextOfflineMode(void)
{
	if(OfflineMode == OfflineMode_Off)
	{
		OfflineMode = OfflineMode_Static;
	}
	else
	{
		++OfflineMode;
	}
	
	InitCurrentMode();
}

void OfflineMode_StartProgramPlay(void)
{
	OfflineMode = OfflineMode_Animate;
	InitCurrentMode();
}


static void InitCurrentMode(void)
{
	// Get settings from storage
	Storage_SettingsResponse settings;
	Storage_GetSettings(&settings);

	// Handle setup tasks for the new mode
	switch(OfflineMode)
	{
		case OfflineMode_Animate:
		{
			//Reset the program counter and increment/shift counters
			animationMode_InstructionProgramCounter = 0;
			colorIncrementMsRemaining = 0;
			ledShiftMsRemaining = 0;
			currentInstruction.IncrementFrame.ColorIncrementCount = 0;
			currentInstruction.IncrementFrame.LedShiftCount = 0;
			
			// we've just switched into this mode, get the frameCount
			animationMode_TotalInstructionCount = Storage_GetInstructionCount();
			if(animationMode_TotalInstructionCount > STORAGE_MAXINSTRUCTIONS)
			{
				// We'll get here in the scenario where eeprom is blank, or eeprom write errors have happened
				animationMode_TotalInstructionCount = 0;
			}
		}
		break;
		
		case OfflineMode_Static:
			staticModeRed	= settings.StaticRed;
			staticModeGreen = settings.StaticGreen;
			staticModeBlue	= settings.StaticBlue;
			break;
		
		case OfflineMode_Cycle:
			cycleModeRed	= 0x00;
			cycleModeGreen	= 0x00;
			cycleModeBlue	= 0x00;
			cycleModeState	= 0x00;
			break;

		default:
			break;
	}
}

static void GetNextFrame_StaticMode(LedDriver_OneColorFrame * const frameData)
{
	// Set the frame data
	//frameData->Red		= 0x00;
	//frameData->Green	= 0x00;
	//frameData->Blue		= 0x00;
	frameData->LedState.RawData = 0xffff;
	//frameData->MillisecondsHold = 0x00ff;
	LedDriver_FadeToColor(staticModeRed, staticModeGreen, staticModeBlue, 0xffff, 0x00ff);
}

static uint8_t currentTempRed = 0;
static uint8_t currentTempGreen = 127;
static uint8_t currentTempBlue = 0;
static float nominalTemp = 70;
static void GetNextFrame_TempMode(LedDriver_OneColorFrame * const frameData)
{
	float f = 70;
	f = TempDriver_GetTempF();
	
	float delta = f - nominalTemp;
	float red = 0;
	float blue = 0;
	if(delta > 0)
	{
		red = 0 + (delta * 20);
		if(red > 255) red = 255;
	}
	
	if(delta < 0)
	{
		blue = 0 + ((delta * -1) * 20);
		if(blue > 255) blue = 255;
	}
	
	float green = 127 - abs(delta) * 20;
	if(green < 0) green = 0;
	
	if(red < currentTempRed) --currentTempRed;
	if(green < currentTempGreen) --currentTempGreen;
	if(blue < currentTempBlue) --currentTempBlue;
	
	if(red > currentTempRed) ++currentTempRed;
	if(green > currentTempGreen) ++currentTempGreen;
	if(blue > currentTempBlue) ++currentTempBlue;
	
	frameData->Red		= (uint8_t)currentTempRed;
	frameData->Green	= (uint8_t)currentTempGreen;
	frameData->Blue		= (uint8_t)currentTempBlue;
	
	frameData->LedState.RawData = 0xffff;
	frameData->MillisecondsHold = 0x0008;
}

static void GetNextFrame_PulseMode(LedDriver_OneColorFrame * const frameData)
{
	//TODO: ditch pulsemode for being kinda stupid?
	//or fix this pizza, cuz it be broke (kinda.)
	
	//if(pulseFrameIndex == pulseModeTargetHigh)
	//{
		//pulseVelocity = -pulseVelocity;
		//pulseModeDelayIndex = 0x02;
	//}
	//else if(pulseFrameIndex == pulseModeTargetLow)
	//{
		//pulseVelocity		= -pulseVelocity;
		//pulseFrameIndex		= pulseModeTargetLow;
		//pulseModeDelayIndex = 0x08;
	//}
	//
	//pulseFrameIndex += pulseVelocity;
	//
//
	//frameData->Red		= (pulseModeRed <= pulseModeDeadValueThreshold)		? pulseModeRed		: (pulseModeRed - pulseFrameIndex);
	//frameData->Green	= (pulseModeGreen <= pulseModeDeadValueThreshold)	? pulseModeGreen	: (pulseModeGreen - pulseFrameIndex);
	//frameData->Blue		= (pulseModeBlue <= pulseModeDeadValueThreshold)	? pulseModeBlue		: (pulseModeBlue - pulseFrameIndex);
	//frameData->LedState.RawData = 0xffff;
	//frameData->MillisecondsHold = pulseModeDelayIndex;
	//
}

static void GetNextFrame_CycleMode(LedDriver_OneColorFrame * const frameData)
{
	switch(cycleModeState)
	{
		case 0:
			if(cycleModeRed < 0xff) { cycleModeRed++; }
			else { cycleModeState = 1; }
			break;
			
		case 1:
			if(cycleModeGreen < 0xff) { cycleModeGreen++; }
			else { cycleModeState = 2; }			
			break;
			
		case 2:
			if(cycleModeRed > 0x00) { cycleModeRed--; }
			else { cycleModeState = 3; }			
			break;
			
		case 3:
			if(cycleModeBlue < 0xff) { cycleModeBlue++; } 
			else { cycleModeState = 4; }			
			break;
			
		case 4:
			if(cycleModeGreen > 0x00) { cycleModeGreen--; }
			else { cycleModeState = 5; }
			break;
			
		case 5:
			if(cycleModeRed < 0xff) { cycleModeRed++; }
			else { cycleModeState = 6; }
			break;
			
		case 6:
			if(cycleModeBlue > 0x00) { cycleModeBlue --; }
			else { cycleModeState = 1; }
			break;
	}
	
	frameData->Red = cycleModeRed;
	frameData->Green = cycleModeGreen;
	frameData->Blue = cycleModeBlue;
	frameData->LedState.RawData = 0xffff;
	frameData->MillisecondsHold = 0x0008;
}

static void GetNextFrame_AnimateMode(LedDriver_OneColorFrame * const frameData)
{
	// TODO: HANDLE INSTRUCTIONS
	if(animationMode_TotalInstructionCount == 0)
	{
		// if stored frame count is 0, then skip this mode
		OfflineMode = OfflineMode_Off;
	}
	else
	{
		bool instructionFinished = true;
		uint16_t currentTemp;
		
		// Perform mid-instruction animation processing (not relevant for SetFrame)
		switch(currentInstruction.InstructionType)
		{
			case InstructionType_IncrementFrame:
				if(currentInstruction.IncrementFrame.ColorIncrementCount > 0 || colorIncrementMsRemaining > 0)
				{
					instructionFinished = false;
					--colorIncrementMsRemaining;
					
					// Perform action only if we've hit the ms delay
					if(colorIncrementMsRemaining == 0 && currentInstruction.IncrementFrame.ColorIncrementCount > 0)
					{
						colorIncrementMsRemaining = currentInstruction.IncrementFrame.ColorIncrementDelayMs;
						--currentInstruction.IncrementFrame.ColorIncrementCount;
						int16_t red = frameData->Red;
						int16_t blue = frameData->Blue;
						int16_t green = frameData->Green;
						
						red += currentInstruction.IncrementFrame.RedIncrement;
						blue += currentInstruction.IncrementFrame.BlueIncrement;
						green += currentInstruction.IncrementFrame.GreenIncrement;
						

						// Boundary clamping based on increment direction
						if(currentInstruction.IncrementFrame.RedIncrement > 0 && red > 0x00ff)
						{
							red = 0x00ff;
						}
						
						if(currentInstruction.IncrementFrame.RedIncrement < 0 && red < 0x0000)
						{
							red = 0x0000;
						}
						
						if(currentInstruction.IncrementFrame.BlueIncrement > 0 && blue > 0x00ff)
						{
							blue = 0x00ff;
						}
						
						if(currentInstruction.IncrementFrame.BlueIncrement < 0 && blue < 0x0000)
						{
							blue = 0x0000;
						}
						
						if(currentInstruction.IncrementFrame.GreenIncrement > 0 && green > 0x00ff)
						{
							green = 0x00ff;
						}
						
						if(currentInstruction.IncrementFrame.GreenIncrement < 0 && green < 0x0000)
						{
							green = 0x0000;
						}
						
						frameData->Red = (uint8_t)red;
						frameData->Blue = (uint8_t)blue;
						frameData->Green = (uint8_t)green;
					}						
				}
				
				if(currentInstruction.IncrementFrame.LedShiftCount > 0 || ledShiftMsRemaining > 0)
				{
					instructionFinished = false;
					--ledShiftMsRemaining;
					
					// Perform action only if we've hit the ms delay
					if(ledShiftMsRemaining == 0 && currentInstruction.IncrementFrame.LedShiftCount > 0)
					{
						// Reset only if we've got more to count
						ledShiftMsRemaining = currentInstruction.IncrementFrame.LedShiftDelayMs;
						--currentInstruction.IncrementFrame.LedShiftCount;
												
						if(currentInstruction.IncrementFrame.LedShiftType == Instructions_ShiftLedLeft)
						{
							frameData->LedState.RawData = Rotate12BitsLeft(frameData->LedState.RawData);
						}							
						else
						{
							frameData->LedState.RawData = Rotate12BitsRight(frameData->LedState.RawData);
						}
					}
				}
				break;
			default:
				break;
		}
		
		
		//if we're done processing, then get the next instruction
		if(instructionFinished)
		{
			Storage_GetInstruction(animationMode_InstructionProgramCounter, &currentInstruction);

			switch(currentInstruction.InstructionType)
			{
				case InstructionType_SetFrame:
					frameData->Red		= currentInstruction.SetFrame.Red;
					frameData->Green	= currentInstruction.SetFrame.Green;
					frameData->Blue		= currentInstruction.SetFrame.Blue;
					frameData->LedState.RawData = currentInstruction.SetFrame.LedState.RawData;
					frameData->MillisecondsHold = currentInstruction.SetFrame.MillisecondsHold;
					break;
				
				case InstructionType_IncrementFrame:
					// Setup increment instruction logic
					colorIncrementMsRemaining = currentInstruction.IncrementFrame.ColorIncrementDelayMs;
					ledShiftMsRemaining = currentInstruction.IncrementFrame.LedShiftDelayMs;
					
					//TODO:ms hold should be 0ms or 1ms?
					frameData->MillisecondsHold = 0x0001;
					break;
					
				case InstructionType_ButtonCondition:
					animateMode_ButtonPressFrameIndex = currentInstruction.ButtonCondition.TargetIndex;
					frameData->MillisecondsHold = 0x0000;
					break;
					
				case InstructionType_TempCondition:
					{
						currentTemp = (uint16_t)(TempDriver_GetTempF() + 0.5);
						if(currentTemp >= currentInstruction.TempCondition.LowTempF 
							&& currentTemp <= currentInstruction.TempCondition.HighTempF)
						{
							animationMode_InstructionProgramCounter = currentInstruction.TempCondition.TargetIndex - 1;
						}
						frameData->MillisecondsHold = 0x0000;
					}
					break;
				
				case InstructionType_Jump:
					animationMode_InstructionProgramCounter = currentInstruction.JumpTo.TargetIndex - 1;
					frameData->MillisecondsHold = 0x0000;
					break;
				default:
					break;
			}
		
			++animationMode_InstructionProgramCounter;
			if(animationMode_InstructionProgramCounter > animationMode_TotalInstructionCount)
			{
				// loop
				animationMode_InstructionProgramCounter = 0;
			}
		}			
	}
}

static void GetNextFrame_OffMode(LedDriver_OneColorFrame * const frameData)
{
	frameData->Red		= 0x00;
	frameData->Green	= 0x00;
	frameData->Blue		= 0x00;
	frameData->LedState.RawData = 0x0000;
	frameData->MillisecondsHold = 0x0000;
}

void OfflineMode_ProcessButtonPressUserA(void)
{
	if(OfflineMode == OfflineMode_Temp)
	{
		nominalTemp = TempDriver_GetTempF();
	}
	else if(OfflineMode == OfflineMode_Animate)
	{
		if(animateMode_ButtonPressFrameIndex != NOFRAMESET)
		{
			animationMode_InstructionProgramCounter = animateMode_ButtonPressFrameIndex;
		}
	}
}
