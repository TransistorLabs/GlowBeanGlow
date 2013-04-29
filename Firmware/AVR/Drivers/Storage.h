/*
 * Created: 1/19/2013 8:29:22 AM
 *  Author: paul trandem
 *  Copyright (c) 2013 Paul Trandem
 */ 


#ifndef STORAGE_H_
#define STORAGE_H_

	#include "Config/HardwareConfig.h"
	#include "Instructions.h"
	
	// ((storage bytes - storage header bytes) / frame size) | ((512-8)/8) = 63
	#define STORAGE_MAXINSTRUCTIONS	(uint16_t)((STORAGECONFIG_BYTES - 8) / 8)
	
	
	/** Structures **/
	
	typedef struct 
	{
		uint8_t StaticRed;
		uint8_t StaticGreen;
		uint8_t StaticBlue;
		uint16_t TempDeviceId;
		uint16_t MaxInstructions;
		uint8_t ReservedA;
	} Storage_SettingsResponse;
	
			
	/** Function Prototypes **/
	// Returns the number of bytes available for storage
	//uint16_t Storage_GetBufferLength(void);
	
	// Gets the current static and max instruction settings
	void Storage_GetSettings(Storage_SettingsResponse * const settingsData);
	
	// Sets the static color
	void Storage_SetStaticColor(const uint8_t red, const uint8_t green, const uint8_t blue);
	
	void Storage_SetInstructionCount(uint16_t instructionCount);
	
	uint16_t Storage_GetInstructionCount(void);
	
	// Gets a specific instruction from storage based on an index
	void Storage_GetInstruction(uint16_t index, Instructions_Instruction * const instructionData);

	// Write an instruction to a specific location in storage, specified by FrameIndex
	void Storage_WriteInstruction(const Instructions_Instruction * const instruction, const int index);

#endif /* STORAGE_H_ */