/*
 * Created: 1/13/2013 10:15:25 AM
 *  Author: paul trandem
 *  Copyright (c) 2013 Paul Trandem

Defines the hardware layout for GBG port pins as connected to an ATMEGAxxU2

 */ 

#ifndef LEDCONFIG_H_
#define LEDCONFIG_H_

#include <avr/io.h>

#define PORTCONFIG_LEDCOUNT	12
#define PORTCONFIG_SWITCHCOUNT	2
	
#define LED0PIN		PB5
#define LED1PIN		PB4
#define LED2PIN		PB0
#define LED3PIN		PD7
#define LED4PIN		PD5
#define LED5PIN		PD4
#define LED6PIN		PD1
#define LED7PIN		PC2
#define LED8PIN		PD0
#define LED9PIN		PC4
#define LED10PIN	PC7
#define LED11PIN	PB6
	
#define LED0PORT	&PORTB
#define LED1PORT	&PORTB
#define LED2PORT	&PORTB
#define LED3PORT	&PORTD
#define LED4PORT	&PORTD
#define LED5PORT	&PORTD
#define LED6PORT	&PORTD
#define LED7PORT	&PORTC
#define LED8PORT	&PORTD
#define LED9PORT	&PORTC
#define LED10PORT	&PORTC
#define LED11PORT	&PORTB
	
#define	SW_MODE					PD3
#define	SW_USERA				PD6
	
// Used for masking the debouncer output
#define	BUTTONPORTMASK	0x48; // PD6, PD3,
	
#define SWITCHPINPORT			PIND
#define SWITCHPORT				PORTD
#define SWITCHDIRECTION			DDRD
	
#define RED	OCR1A
#define GRN	OCR1B
#define BLU	OCR1C
		
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

void PortConfig_InitHardware(void);

#endif /* LEDCONFIG_H_ */
