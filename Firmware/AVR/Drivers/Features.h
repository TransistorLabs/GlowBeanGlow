/*
 * Created: 1/20/2013 9:55:50 PM
 *  Author: paul trandem
 *  Copyright (c) 2013 Paul Trandem
 */ 


#ifndef FEATURES_H_
#define FEATURES_H_

	#include <avr/io.h>
	#include "Instructions.h"

	/* Enums */
	
	typedef enum
	{
		SetFeatureCommand_SetStaticColor = 0x00,
		SetFeatureCommand_ChangeFeatureMode = 0x01,
		
		// Assert Begin Marker - not a valid value
		SetFeatureCommand_End,
	} SetFeatureCommand;

	// If the application is in UsbActive state, the SetFeature USB commands will configure the mode,
	// which determines how USB OUTPUT reports are handled
	typedef enum
	{
		// In this mode, usb output reports will set the frame data directly to the display
		FeatureModeOptions_RenderLiveFrameData = 0x00,
		
		// In this mode, usb output reports will trigger the reset of an index and storage of future frame data
		FeatureModeOptions_StoreProgramStart = 0x01,
		
		// In this mode, the device will write out the total frame count to memory, then automatically switch to mode RenderCurrentFrameData
		FeatureModeOptions_StoreProgramStop = 0x02,
		
		// In this mode, the stored program can be played while the device is connected to a usb host
		FeatureModeOptions_PlayStoredProgram = 0x03,

		// Assert End Marker - not a valid value
		FeatureModeOptions_End,
		
		
		// These modes should not be set directly:
		FeatureModeOptions_ProgramPlaying = 0xfe,
		// This mode is automatically entered after StoreAnimationFrameStart
		FeatureModeOptions_StoreProgramInProgress = 0xff 
	} Features_ModeOptions;

	/* Function Prototypes */

	void Features_Init(void);
	Features_ModeOptions Features_GetFeatureMode(void);
	void Features_ProcessReport(const void const *reportData);
	uint16_t Features_CreateReport(void const *reportData);
	void Features_StoreInstructionData(const Instructions_Instruction * const frameData);

#endif /* FEATURES_H_ */