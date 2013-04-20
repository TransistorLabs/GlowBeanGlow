/*
 * Created: 2/13/2013 9:05:07 PM
 *  Author: paul trandem
 *  Copyright (c) 2013 Paul Trandem
 */ 


#ifndef TEMPDRIVER_H_
#define TEMPDRIVER_H_

	#include <avr/io.h>
	
	typedef union
	{
		uint16_t RawData;

		struct 
		{
			uint8_t RawDataHigh;
			uint8_t RawDataLow;
		};
		
		struct
		{
			uint16_t	IsNegative		: 1,
						WholeNumberData	: 8,
						FractionData	: 4,
						ValidData		: 1,
						Undefined		: 2;
		};
	} TempDriver_TemperatureData;

	void TempDriver_Init(void);
	
	// Handles millisecond tasks for the driver
	// Should be called in the MS interrupt
	void TempDriver_MillisecondTask(void);
	
	// Will perform tasks of retrieving the temp from the sensor
	// Should be called in the main loop
	void TempDriver_Task(void);
	
	// returns the current raw data structure in two bytes
	void TempDriver_GetTempDataStructure(TempDriver_TemperatureData * const tempData);
	
	// returns the current calculated Celsius value
	uint8_t TempDriver_GetTempC(void);


#endif /* TEMPDRIVER_H_ */