/*
 * Created: 2/3/2013 4:23:11 PM
 *  Author: paul trandem
 *  Copyright (c) 2013 Paul Trandem
 */ 


#ifndef INPUTDRIVER_H_
#define INPUTDRIVER_H_

	#include <avr/io.h>
	
	#define INPUTDRIVER_MODESWITCHKEYMASK(buttonUpMask) (_BV(SW_MODE) & buttonUpMask)

	void InputDriver_Init(void (*buttonDownEvent)(uint8_t buttonMask), void (*buttonUpEvent)(uint8_t buttonMask));
	void InputDriver_Task(void);
	
#endif /* INPUTDRIVER_H_ */