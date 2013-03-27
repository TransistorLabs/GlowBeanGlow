/*
 * TempDriver.c
 *
 * Created: 2/13/2013 9:07:23 PM
 *  Author: Paul Trandem
 */ 

#include "TempDriver.h"
#include "TransistorLabs/avr.h"
#include "Config/HardwareConfig.h"

#include <avr/interrupt.h>
#include <stdbool.h>

// The TempReadDelayMs defines the number of milliseconds between
// actual sensor reads.  The data itself is "cached" for retrieval by
// the GetTemp___() functions.

// The conversion time for the LM74 is typically 280ms, with a max of 425ms
// Initial value for the counter is 12c, or 300 ms nominal, 


const static uint16_t TempReadDelayMs = 0x0001;
static volatile uint16_t readDelayMsCounter = 0x0001;
static volatile bool shouldReadTemp = true;

static TempDriver_TemperatureData currentTempData;

// initialize to 21 degrees c, or ~70 degrees f
static uint8_t currentTempC = 0x15;

void TempDriver_Init(void)
{
	// SPI Port Configuration
	SETPINDIRECTION_OUTPUT(SPI_PORTDIRECTION, SPI_MOSI);
	SETPINDIRECTION_OUTPUT(SPI_PORTDIRECTION, SPI_SCK);
	SETPINDIRECTION_OUTPUT(TEMP_CHIPSELECT_DIRECTION, TEMP_CHIPSELECT);
	
	
	EnableSPIMaster_Div16();
	//SPI_SetCPOLHigh();
}

void TempDriver_MillisecondTask(void)
{
	--readDelayMsCounter;
	if(readDelayMsCounter == 0x00)
	{
		readDelayMsCounter = TempReadDelayMs;
		shouldReadTemp = true;
	}
}

void TempDriver_Task(void)
{
	if(shouldReadTemp)
	{
		// Note: actual outgoing data doesn't matter, this just triggers
		//	the transaction. The LM74 ignores the incoming data during
		//	the first two bytes of communication
		// We'll just send 0x00, since in theory it saves (some?) current
		// and probably won't induce noise
		uint8_t highByte;
		uint8_t lowByte;
		
		Temp_SelectChip();
		
		SPI_DATA = 0x00;
		WaitForSPI_XferComplete();
		highByte = SPI_DATA;
		
		SPI_DATA = 0x00;
		WaitForSPI_XferComplete();
		lowByte = SPI_DATA;
		
		Temp_DeselectChip();
		
		currentTempData.RawDataHigh = highByte;
		currentTempData.RawDataLow = lowByte;
		
		if(currentTempData.ValidData)
		{
			float tc = currentTempData.WholeNumberData;
			currentTempC = tc;
		}
		
		
		shouldReadTemp = false;
	}
}

void TempDriver_GetTempDataStructure(TempDriver_TemperatureData * const tempData)
{
	*tempData = currentTempData;
}

uint8_t TempDriver_GetTempC(void)
{
	return currentTempC;
}
