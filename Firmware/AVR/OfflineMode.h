/*
 * Created: 2/3/2013 1:33:20 PM
 *  Author: paul trandem
 *  Copyright (c) 2013 Paul Trandem
 */ 


#ifndef OFFLINEMODE_H_
#define OFFLINEMODE_H_

	#include "Drivers/LedDriver.h"

	#define NUMOFFLINEMODES	5

	typedef enum
	{
		OfflineMode_Static,
		OfflineMode_Temp,
		OfflineMode_Cycle,
		OfflineMode_Animate,
		OfflineMode_Off
	} OfflineMode_ModeOptions;

	void OfflineMode_Init(void);
	void OfflineMode_GetNextFrame(LedDriver_OneColorFrame * const frameData );
	void OfflineMode_SetNextOfflineMode(void);

#endif /* OFFLINEMODE_H_ */