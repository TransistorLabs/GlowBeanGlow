/*
 * InputDriver.c
 *
 * Created: 2/3/2013 4:23:22 PM
 *  Author: Paul Trandem
 */ 

#include "InputDriver.h"
#include "Config/HardwareConfig.h"

#define DEBOUNCECHECKS		5

static void DebounceSwitch(void);

volatile static uint8_t debouncedButtonState;
volatile static uint8_t lastDebouncedButtonState;
volatile static uint8_t buttonState[DEBOUNCECHECKS];
volatile static uint8_t buttonStateIndex = 0;
volatile static uint8_t buttonKeyDown;
volatile static uint8_t buttonKeyUp;

static void (*EVENT_ButtonDown)(uint8_t buttonMask);
static void (*EVENT_ButtonUp)(uint8_t buttonMask);

void InputDriver_Init(void (*buttonDownEvent)(uint8_t buttonMask), void (*buttonUpEvent)(uint8_t buttonMask))
{
	EVENT_ButtonDown = buttonDownEvent;
	EVENT_ButtonUp = buttonUpEvent;
}

void InputDriver_Task()
{
	DebounceSwitch();
	if(buttonKeyDown > 0) EVENT_ButtonDown(buttonKeyDown);
	if(buttonKeyUp > 0) EVENT_ButtonUp(buttonKeyUp);
}

static void DebounceSwitch(void)
{
	uint8_t i, j;
	
	//Get the port state
	buttonState[buttonStateIndex] = ~(SWITCHPINPORT);
	++buttonStateIndex;
	j = 0xff;
	
	for(i=0; i<DEBOUNCECHECKS; i++) j = j & buttonState[i];
	
	//The bit is only set in the debounced variable if it has been on for all iterations of DEBOUNCECHECKS
	// (meaning it's now clean and properly debounced)
	// The output bits we don't actually care about are &'d out using the BUTTONPORTMASK
	lastDebouncedButtonState = debouncedButtonState;
	debouncedButtonState = j & BUTTONPORTMASK;
	
	// Isolate transitions
	buttonKeyDown = debouncedButtonState & ~lastDebouncedButtonState;
	buttonKeyUp = ~debouncedButtonState & lastDebouncedButtonState;
	
	//circular
	if(buttonStateIndex >= DEBOUNCECHECKS) buttonStateIndex = 0;
}	