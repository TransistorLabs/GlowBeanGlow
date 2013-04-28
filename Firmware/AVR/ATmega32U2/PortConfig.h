/*
 * Created: 1/13/2013 10:15:25 AM
 *  Author: paul trandem
 *  Copyright (c) 2013 Paul Trandem

Defines the hardware layout for GBG port pins as connected to an ATMEGAxxU2

 */ 


#ifndef LEDCONFIG_H_
#define LEDCONFIG_H_

	#include <avr/io.h>

	#define PORTCONFIG_LEDCOUNT	11

	#define LED0PIN		PB5
	#define LED1PIN		PB4
	#define LED2PIN		PB0
	#define LED3PIN		PD5
	#define LED4PIN		PD4
	#define LED5PIN		PD1
	#define LED6PIN		PD0
	#define LED7PIN		PC2
	#define LED8PIN		PC4
	#define LED9PIN		PC7
	#define LED10PIN	PB6

	#define LED0PORT	&PORTB
	#define LED1PORT	&PORTB
	#define LED2PORT	&PORTB
	#define LED3PORT	&PORTD
	#define LED4PORT	&PORTD
	#define LED5PORT	&PORTD
	#define LED6PORT	&PORTD
	#define LED7PORT	&PORTC
	#define LED8PORT	&PORTC
	#define LED9PORT	&PORTC
	#define LED10PORT	&PORTB

	#define RED	OCR1A
	#define GRN	OCR1B
	#define BLU	OCR1C
	
	#define	SW_MODE					PORTD3
	#define	SW_USERA				PORTD6
	#define	SW_USERB				PORTD7
	
	#define SWITCHPINPORT			PIND
	#define SWITCHPORT				PORTD
	#define SWITCHDIRECTION			DDRD
	
	#define SPI_PORTDIRECTION		DDRB
	#define SPI_MOSI				PORTB2
	#define SPI_SCK					PORTB1
	#define SPI_DATA				SPDR
	
	#define TEMP_CHIPSELECT_PORT		PORTD
	#define TEMP_CHIPSELECT_DIRECTION	DDRD
	#define TEMP_CHIPSELECT				PORTD2
	
	#define EnableSPIMaster_Div16()		SPCR = (1 << SPE) | ( 1 << MSTR) | (1 << SPR0)
	#define SPI_SetCPOLHigh()			SPCR |= _BV(CPOL)
	#define WaitForSPI_XferComplete()	while(!(SPSR & (1 << SPIF))){}
	#define Temp_SelectChip()	TEMP_CHIPSELECT_PORT &= ~_BV(TEMP_CHIPSELECT)
	#define Temp_DeselectChip()	TEMP_CHIPSELECT_PORT |= _BV(TEMP_CHIPSELECT)
	
	// Used for masking the debouncer output
	#define	BUTTONPORTMASK	0xC8; // PD7, PD6, PD3, 

	void PortConfig_InitHardware(void);

#endif /* LEDCONFIG_H_ */
