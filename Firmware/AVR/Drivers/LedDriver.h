/*
 * LEDs.h
 *
 * Created: 1/9/2013 7:27:12 PM
 *  Author: Paul Trandem
 */ 


#ifndef LedDriver_H_
#define LedDriver_H_

	#include <avr/common.h>
	#include <stdbool.h>
	
	/* Data Structures */
	typedef union 
	{
		struct 
		{
			uint16_t	Led1	: 1,
						Led2	: 1,
						Led3	: 1,
						Led4	: 1,
						Led5	: 1,
						Led6	: 1,
						Led7	: 1,
						Led8	: 1,
						Led9	: 1,
						Led10	: 1,
						Led11	: 1,
						Led12	: 1,
						Led13	: 1,
						Led14	: 1,
						Led15	: 1,
						Led16	: 1;
		};
		uint16_t RawData;
	} LedDriver_LedState;
	
	typedef struct {
		uint8_t Red;
		uint8_t Green;
		uint8_t Blue;
		LedDriver_LedState LedState;
		uint16_t MillisecondsHold;
		uint8_t Reserved;
	} LedDriver_Frame;
	

	/* Function Prototypes */

	// Initialize the driver
	void LedDriver_Init(void (*CALLBACK_GetNextFrame)(LedDriver_Frame * const nextFrame));
	
	// Should be called in a main loop, as often as possible to ensure rendering occurs correctly
	bool LedDriver_Task(void);
	
	// Should be called at every millisecond tick
	void LedDriver_MillisecondTask(void);
	
	// Set the frame to be rendered on the next render pass
	void LedDriver_RenderFrame(const LedDriver_Frame * const frameData);
	void LedDriver_RenderKeyFrame(uint8_t red, uint8_t green, uint8_t blue, uint16_t ledsMask, uint16_t millisecondHold);
	
	// Clear the frame data
	void LedDriver_Clear(void);
	
	// Set test output frame data
	void LedDriver_TestWhite(void);
	void LedDriver_TestColor(uint8_t red, uint8_t green, uint8_t blue);
	
	
#endif /* LedDriver_H_ */