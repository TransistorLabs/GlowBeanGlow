/*
 * Created: 2/13/2013 9:07:23 PM
 *  Author: paul trandem
 *  Copyright (c) 2013 Paul Trandem
 */ 

#include "TempDriver.h"
#include "TransistorLabs/avr.h"
#include "Config/HardwareConfig.h"

#include <avr/interrupt.h>
#include <stdbool.h>
#include <util/atomic.h>

// The TempReadDelayMs defines the number of milliseconds between
// actual sensor reads.  The data itself is "cached" for retrieval by
// the GetTemp___() functions.

// The conversion time for the LM74 is typically 280ms, with a max of 425ms
// Initial value for the counter is 12c, or 300 ms nominal, 

const static uint16_t TempReadDelayMs = 0x03e8;
static volatile uint16_t readDelayMsCounter = 0x0001;
static volatile bool shouldReadTemp = true;

static TempDriver_TemperatureData currentTempData;

static float currentTempC = 21.1111; // ~70 degrees F

void Shutdown(bool shutdown);

void TempDriver_Init(void)
{
	// SPI Port Configuration
	SETPINDIRECTION_OUTPUT(SPI_PORTDIRECTION, SPI_MOSI);
	SETPINDIRECTION_OUTPUT(SPI_PORTDIRECTION, SPI_SCK);
	SETPINDIRECTION_OUTPUT(TEMP_CHIPSELECT_DIRECTION, TEMP_CHIPSELECT);
	EnableSPIMaster_Div16();
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
		// Bring the device out of shutdown
		Shutdown(false);
		
		uint8_t highByte;
		uint8_t lowByte;
		
		Temp_SelectChip();
		
		SPI_DATA = 0x01;
		WaitForSPI_XferComplete();
		highByte = SPI_DATA;
		
		SPI_DATA = 0x01;
		WaitForSPI_XferComplete();
		lowByte = SPI_DATA;
		
		Temp_DeselectChip();
		
		currentTempData.RawDataHigh = highByte;
		currentTempData.RawDataLow = lowByte;
		
		if(currentTempData.ValidData)
		{
			uint8_t tempWhole = currentTempData.RawDataHigh << 1;
			currentTempC = (float)tempWhole;
			
			if ((currentTempData.FractionData & 0x01) > 0) { currentTempC += 0.0625F; }
			if ((currentTempData.FractionData & 0x02) > 0) { currentTempC += 0.125F; }
			if ((currentTempData.FractionData & 0x04) > 0) { currentTempC += 0.25F; }
            if ((currentTempData.FractionData & 0x08) > 0) { currentTempC += 0.5F; }
			
			// If we got valid data, shut down the device
			Shutdown(true);
		}
		
		shouldReadTemp = false;
	}
}

void TempDriver_GetTempDataStructure(TempDriver_TemperatureData * const tempData)
{
	*tempData = currentTempData;
}

float TempDriver_GetTempC(void)
{
	return currentTempC;
}

float TempDriver_GetTempF(void)
{
	return currentTempC*9/5 + 32;
}

void Shutdown(bool shutdown)
{
	Temp_SelectChip();

	if(shutdown == true)
	{
		SPI_DATA = 0xff;
		WaitForSPI_XferComplete();
		SPI_DATA = 0xff;
	}
	else
	{
		SPI_DATA = 0x00;
		WaitForSPI_XferComplete();
		SPI_DATA = 0x00;
	}
	
	WaitForSPI_XferComplete();
	Temp_DeselectChip();
}