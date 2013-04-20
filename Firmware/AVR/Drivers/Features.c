/*
 * Created: 1/20/2013 9:56:02 PM
 *  Author: paul trandem
 *  Copyright (c) 2013 Paul Trandem
 */ 

#include "Features.h"
#include "Storage.h"

/* Structures */
typedef struct {
	uint8_t Command;
	uint8_t Status;
	uint8_t CommandData[6];
} Features_SetFeatureReport;

/* Function Prototypes */
static void ManageFeatureModeState(void);

/* Private variables */
static Features_ModeOptions currentFeatureMode;
static int16_t currentInstructionIndex = -1;


void Features_Init(void)
{
	currentFeatureMode = (Features_ModeOptions)FeatureModeOptions_RenderLiveFrameData;
}

void Features_ProcessReport(void const *reportData)
{
	Features_SetFeatureReport  *featureReport = (Features_SetFeatureReport *)reportData;
	
	switch(featureReport->Command)
	{
		case SetFeatureCommand_ChangeFeatureMode:
		{
			// Verify that the command being provided is valid
			if(featureReport->CommandData[0] < FeatureModeOptions_End)
			{
				currentFeatureMode = (Features_ModeOptions) featureReport->CommandData[0];
				ManageFeatureModeState();
			}
		}
		break;
		
		case SetFeatureCommand_SetStaticColor:
			{
				uint8_t red = featureReport->CommandData[0];
				uint8_t green = featureReport->CommandData[1];
				uint8_t blue = featureReport->CommandData[2];
				Storage_SetStaticColor(red, green, blue);
			}			
			break;
					
		default:
			break;
	}
}

uint16_t Features_CreateReport(void const *reportData)
{
	Storage_SettingsResponse *deviceSettings = (Storage_SettingsResponse *) reportData;
	Storage_GetSettings(deviceSettings);
	return sizeof(Storage_SettingsResponse);
}

Features_ModeOptions Features_GetFeatureMode(void)
{
	return currentFeatureMode;
}

// This function should be called with report data when 
// an OUTPUT report happens while the device is 
// in the StoreProgramInProgress mode
void Features_StoreInstructionData(const Instructions_Instruction * const instructionData)
{
	// Ensure we are in the right mode to actually process this data
	if(currentFeatureMode == FeatureModeOptions_StoreProgramInProgress)
	{
		++currentInstructionIndex; // Update the frame index
		
		Instructions_Instruction instruction;
		instruction = *instructionData;
	
		Storage_WriteInstruction(&instruction, currentInstructionIndex);
	}		
}

// Manage feature mode state machine
static void ManageFeatureModeState(void)
{
	
	switch(currentFeatureMode)
	{
		
		case FeatureModeOptions_StoreProgramStart:
			// reset index counter
			currentInstructionIndex = -1; 
			// Transition to InProgress mode and wait for data
			currentFeatureMode = FeatureModeOptions_StoreProgramInProgress;
			break;
		
			
		case FeatureModeOptions_StoreProgramStop:
			{
				// Store the count for the animation program
				Storage_SetInstructionCount(currentInstructionIndex);

				// Reset to the normal, "non-feature" render mode
				currentFeatureMode = FeatureModeOptions_RenderLiveFrameData;
			}			
			break;
		
			
		case FeatureModeOptions_StoreProgramInProgress:
		case FeatureModeOptions_RenderLiveFrameData:
		default:
			// do nothing
			break;
	}
}