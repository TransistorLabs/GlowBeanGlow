/*
 * Created: 1/13/2013 10:37:16 AM
 *  Author: paul trandem
 *  Copyright (c) 2013 Paul Trandem
 
 Methods for configuring the ATMEGAxxU2 hardware for LEDs (PWM, etc)
 
 */ 

#include "PortConfig.h"
#include "TransistorLabs/avr.h"

void PortConfig_InitHardware(void)
{
	/*
		The ATmegaxxU2 series has two timers, Timer0 and Timer1.  
		Timer1 features three 8-bit PWM outputs, so we'll use that one for all PWM needs.
	*/
	
	// Configure Timer1 for Fast PWM 8-bit
	TCCR1A |= _BV(WGM10);
	TCCR1B |= _BV(WGM12);
	
	// Turn on OC1A - inverted
	TCCR1A |=  _BV(COM1A1) | _BV(COM1A0);
	
	TCCR1A |=  _BV(COM1B1) | _BV(COM1B0);
	
	TCCR1A |=  _BV(COM1C1) | _BV(COM1C0);
	
	RED = 0x0000;
	GRN = 0x0000;
	BLU = 0x0000;
	

	// Configure port outputs (1)
	DDRC |= _BV(PC2) | _BV(PC4) | _BV(PC5) | _BV(PC6) | _BV(PC7);
	DDRB |= _BV(PB0) | _BV(PB4) | _BV(PB5) | _BV(PB6) | _BV(PB7);
	DDRD |= _BV(PD0) | _BV(PD4) | _BV(PD5) | _BV(PD1);
	
	// enable PWM - using system clock, no prescaler
	TCCR1B |= _BV(CS10);
	
	// Configure the input hardware
	
	// Configure switch 1 as input (DDRx = 0)
	SETPINDIRECTION_INPUT(SWITCHDIRECTION, SW_MODE);
	SETPINDIRECTION_INPUT(SWITCHDIRECTION, SW_USERA);
	SETPINDIRECTION_INPUT(SWITCHDIRECTION, SW_USERB);

	// Turn on internal pull-up (1)
	SETINTERNALPULLUP_ON(SWITCHPORT, SW_MODE );
	SETINTERNALPULLUP_ON(SWITCHPORT, SW_USERA );
	SETINTERNALPULLUP_ON(SWITCHPORT, SW_USERB );
	
}
