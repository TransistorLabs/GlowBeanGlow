/*
 * Storage.c
 *
 * Created: 1/19/2013 8:29:39 AM
 *  Author: Paul Trandem
 */ 

#include "Storage.h"
#include <avr/eeprom.h>

// Define EEPROM locations
uint8_t STORAGEMEMORY StorageConfig_StaticRed;
uint8_t STORAGEMEMORY StorageConfig_StaticGreen;
uint8_t STORAGEMEMORY StorageConfig_StaticBlue;
uint16_t STORAGEMEMORY StorageConfig_InstructionCount;
uint8_t STORAGEMEMORY StorageConfig_Reserved;
Instructions_Instruction STORAGEMEMORY StorageConfig_Instructions[STORAGE_MAXINSTRUCTIONS];
	
void Storage_GetSettings(Storage_SettingsResponse * const settingsData)
{
	uint8_t red, green, blue;
	red = eeprom_read_byte(&StorageConfig_StaticRed);
	green = eeprom_read_byte(&StorageConfig_StaticGreen);
	blue = eeprom_read_byte(&StorageConfig_StaticBlue);
	
	settingsData->StaticRed		= red;
	settingsData->StaticGreen	= green;
	settingsData->StaticBlue	= blue;
	settingsData->MaxInstructions = STORAGE_MAXINSTRUCTIONS;
}

void Storage_SetInstructionCount(uint16_t instructionCount)
{
	eeprom_update_word(&StorageConfig_InstructionCount, instructionCount);
}

uint16_t Storage_GetInstructionCount(void)
{
	return eeprom_read_word(&StorageConfig_InstructionCount);
}

// Write an instruction to a specific location in storage, specified by Index
void Storage_WriteInstruction(const Instructions_Instruction * const instruction, const int index)
{
	eeprom_update_block((void *)instruction, &StorageConfig_Instructions[index], sizeof(Instructions_Instruction));
}

// Gets a specific instruction from storage based on an index
void Storage_GetInstruction(uint16_t index, Instructions_Instruction * const instructionData)
{
	Instructions_Instruction data;
	eeprom_read_block((void *) &data, &StorageConfig_Instructions[index], sizeof(Instructions_Instruction));
	*instructionData = data;
}

void Storage_SetStaticColor(const uint8_t red, const uint8_t green, const uint8_t blue)
{
	eeprom_write_byte(&StorageConfig_StaticRed, red);
	eeprom_write_byte(&StorageConfig_StaticGreen, green);
	eeprom_write_byte(&StorageConfig_StaticBlue, blue);
}
