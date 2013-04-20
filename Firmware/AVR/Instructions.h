/*
 * Created: 3/10/2013 8:21:38 PM
 *  Author: paul trandem
 *  Copyright (c) 2013 Paul Trandem
 */ 


#ifndef INSTRUCTIONS_H_
#define INSTRUCTIONS_H_

	#include "Drivers/LedDriver.h"

	/* Enums */
	typedef enum {
		InstructionType_SetFrame,		//0x00
		InstructionType_IncrementFrame,	//0x01
		InstructionType_Condition,		//0x02
		InstructionType_Jump,			//0x03

		// Assert End Marker - not a valid value
		InstructionType_End
	} Instructions_InstructionTypes;

	typedef enum {
		Instructions_ShiftLedLeft,		//0x00
		Instructions_ShiftLedRight,		//0x01
	} Instructions_LedShiftOptions;

/************************************************************************/
/* Data Structures                                                      */
/************************************************************************/

	// Set Frame Instruction (instruction type 0x00)
	typedef struct {
		int8_t Red;
		int8_t Green;
		int8_t Blue;
		LedDriver_LedState LedState;
		uint16_t MillisecondsHold;
		uint8_t InstructionType	: 4,
				Reserved		: 4;
	} Instructions_SetFrame;

	// Increment Frame Instruction (instruction type: 0x01)
	// Ri Gi Bi Cd Cc Ld Lc Lst IT
	typedef struct
	{
		// Color increment data
		int8_t RedIncrement;
		int8_t GreenIncrement;
		int8_t BlueIncrement;
		uint8_t ColorIncrementDelayMs;
		
		// If a count of 0, then no change will be made to color values
		uint8_t ColorIncrementCount;

		// Led shift data
		uint8_t LedShiftDelayMs;
		
		// If a count of 0, then no shifts will be made to LEDs
		uint8_t LedShiftCount;

		// Led Shift types:
		//  0x00 - <<
		//  0x01 - >>
		
		uint8_t InstructionType 	: 4,
				LedShiftType 		: 2,
				Reserved		 	: 2;
	} Instructions_IncrementFrame;
	
	typedef struct
	{
		uint16_t TargetIndex;
		uint8_t Reserved[5];	// TODO Add conditions here?
		uint8_t InstructionType : 4,
				ReservedBits	: 4;
	} Instructions_JumpTo;

	typedef union
	{
		Instructions_SetFrame SetFrame;
		Instructions_IncrementFrame IncrementFrame;
		Instructions_JumpTo JumpTo;
		struct
		{
			// This anonymous struct is used to 
			// expose global instruction stuff, like the type
			uint8_t Reserved[7];
			uint8_t InstructionType : 4,
					ReservedBits	: 4;
		};
	} Instructions_Instruction;


#endif /* INSTRUCTIONS_H_ */